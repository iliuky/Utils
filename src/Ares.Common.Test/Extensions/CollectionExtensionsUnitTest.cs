using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ares.Common.Extensions;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Ares.Common.Test.Extensions
{
    [TestClass]
    public class CollectionExtensionsUnitTest
    {
        [TestMethod]
        public void IsNullOrEmpty()
        {
            IEnumerable<int> collection1 = Enumerable.Empty<int>();
            IEnumerable<int> collection2 = null;
            IEnumerable<int> collection3 = Enumerable.Range(0, 0);
            IEnumerable<int> collection4 = Enumerable.Range(0, 1);
            IEnumerable<string> collection5 = new List<string> { "1", "2" };
            IEnumerable<string> collection6 = new string[] { "1", "2" };

            Assert.IsTrue(collection1.IsNullOrEmpty(), "空数组, 是否为空判断验证失败");
            Assert.IsTrue(collection2.IsNullOrEmpty(), "对象空, 是否为空判断验证失败");
            Assert.IsTrue(collection3.IsNullOrEmpty(), "空延时对象, 是否为空判断验证失败");
            Assert.IsFalse(collection4.IsNullOrEmpty(), "有值延时对象, 是否为空判断验证失败");
            Assert.IsFalse(collection5.IsNullOrEmpty(), "有值的List, 是否为空判断验证失败");
            Assert.IsFalse(collection6.IsNullOrEmpty(), "有值的数组, 是否为空判断验证失败");
        }

        [TestMethod]
        public void Split()
        {
            int[] arr = null;
            //arr.Split(3);
            arr = new int[0];
            Assert.AreEqual(arr.Length, arr.Split(10).Sum(u => u.Count()));
            arr = new int[1001];
            Assert.AreEqual(arr.Length, arr.Split(100).Sum(u => u.Count()));
            //for (int i = 0; i < 1234; i++)
            //{
            //    arr = new int[i];
            //    Assert.AreEqual(i, arr.Split(System.Math.Max(i & 15, 1)).Sum(u => u.Count()));
            //}
        }

        [TestMethod]
        public void ToParamString()
        {
            var dict = new Dictionary<string, int>
            {
                ["dd"] = 1
            };

            Assert.AreEqual("dd=1", dict.ToParamString());
            Assert.AreEqual("dd,1", dict.ToParamString("0000",","));

            dict["db"] = 5;
            Assert.AreEqual("dd=1&db=5", dict.ToParamString());
            Assert.AreEqual("dd,10000db,5", dict.ToParamString("0000", ","));

            dict["hh"] = 88;
            Assert.AreEqual("dd=1&db=5&hh=88", dict.ToParamString());
            Assert.AreEqual("dd,1|db,5|hh,88", dict.ToParamString("|", ","));
        }
    }
}
