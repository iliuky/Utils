using System;
using System.Collections.Generic;
using System.Text;

namespace Ares.Common.Extensions
{
    /// <summary>
    /// 字符串类型 扩展类
    /// </summary>
    public static class StringExtensions
    {
        #region 字符串判断

        /// <summary>
        /// 判断字符串是否为空 如果等于string.Empty 或者 null,  就返回 true
        /// </summary>
        /// <param name="value"></param>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 判断字符串是否为空 如果等于空白字符 或者 null,  就返回 true
        /// </summary>
        /// <param name="value"></param>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 判断字符串是否为空 如果等于string.Empty 或者 null,  就返回 defaultValue
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">返回的默认值</param>
        /// <returns></returns>
        public static string IfNullOrEmpty(this string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>
        /// 判断字符串是否为空 如果等于string.Empty 或者 null,  就返回 elementSelector 委托获取的值
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="elementSelector">获取默认值的委托</param>
        /// <returns></returns>
        public static string IfNullOrEmpty(this string value, Func<string> elementSelector)
        {
            return string.IsNullOrEmpty(value) ? elementSelector() : value;
        }

        #endregion
    }
}
