using Ares.Common.Extensions;
using Ares.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ares.Common.Test.Extensions
{
    [TestClass]
    public class EnumExtensionsUnitTest
    {
        [TestMethod]
        public void GetDescription()
        {
            //for (int i = 0; i < 10000000; i++)
            //{
            //    EErrorCode.DbError.GetDescription();
            //}

            var empty = EErrorCode.DbError.GetDescription();
            Assert.IsNotNull(empty);
        }

    }

    #region 枚举定义

    /// <summary>
    /// 错误状态定义
    /// </summary>
    public enum EErrorCode : int
    {
        [Description("权限错误")]
        Unauthorized = 400,

        [Description("数据库错误")]
        DbError = 500,
    }


    #endregion
}
