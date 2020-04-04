using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ares.Common.Internal
{
    /// <summary>
    /// 构建枚举描述帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class BuilderEnumDescription<T> where T : Enum
    {
        private static readonly Dictionary<T, string> Description;

        static BuilderEnumDescription()
        {
            Description = InitFieldDesc();
        }

        /// <summary>
        /// 获取枚举 T 字段的描述
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetDescription(T value)
        {
            if (!Description.TryGetValue(value, out var desc))
            {
                throw new ArgumentOutOfRangeException($"{typeof(T).Name}.{value.ToString()} 不存在特性{nameof(DescriptionAttribute)}");
            }

            return desc;
        }

 
        /// <summary>
        /// 初始化枚举 T 字段的描述
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        private static Dictionary<T, string> InitFieldDesc()
        {
            var type = typeof(T);
            var values = Enum.GetValues(type);
            var description = new Dictionary<T, string>(values.Length);
            foreach (T value in values)
            {
                var field = type.GetField(value.ToString());
                foreach (var attr in field.GetCustomAttributes(false))
                {
                    if (!(attr is DescriptionAttribute desAttr)) continue;
                    description[value] = desAttr.Description;
                }
            }

            return description;
        }
    }
}
