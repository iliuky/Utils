using System;
using System.Linq;
using System.Collections.Generic;
using Ares.Common.Internal;

namespace Ares.Common.Extensions
{
    /// <summary>
    /// 枚举类型 扩展类
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举字段描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">枚举值.</param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value) where T : Enum
        {
            return BuilderEnumDescription<T>.GetDescription(value);
        }
    }
}
