using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public class DateTimeToStringHelperTest
    {
        [TestMethod]
        [DataRow("2023-05-01 05:01:02")]
        [DataRow("2023-12-11 20:21:22")]
        public void ToFullString(string timeStr)
        {
            var time = DateTime.Parse(timeStr);
            var exp= time.ToString("yyyy-MM-dd HH:mm:ss");
            var act=DateTimeToStringHelper.ToFullString(ref time);
            Assert.AreEqual(exp, act);
        }
        [TestMethod]
        [DataRow("2023-05-01 05:01:02")]
        [DataRow("2023-12-11 20:21:22")]
        public void ToFullStringWithSelf(string timeStr)
        {
            var time = DateTime.Parse(timeStr);
            var exp = time.ToString("yyyy/MM/dd HH$mm$ss");
            var act = DateTimeToStringHelper.ToFullString(ref time,'/','$');
            Assert.AreEqual(exp, act);
        }
        [TestMethod]
        [DataRow("2023-05-01 05:01:02")]
        [DataRow("2023-12-11 20:21:22")]
        public void ToTimeString(string timeStr)
        {
            var time = DateTime.Parse(timeStr);
            var exp = time.ToString("HH:mm:ss");
            var act = DateTimeToStringHelper.ToTimeString(ref time);
            Assert.AreEqual(exp, act);
        }
        [TestMethod]
        [DataRow("2023-05-01 05:01:02")]
        [DataRow("2023-12-11 20:21:22")]
        public void ToTimeStringSelf(string timeStr)
        {
            var time = DateTime.Parse(timeStr);
            var exp = time.ToString("HH&mm&ss");
            var act = DateTimeToStringHelper.ToTimeString(ref time,'&');
            Assert.AreEqual(exp, act);
        }
        [TestMethod]
        [DataRow("2023-05-01 05:01:02")]
        [DataRow("2023-12-11 20:21:22")]
        public void ToDateString(string timeStr)
        {
            var time = DateTime.Parse(timeStr);
            var exp = time.ToString("yyyy-MM-dd");
            var act = DateTimeToStringHelper.ToDateString(ref time);
            Assert.AreEqual(exp, act);
        }
        [TestMethod]
        [DataRow("2023-05-01 05:01:02")]
        [DataRow("2023-12-11 20:21:22")]
        public void ToDateStringSelf(string timeStr)
        {
            var time = DateTime.Parse(timeStr);
            var exp = time.ToString("yyyy_MM_dd");
            var act = DateTimeToStringHelper.ToDateString(ref time, '_');
            Assert.AreEqual(exp, act);
        }
    }
}
