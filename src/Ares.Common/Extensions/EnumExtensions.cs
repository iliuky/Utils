using System;
using System.Linq;
using System.Collections.Generic;
using Ares.Common.Attributes;
using Ares.Common.Inside;

namespace Ares.Common.Extensions
{
    /// <summary>
    /// 枚举类型 扩展类
    /// </summary>
    public static class EnumExtensions
    {
        #region 枚举描述

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">枚举值.</param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value) where T : Enum
        {
            return !Enum.IsDefined(typeof(T), value) ? string.Empty : BuilderEnumDescription<T>.GetDescription(value);
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">枚举值</param>
        /// <param name="scope">业务名称.</param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value, string scope) where T : Enum
        {
            return !Enum.IsDefined(typeof(T), value) ? string.Empty : BuilderEnumDescription<T>.GetDescription(value, scope);
        }

        #endregion

        
    }
}
