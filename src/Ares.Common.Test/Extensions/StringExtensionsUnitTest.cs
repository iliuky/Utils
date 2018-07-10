using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ares.Common.Extensions;

namespace Ares.Common.Test.Extensions
{
    [TestClass]
    public class StringExtensionsUnitTest
    {
        [TestMethod]
        public void IsNullOrEmpty()
        {
            string value1 = string.Empty;
            string value2 = null;
            string value3 = "test";

            Assert.IsTrue(value1.IsNullOrEmpty(), "×Ö·û´®¿Õ, ÊÇ·ñÎª¿ÕÅÐ¶ÏÑéÖ¤Ê§°Ü");
            Assert.IsTrue(value2.IsNullOrEmpty(), "¶ÔÏó¿Õ, ÊÇ·ñÎª¿ÕÅÐ¶ÏÑéÖ¤Ê§°Ü");
            Assert.IsFalse(value3.IsNullOrEmpty(), "×Ö·û´®, ÊÇ·ñÎª¿ÕÅÐ¶ÏÑéÖ¤Ê§°Ü");
        }

        [TestMethod]
        public void IfNullOrEmpty()
        {
            string value1 = string.Empty;
            string value2 = null;
            string value3 = "test";

            Assert.AreEqual("", value1.IfNullOrEmpty(""));
            Assert.AreEqual("123", value2.IfNullOrEmpty("123"));
            //Assert.AreEqual("¹þ¹þ¹þ", value3.IfNullOrEmpty(() => "¹þ¹þ¹þ"));
            Assert.AreEqual("¹þ¹þ¹þ", value2.IfNullOrEmpty(() => "¹þ¹þ¹þ"));
            Assert.AreEqual("test", value3.IfNullOrEmpty("¹þ¹þ¹þ"));
        }
    }
}
