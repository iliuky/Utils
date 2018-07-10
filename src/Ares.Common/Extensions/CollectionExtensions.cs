using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Ares.Common.Extensions
{
    /// <summary>
    /// 集合类型 扩展类
    /// </summary>
    public static class CollectionExtensions
    {
        #region 集合判断

        /// <summary>
        /// 判断集合是否等于空, 等于 null 或者 数量等于 0 就返回 true
        /// </summary>
        /// <param name="value">The string.</param>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            switch (value)
            {
                case null:
                    return true;
                case ICollection<T> collection:
                    return collection.Count == 0;
            }

            return !value.Any();
        }

        #endregion

        #region 类型转换

        /// <summary>
        /// 转换成 IList 集合
        /// </summary>
        /// <param name="value">The string.</param>
        public static IList<T> AsIList<T>(this IEnumerable<T> value)
        {
            switch (value)
            {
                case null:
                    throw new ArgumentNullException(nameof(value));
                case IList<T> ilist:
                    return ilist;
            }

            return value.ToList();
        }

        /// <summary>
        /// 转换成 List 集合
        /// </summary>
        /// <param name="value">The string.</param>
        public static List<T> AsList<T>(this IEnumerable<T> value)
        {
            switch (value)
            {
                case null:
                    throw new ArgumentNullException(nameof(value));
                case List<T> ilist:
                    return ilist;
            }

            return value.ToList();
        }

        #endregion

        #region 转换参数

        /// <summary>
        /// 转URL 参数形式字符串
        /// </summary>
        /// <param name="value">The dictionary.</param>
        /// <returns></returns>
        public static string ToParamString<TValue>(this IEnumerable<KeyValuePair<string, TValue>> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var list = value.AsIList();
            var enumerator = list.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return string.Empty;
            }

            var buff = new StringBuilder(Math.Max(16, 12 * list.Count));
            buff.AppendFormat("{0}={1}", enumerator.Current.Key, enumerator.Current.Value.ToString());
            while (enumerator.MoveNext())
            {
                buff.AppendFormat("&{0}={1}", enumerator.Current.Key, enumerator.Current.Value.ToString());
            }
            return buff.ToString();
        }

        /// <summary>
        /// 转 参数形式字符串
        /// </summary>
        /// <typeparam name="TValue">字典值的类型</typeparam>
        /// <param name="value">字典</param>
        /// <param name="separator">分隔符 & </param>
        /// <param name="equal">对等符号 = </param>
        /// <returns></returns>
        public static string ToParamString<TValue>(this IEnumerable<KeyValuePair<string, TValue>> value, string separator, string equal)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var list = value.AsIList();
            var enumerator = list.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return string.Empty;
            }

            var buff = new StringBuilder(Math.Max(16, 12 * list.Count));
            buff.AppendFormat("{0}{1}{2}", enumerator.Current.Key, equal, enumerator.Current.Value.ToString());
            while (enumerator.MoveNext())
            {
                buff.AppendFormat("{0}{1}{2}{3}", separator, enumerator.Current.Key, equal, enumerator.Current.Value.ToString());
            }
            return buff.ToString();
        }

        #endregion

        #region 集合分割

        /// <summary>
        /// 将集合分割成指定大小的小集合
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="value">集合.</param>
        /// <param name="leng">分割后子集合大小.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> value, int leng)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (leng < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "参数" + nameof(leng) + "必须大于 0 ");
            }
            
            var source = (value as ICollection<T>) ?? value.ToArray();
            if (source.Count == 0)
            {
                return Enumerable.Empty<IEnumerable<T>>();
            }
            
            return source.Count <= leng ? new IEnumerable<T>[] { source } : SplitIterator(source, leng);
        }

        /// <summary>
        /// 将集合分割成指定大小的小集合
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="value">集合.</param>
        /// <param name="leng">分割后子集合大小.</param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<T>> SplitIterator<T>(ICollection<T> value, int leng)
        {
            var count = value.Count;
            var pageIndex = (count - 1) / leng + 1;
            for (var i = 0; i < pageIndex; i++)
            {
                yield return value.Skip(i * leng).Take(leng);
            }
        }

        #endregion
    }
}
