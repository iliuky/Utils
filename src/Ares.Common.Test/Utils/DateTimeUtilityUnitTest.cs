using Ares.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ares.Common.Test.Utils
{
    [TestClass]
    public class DateTimeUtilityUnitTest
    {
        [TestMethod]

        public void DateTimeToUnixTimeStamp()
        {
            var now = DateTime.Now;

            var timeStamp1 = DateTimeUtility.DateTimeToUnixTimeStamp(now);
            var now1 = DateTimeUtility.UnixTimeStampToDateTime(timeStamp1);

            Assert.IsTrue((now1 - now).TotalSeconds < 1);
        }
    }
}
