using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ares.Common.Attributes;
using Ares.Common.Extensions;

namespace Ares.Common.Inside
{
    /// <summary>
    /// 构建枚举描述帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class BuilderEnumDescription<T> where T : Enum
    {
        private static readonly Dictionary<T, IEnumerable<(string Scope, string Description)>> _description;
        private static readonly IEnumerable<(string Scope, string Description)> _enumDescription;

        static BuilderEnumDescription()
        {
            _description = InitFieldDesc();
            _enumDescription = InitEnumDesc();
        }

        /// <summary>
        /// 获取枚举 T 字段的描述
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetDescription(T value)
        {
            return GetDescription(value, string.Empty);
        }

        /// <summary>
        /// 获取枚举 T 字段的描述
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="scope">业务名称.</param>
        /// <returns></returns>
        public static string GetDescription(T value, string scope)
        {
            if (!_description.TryGetValue(value, out var dess))
            {
                throw new ArgumentOutOfRangeException($"{typeof(T).Name}.{value.ToString()} 不存在特性{nameof(EnumDescriptionAttribute)}");
            }

            var (_, description) = dess.FirstOrDefault(u => u.Scope == scope.ToLower());

            if (description == null)
            {
                throw new ArgumentOutOfRangeException($"{typeof(T).Name}.{value.ToString()} 不存在特性{nameof(EnumDescriptionAttribute)}.Scope:{scope} ");
            }

            return description;
        }

        /// <summary>
        /// 获取枚举 T 的描述
        /// </summary>
        /// <returns></returns>
        public static string GetEnumDescription()
        {
            return GetEnumDescription(string.Empty);
        }

        /// <summary>
        /// 获取枚举 T 的描述
        /// </summary>
        /// <param name="scope">业务名称.</param>
        /// <returns></returns>
        public static string GetEnumDescription(string scope)
        {
            var (_, description) = _enumDescription.FirstOrDefault(u => u.Scope == scope.ToLower());
            if (description == null)
            {
                throw new ArgumentOutOfRangeException($"{typeof(T).Name} 不存在特性{nameof(EnumDescriptionAttribute)}.Scope:{scope} ");
            }

            return description;
        }

        /// <summary>
        /// 初始化枚举 T 字段的描述
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        private static Dictionary<T, IEnumerable<(string Scope, string Description)>> InitFieldDesc()
        {
            var type = typeof(T);
            var values = Enum.GetValues(type);
            var description = new Dictionary<T, IEnumerable<(string Scope, string Description)>>(values.Length);
            foreach (T value in values)
            {
                var field = type.GetField(value.ToString());
                var descList = new List<(string Scope, string Description)>();
                foreach (var attr in field.GetCustomAttributes(false))
                {
                    if (!(attr is EnumDescriptionAttribute scopeDesAttr)) continue;
                    if (descList.Any(u => u.Scope.Equals(scopeDesAttr.Scope, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        throw new Exception($"{type.Name}.{value} 定义了重复的特性 {nameof(EnumDescriptionAttribute)}.Scope {scopeDesAttr.Scope}");
                    }
                    descList.Add((scopeDesAttr.Scope.ToLower(), scopeDesAttr.Description));
                }

                description.Add(value, descList.ToArray());
            }

            return description;
        }

        /// <summary>
        /// 初始化枚举 T 类的描述
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static IEnumerable<(string scope, string Description)> InitEnumDesc()
        {
            var type = typeof(T);
            var list = new List<(string Scope, string Description)>();
            foreach (var attr in type.GetCustomAttributes(false))
            {
                if (!(attr is EnumDescriptionAttribute scopeDesAttr)) continue;
                if (list.Any(u => u.Scope.Equals(scopeDesAttr.Scope, StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new Exception($"{type.Name} 定义了重复的特性 {nameof(EnumDescriptionAttribute)}.Scope {scopeDesAttr.Scope}");
                }
                list.Add((scopeDesAttr.Scope.ToLower(), scopeDesAttr.Description));
            }

            return list.ToArray();
        }
    }
}
