using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public class ValueListFindExtensionsTest
    {
        [TestMethod]
        public void ToList()
        {
            var list = new ValueList<int>(1);
            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }
            var l = ValueListFindExtensions.ToList(list);
            Assert.AreEqual(list.Size, list.Size);
            for (int i = 0; i < list.Size; i++)
            {
                Assert.AreEqual(list[i], l[i]);
            }
            list.Dispose();
        }
        [TestMethod]
        public void IndexOf()
        {
            var list = new ValueList<int>(1);
            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }
            var index = ValueListFindExtensions.FindIndex(list, 10);
            Assert.AreEqual(10, index);

            index = ValueListFindExtensions.FindIndex(list, x => x == 10);
            Assert.AreEqual(10, index);

            index = ValueListFindExtensions.FindIndex(list, 999);
            Assert.AreEqual(-1, index);
            list.Dispose();
        }
        [TestMethod]
        public void Contains()
        {
            var list = new ValueList<int>(1);
            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }
            var c = ValueListFindExtensions.Contains(list, 10);
            Assert.IsTrue(c);

            c = ValueListFindExtensions.Contains(list, 999);
            Assert.IsFalse(c);
            list.Dispose();
        }
    }
}
