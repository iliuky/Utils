using System;
using System.Collections.Generic;
using System.Text;

namespace Ares.Common.Extensions
{
    /// <summary>
    /// 字典类型 扩展类
    /// </summary>
    public static class DictionaryExtensions
    {
        #region 取值
        
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TKey">键类型.</typeparam>
        /// <typeparam name="TValue">值类型.</typeparam>
        /// <param name="dict">字典</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">当key 不存在时 返回默认值</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">dict</exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TKey">键类型.</typeparam>
        /// <typeparam name="TValue">值类型.</typeparam>
        /// <param name="dict">字典</param>
        /// <param name="key">键</param>
        /// <param name="elementSelector">当key 不存在时 执行委托获取值</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">dict</exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> elementSelector)
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            return dict.TryGetValue(key, out var value) ? value : elementSelector();
        }

        #endregion
    }
}
