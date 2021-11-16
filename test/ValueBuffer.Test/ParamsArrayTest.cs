using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public class ParamsArrayTest
    {
        [TestMethod]
        public void GivenNullInit_MustThrowExcetion()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ParamsArray((object[])null));
        }
        [TestMethod]
        public void Init()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();

            var arr = new ParamsArray(o1);
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(o1, arr[0]);

            arr = new ParamsArray(new object[] { o1 });
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(o1, arr[0]);

            arr = new ParamsArray(o1, o2);
            Assert.AreEqual(2, arr.Length);
            Assert.AreEqual(o1, arr[0]);
            Assert.AreEqual(o2, arr[1]);

            arr = new ParamsArray(new object[] { o1,o2 });
            Assert.AreEqual(2, arr.Length);
            Assert.AreEqual(o1, arr[0]);
            Assert.AreEqual(o2, arr[1]);

            arr = new ParamsArray(o1, o2,o3);
            Assert.AreEqual(3, arr.Length);
            Assert.AreEqual(o1, arr[0]);
            Assert.AreEqual(o2, arr[1]);
            Assert.AreEqual(o3, arr[2]);

            arr = new ParamsArray(new object[] { o1, o2, o3 });
            Assert.AreEqual(3, arr.Length);
            Assert.AreEqual(o1, arr[0]);
            Assert.AreEqual(o2, arr[1]);
            Assert.AreEqual(o3, arr[2]);

            arr = new ParamsArray(new object[] { o1, o2, o3,o4 });
            Assert.AreEqual(4, arr.Length);
            Assert.AreEqual(o1, arr[0]);
            Assert.AreEqual(o2, arr[1]);
            Assert.AreEqual(o3, arr[2]);
            Assert.AreEqual(o4, arr[3]);
        }
    }
}
