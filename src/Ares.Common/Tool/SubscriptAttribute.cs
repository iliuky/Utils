using System;

namespace Ares.Common.Tool
{
    /// <summary>
    /// 内容下标
    /// </summary>
    public class SubscriptAttribute : Attribute
    {
        public SubscriptAttribute(int order)
        {
            this.Order = order;
        }

        /// <summary>
        /// 下标
        /// </summary>
        /// <value></value>
        public int Order { get; }
    }
}