using System;
using System.Collections.Generic;
using System.Text;

namespace Ares.Common.Attributes
{
    /// <summary>
    /// 枚举多业务描述
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
    public class EnumDescriptionAttribute : Attribute
    {
        public EnumDescriptionAttribute(string description)
        {
            this.Scope = string.Empty;
            this.Description = description;
        }

        public EnumDescriptionAttribute(string scope, string description)
        {
            this.Scope = scope;
            this.Description = description;
        }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// 业务描述
        /// </summary>
        public string Description { get; set; }
    }
}
