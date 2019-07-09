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

            Assert.IsTrue(value1.IsNullOrEmpty(), "�ַ�����, �Ƿ�Ϊ���ж���֤ʧ��");
            Assert.IsTrue(value2.IsNullOrEmpty(), "�����, �Ƿ�Ϊ���ж���֤ʧ��");
            Assert.IsFalse(value3.IsNullOrEmpty(), "�ַ���, �Ƿ�Ϊ���ж���֤ʧ��");
        }

        [TestMethod]
        public void IfNullOrEmpty()
        {
            string value1 = string.Empty;
            string value2 = null;
            string value3 = "test";

            Assert.AreEqual("", value1.IfNullOrEmpty(""));
            Assert.AreEqual("123", value2.IfNullOrEmpty("123"));
            //Assert.AreEqual("������", value3.IfNullOrEmpty(() => "������"));
            Assert.AreEqual("������", value2.IfNullOrEmpty(() => "������"));
            Assert.AreEqual("test", value3.IfNullOrEmpty("������"));
        }
    }
}
