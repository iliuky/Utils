using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ares.Common.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// 进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">ef 对象</param>
        /// <param name="sort">排序表达式"XXX DESC,XXX ASC"</param>
        /// <returns></returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string sort)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (sort.IsNullOrWhiteSpace()) throw new ArgumentNullException("sort");

            var sorts = sort.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var sortArr = new Tuple<string, string>[sorts.Length];
            for (int i = 0; i < sorts.Length; i++)
            {
                var s = sorts[i];
                var kv = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length > 2 || kv.Length == 0)
                {
                    throw new ArgumentException("请检查参数sort格式");
                }
                var key = kv[0];
                var mode = "ASC";
                if (kv.Length == 2)
                {
                    mode = kv[1].ToUpper();
                    if (mode != "DESC" && mode != "ASC")
                    {
                        throw new ArgumentException("请检查参数sort排序方式");
                    }
                }
                sortArr[i] = new Tuple<string, string>(key, mode);
            }

            return OrderByImpl(source, sortArr);
        }

        /// <summary>
        /// 进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">ef 对象</param>
        /// <param name="sorts">排序表达式"XXX DESC"</param>
        /// <returns></returns>
        private static IQueryable<T> OrderByImpl<T>(IQueryable<T> source, IList<Tuple<string, string>> sorts)
        {
            for (int i = 0; i < sorts.Count; i++)
            {
                var item = sorts[i];
                var propertyName = item.Item1.Trim();
                var sortDirection = item.Item2.Trim();

                var parameter = Expression.Parameter(source.ElementType, string.Empty);
                var property = Expression.Property(parameter, propertyName);
                var lambda = Expression.Lambda(property, parameter);

                var methodName = string.Empty;
                if (i == 0)
                {
                    methodName = (sortDirection.ToUpper() == "ASC") ? "OrderBy" : "OrderByDescending";
                }
                else
                {
                    methodName = (sortDirection.ToUpper() == "ASC") ? "ThenBy" : "ThenByDescending";
                }

                var methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                    new Type[] { source.ElementType, property.Type },
                                                    source.Expression, Expression.Quote(lambda));
                source = source.Provider.CreateQuery<T>(methodCallExpression);
            }
            return source;
        }
    }
}