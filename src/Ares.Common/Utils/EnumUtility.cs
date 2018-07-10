using System;
using System.Collections.Generic;
using System.Text;
using Ares.Common.Inside;

namespace Ares.Common.Utils
{
    public class EnumUtility
    {
        /// <summary>
        /// 获取枚举 T 类的描述
        /// </summary>
        /// <param name="scope">业务名称.</param>
        /// <returns></returns>
        public static string GetDescription<T>(string scope) where T : Enum
        {
            return BuilderEnumDescription<T>.GetEnumDescription(scope);
        }

        /// <summary>
        /// 获取枚举 T 类的描述
        /// </summary>
        /// <returns></returns>
        public static string GetDescription<T>() where T : Enum
        {
            return BuilderEnumDescription<T>.GetEnumDescription(string.Empty);
        }
    }
}
