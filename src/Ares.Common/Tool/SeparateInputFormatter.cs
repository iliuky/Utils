using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace Ares.Common.Tool
{
    /// <summary>
    /// 自定义格式表单接收
    /// </summary>
    public class SeparateInputFormatter : TextInputFormatter
    {
        public static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json");
        public static readonly MediaTypeHeaderValue TextJson = MediaTypeHeaderValue.Parse("text/json");
        public static readonly MediaTypeHeaderValue ApplicationAnyJsonSyntax = MediaTypeHeaderValue.Parse("application/*+json");

        private object _lock = new object();
        private Dictionary<Type, IEnumerable<Action<object, string[]>>> SetValueFuncs = new Dictionary<Type, IEnumerable<Action<object, string[]>>>();

        public SeparateInputFormatter()
        {
            base.SupportedEncodings.Add(TextInputFormatter.UTF8EncodingWithoutBOM);
            base.SupportedEncodings.Add(TextInputFormatter.UTF16EncodingLittleEndian);
            base.SupportedMediaTypes.Add(ApplicationJson);
            base.SupportedMediaTypes.Add(TextJson);
            base.SupportedMediaTypes.Add(ApplicationAnyJsonSyntax);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            var request = context.HttpContext.Request;
            if (!request.Body.CanSeek)
            {
                HttpRequestRewindExtensions.EnableBuffering(request);
                await StreamHelperExtensions.DrainAsync(request.Body, CancellationToken.None);
                request.Body.Seek(0L, SeekOrigin.Begin);
            }

            using (TextReader textReader = context.ReaderFactory(request.Body, encoding))
            {
                var modelType = context.ModelType;
                if (modelType.IsArray)
                {
                    var itemType = modelType.GetElementType();
                    var arr = new ArrayList();
                    while (true)
                    {
                        var line = textReader.ReadLine();
                        if (string.IsNullOrEmpty(line)) break;
                        var obj = GetModel(line, itemType);
                        arr.Add(obj);
                    }
                    var modelList = arr.ToArray(itemType);
                    return await InputFormatterResult.SuccessAsync(modelList);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(modelType))
                {
                    var itemType = modelType.GenericTypeArguments[0];
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    IList modelList = (IList)Activator.CreateInstance(listType);
                    while (true)
                    {
                        var line = textReader.ReadLine();
                        if (string.IsNullOrEmpty(line)) break;
                        var obj = GetModel(line, itemType);
                        modelList.Add(obj);
                    }
                    return await InputFormatterResult.SuccessAsync(modelList);
                }
                else
                {
                    var obj = GetModel(textReader.ReadLine(), modelType);
                    return await InputFormatterResult.SuccessAsync(obj);
                }
            }
        }

        /// <summary>
        /// 获取model
        /// </summary>
        /// <param name="text"></param>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private object GetModel(string text, Type modelType)
        {
            var obj = Activator.CreateInstance(modelType);
            if (!SetValueFuncs.TryGetValue(modelType, out var setValuesFunc))
            {
                lock (_lock)
                {
                    if (!SetValueFuncs.TryGetValue(modelType, out setValuesFunc))
                    {
                        setValuesFunc = InitTypeSetValuesFunc(modelType);
                        SetValueFuncs[modelType] = setValuesFunc;
                    }
                }
            }

            var arr = text.Split('|');
            foreach (var func in setValuesFunc)
            {
                func(obj, arr);
            }

            return obj;
        }

        /// <summary>
        /// 初始化设置属性的委托
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private IEnumerable<Action<object, string[]>> InitTypeSetValuesFunc(Type modelType)
        {
            var TpropertyInfo = modelType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var setValuesFuncList = new List<Action<object, string[]>>();
            foreach (var item in TpropertyInfo)
            {
                var attr = item.GetCustomAttributes(typeof(SubscriptAttribute), true).FirstOrDefault() as SubscriptAttribute;
                if (attr == null)
                {
                    continue;
                }

                var index = attr.Order;
                Action<object, string[]> fun;
                if (item.PropertyType == typeof(string))
                {
                    fun = (obj, list) =>
                    {
                        if (list.Length >= index)
                        {
                            item.SetValue(obj, list[index - 1]);
                        }
                    };
                }
                else
                {
                    fun = (obj, list) =>
                    {
                        if (list.Length >= index)
                        {
                            var str = list[index - 1];
                            if (!string.IsNullOrEmpty(str))
                            {
                                object val = null;
                                try
                                {
                                    val = Convert.ChangeType(str, item.PropertyType);
                                }
                                catch
                                {
                                    throw new ArgumentException($"字段{item.Name}, 类型转行失败", item.Name);
                                }
                                item.SetValue(obj, val);
                            }
                        }
                    };
                }

                setValuesFuncList.Add(fun);
            }
            return setValuesFuncList;
        }
    }
}