using Ares.Common.Attributes;
using Ares.Common.Extensions;
using Ares.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            //    var empty = EErrorCode.DbError.GetDescription();
            //    var amicable = EErrorCode.DbError.GetDescription("Amicable");
            //    var detailed = EErrorCode.DbError.GetDescription("Detailed");
            //    var simple = EErrorCode.DbError.GetDescription("Simple");

            //    var enumEmpty = EnumUtility.GetDescription<EErrorCode>();
            //    var enumAmicable = EnumUtility.GetDescription<EErrorCode>("Amicable");
            //    var enumDetailed = EnumUtility.GetDescription<EErrorCode>("Detailed");
            //    var enumSimple = EnumUtility.GetDescription<EErrorCode>("Simple");
            //}

            var empty = EErrorCode.DbError.GetDescription();
            var amicable = EErrorCode.DbError.GetDescription("Amicable");
            var detailed = EErrorCode.DbError.GetDescription("detailed");
            var simple = EErrorCode.DbError.GetDescription("simple");

            var empty1 = EErrorCode.DbError.GetDescription();
            var amicable1 = EErrorCode.DbError.GetDescription("amicable");
            var detailed1 = EErrorCode.DbError.GetDescription("Detailed");
            var simple1 = EErrorCode.DbError.GetDescription("Simple");

            Assert.IsNotNull(empty);
            Assert.IsNotNull(amicable);
            Assert.IsNotNull(detailed);
            Assert.IsNotNull(simple);

            var enumEmpty = EnumUtility.GetDescription<EErrorCode>();
            var enumAmicable = EnumUtility.GetDescription<EErrorCode>("Amicable");
            var enumDetailed = EnumUtility.GetDescription<EErrorCode>("Detailed");
            var enumSimple = EnumUtility.GetDescription<EErrorCode>("Simple");
            Assert.IsNotNull(enumEmpty);
            Assert.IsNotNull(enumAmicable);
            Assert.IsNotNull(enumDetailed);
            Assert.IsNotNull(enumSimple);

            //EErrorCode noExits = 0;
            //noExits.GetDescription("Simple");
            //EErrorCode.DbError.GetDescription("dfasdfsd");
        }

    }

    #region 枚举定义

    /// <summary>
    /// 错误状态定义
    /// </summary>
    [EnumDescription("错误状态定义")]
    [EnumDescription("Amicable", "友好的错误定义")]
    [EnumDescription("Detailed", "详细错误")]
    [EnumDescription("Simple", "简单错误")]
    public enum EErrorCode : int
    {
        [EnumDescription("权限错误")]
        [EnumDescription("Amicable", "授权错误")]
        [EnumDescription("Detailed", "用户权限不足, 授权失败")]
        [EnumDescription("Simple", "系统错误")]
        Unauthorized = 400,

        [EnumDescription("数据库错误")]
        [EnumDescription("Amicable", "系统开小差了, 请稍后")]
        [EnumDescription("Detailed", "数据库连接异常")]
        [EnumDescription("Simple", "系统错误")]
        DbError = 500,
    }


    #endregion
}
