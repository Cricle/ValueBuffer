using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public partial class ValueListTest
    {
        [TestMethod]
        public void InitWithMinThanZero()
        {
            Assert.ThrowsException<ArgumentException>(() => new ValueList<object>(-1));
        }
        [TestMethod]
        public void ZeroAddDispose()
        {
            var list = new ValueList<int>();
            Assert.AreEqual(0, list.Size);
            list.Dispose();
        }
        [TestMethod]
        public void AddNotAlloc()
        {
            var list = new ValueList<int>(7);
            list.Add(0);
            var count = list.LocalCount;
            for (int i = 1; i < count; i++)
            {
                list.Add(i);
            }
            Assert.AreEqual(1, list.BufferSlotIndex);
            Assert.AreEqual(count, list.Size);
            Assert.AreEqual(count, list.LocalCount);
            Assert.AreEqual(count, list.LocalUsed);
            list.Dispose();
        }
        [TestMethod]
        public void AddWhenAlloc()
        {
            var list = new ValueList<int>(7);
            list.Add(0);
            var count = list.LocalCount;
            for (int i = 1; i < count; i++)
            {
                list.Add(i);
            }
            list.Add(10);
            Assert.AreEqual(2, list.BufferSlotIndex);
            Assert.AreEqual(count+1, list.Size);
            Assert.AreEqual(count*2, list.LocalCount);
            Assert.AreEqual(1, list.LocalUsed);
            list.Dispose();
        }
        [TestMethod]
        [DataRow(0, 100)]
        [DataRow(100, 100)]
        [DataRow(100, 1000)]
        [DataRow(999, 1000)]
        [DataRow(50, 10)]
        public void AddWhenGet(int cap,int count)
        {
            var list = new ValueList<int>(cap);
            for (int i = 0; i < count; i++)
            {
                list.Add(i);
            }
            Assert.AreEqual(count, list.Size);
            var enu = list.GetEnumerator();
            var x = 0;
            while (enu.MoveNext())
            {
                Assert.AreEqual(x++, enu.Current);
            }
            var enuSlot = list.GetSlotEnumerator();
            x = 0;
            while (enuSlot.MoveNext())
            {
                for (int j = 0; j < enuSlot.Current.Length; j++)
                {
                    Assert.AreEqual(x++, enuSlot.Current[j]);
                }
                var arr = enuSlot.CurrentArray;
                for (int j = 0; j < enuSlot.Current.Length; j++)
                {
                    Assert.AreEqual(enuSlot.Current[j], arr[j]);
                }
            }
            list.Dispose();
        }
        [TestMethod]
        public void Remove()
        {
            var list = new ValueList<int>(3);
            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }
            Assert.AreEqual(10, list.Size);
            list.RemoveLast();
            Assert.AreEqual(9, list.Size);
            for (int i = 0; i < 9; i++)
            {
                Assert.AreEqual(i, list[i]);
            }
            list.Dispose();
        }
        [TestMethod]
        public void RemoveRange()
        {
            var list = new ValueList<int>(3);
            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }
            Assert.AreEqual(10, list.Size);
            list.RemoveLast(5);
            Assert.AreEqual(5, list.Size);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(i, list[i]);
            }
            list.Dispose();
        }
        [TestMethod]
        [DataRow(0, 100)]
        [DataRow(50, 100)]
        [DataRow(100, 100)]
        [DataRow(101, 100)]
        public void ToArray(int cap,int count)
        {
            var list = new ValueList<int>(cap);
            for (int i = 0; i < count; i++)
            {
                list.Add(i);
            }
            var arr = list.ToArray();

            CheckArray(list, arr);
            list.Dispose();
        }
        [TestMethod]
        [DataRow(100, 101)]
        [DataRow(1, 1)]
        [DataRow(1, 2)]
        public void ToArrayWhenGiven(int count,int arrSize)
        {
            var list = new ValueList<int>();
            for (int i = 0; i < count; i++)
            {
                list.Add(i);
            }
            var arr = new int[arrSize];
            list.ToArray(arr);

            CheckArray(list, arr);
            list.Dispose();
        }
        [TestMethod]
        public void AddRangeWithArray()
        {
            var list = new ValueList<int>();

            var arr = Enumerable.Range(0, 100).ToArray();

            list.Add(arr);
            CheckArray(list, arr);

            list.Dispose();

            list = new ValueList<int>();

            list.Add(arr,50);
            CheckArray(list, arr.Skip(50).ToArray());

            list.Dispose();

            list = new ValueList<int>();

            list.Add(arr, 20,50);
            CheckArray(list, arr.Skip(20).Take(50).ToArray());
            list.Dispose();
        }
#if NET5_0_OR_GREATER

        [TestMethod]
        public void AddRangeWithList()
        {
            var list = new ValueList<int>();

            var lst = new List<int> { 1, 2, 3, 4 };

            list.AddRange(lst);
            CheckArray(list, lst.ToArray());
            list.Dispose();
        }
#endif
        [TestMethod]
        public void GetSetIndex()
        {
            var list = new ValueList<int>(1);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);

            list[0] = 4;
            list[1] = 5;
            list[2] = 6;
            list[3] = 7;

            Assert.AreEqual(4, list[0]);
            Assert.AreEqual(5, list[1]);
            Assert.AreEqual(6, list[2]);
            Assert.AreEqual(7, list[3]);

            list.Dispose();
        }
        [TestMethod]
        public void RemoveWithSwith()
        {
            var list = new ValueList<int>(1);
            for (int i = 0; i < 7000; i++)
            {
                list.Add(i);
            }
            while (list.Size > 0)
            {
                list.RemoveLast();
                var arr = list.ToArray();
                CheckArray(list, arr);
            }
        }
        [TestMethod]
        [DataRow(0)]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(162)]
        [DataRow(654)]
        public void AddRepeat(int repeat)
        {
            var list = new ValueList<int>(1);
            list.Add(1, repeat);
            Assert.AreEqual(repeat, list.Size);
            var arr = list.ToArray();
            CheckArray(list, arr);
        }
        private void CheckArray<T>(in ValueList<T> list,T[] arr)
        {
            var enu = list.GetEnumerator();
            var x = 0;
            while (enu.MoveNext())
            {
                var val = arr[x];
                Assert.AreEqual(enu.Current,val, "The " + x);
                x++;
            }
        }
    }
}
