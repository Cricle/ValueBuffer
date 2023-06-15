using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
            var enu = ValueList<int>.GetEnumerator(list);
            var x = 0;
            while (enu.MoveNext())
            {
                Assert.AreEqual(x++, enu.Current);
            }
            var enuSlot = ValueList<int>.GetSlotEnumerator(list);
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
        class Index
        {
            public int A { get; set; }
        }
        [TestMethod]
        public void AddOne()
        {
            var list = new ValueList<Index>(32768);
            var obj = new Index { A = 123 };
            list.Add(obj);
            var arr = list.ToArray();
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(obj, arr[0]);
            list.Dispose();
        }
        [TestMethod]
        public void ToArrayOutOfRange()
        {
            var list = new ValueList<Index>(1);
            for (int i = 0; i < 100; i++)
            {
                list.Add(new Index { A = i });
            }
            var lst = new Index[10];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.ToArray(lst));
            list.Dispose();
        }
        [TestMethod]
        public void AddOutOfFirstBlock_ToArray()
        {
            var list = new ValueList<Index>(1);
            for (int i = 0; i < 400000; i++)
            {
                list.Add(new Index { A = i });
            }
            var arr = list.ToArray();
            Assert.AreEqual(400000, arr.Length);
            for (int i = 0; i < 400000; i++)
            {
                Assert.AreEqual(list[i], arr[i],"The "+i);
            }
            list.Dispose();
        }
        [TestMethod]
        [DataRow(0, 100)]
        [DataRow(100, 100)]
        [DataRow(100, 1000)]
        [DataRow(999, 1000)]
        [DataRow(50, 10)]
        public void AddWhenGetObject(int cap, int count)
        {
            var list = new ValueList<Index>(cap);
            for (int i = 0; i < count; i++)
            {
                list.Add(new Index { A=i});
            }
            Assert.AreEqual(count, list.Size);
            var enu = ValueList<Index>.GetEnumerator(list);
            var x = 0;
            while (enu.MoveNext())
            {
                var a = list[x];
                var b = enu.Current;
                Assert.AreEqual(a,b);
                x++;
            }
            var enuSlot = ValueList<Index>.GetSlotEnumerator(list);
            x = 0;
            while (enuSlot.MoveNext())
            {
                for (int j = 0; j < enuSlot.Current.Length; j++)
                {
                    Assert.AreEqual(list[x++], enuSlot.Current[j]);
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
            var enu = ValueList<T>.GetEnumerator(list);
            var x = 0;
            while (enu.MoveNext())
            {
                var val = arr[x];
                Assert.AreEqual(enu.Current,val, "The " + x);
                x++;
            }
        }
        [TestMethod]
        public void EnumerableOutOfSlot()
        {
            using (var lst = new ValueList<int>())
            {
                var count = 7876543;
                for (int i = 0; i < count; i++)
                {
                    lst.Add(i);
                }
                var slot = ValueList<int>.GetEnumerator(lst);
                var j = 0;
                while (slot.MoveNext())
                {
                    Assert.AreEqual(j++, slot.Current, j.ToString());
                }
            }
        }
        [TestMethod]
        public void EnumerableByObject()
        {
            using (var lst = new ValueList<int>())
            {
                var count = 7876543;
                for (int i = 0; i < count; i++)
                {
                    lst.Add(i);
                }
                var slot = ValueList<int>.GetEnumerator(lst);
                var j = 0;
                while (slot.MoveNext())
                {
                    Assert.AreEqual(j++, slot.Current, j.ToString());
                }
            }
        }
        [TestMethod]
        public void NesJsonSerializer()
        {
            using (var lst = new ValueList<int>())
            {
                var count = 50;
                for (int i = 0; i < count; i++)
                {
                    lst.Add(i);
                }
                var str = JsonConvert.SerializeObject(lst);
                var back = JsonConvert.DeserializeObject<List<int>>(str);
                for (int i = 0; i < count; i++)
                {
                    Assert.AreEqual(i, back[i],i.ToString());
                }
            }
        }
        [TestMethod]
        public void TextJsonSerializer()
        {
            using (var lst = new ValueList<int>())
            {
                var count = 50;
                for (int i = 0; i < count; i++)
                {
                    lst.Add(i);
                }
                var str = System.Text.Json.JsonSerializer.Serialize(lst);
                var back = System.Text.Json.JsonSerializer.Deserialize<List<int>>(str);
                for (int i = 0; i < count; i++)
                {
                    Assert.AreEqual(i, back[i], i.ToString());
                }
            }
        }
        [TestMethod]
        public void Enumerable_Empty()
        {
            using (var vl=new ValueList<int>())
            {
                var enu = ((IEnumerable<int>)vl).GetEnumerator();
                Assert.IsFalse(enu.MoveNext());
            }
        }
        [TestMethod]
        public void Enumerable_Values()
        {
            using (var vl = new ValueList<int>())
            {
                vl.Add(1);
                var enu = ((IEnumerable<int>)vl).GetEnumerator();
                Assert.IsTrue(enu.MoveNext());
                Assert.AreEqual(1, enu.Current);
                Assert.IsFalse(enu.MoveNext());
            }
        }
        [TestMethod]
        public void Enumerable_Values_Cross_Blocks()
        {
            using (var vl = new ValueList<int>(1))
            {
                for (int i = 0; i < 789787; i++)
                {
                    vl.Add(i);
                }
                var enu = ((IEnumerable<int>)vl).GetEnumerator();
                for (int i = 0; i < 789787; i++)
                {
                    Assert.IsTrue(enu.MoveNext(),i.ToString());
                    Assert.AreEqual(i, enu.Current, i.ToString());
                }
                Assert.IsFalse(enu.MoveNext());
            }
        }
        [TestMethod]
        public void SlotEnumerable()
        {
            using (var lst = new ValueList<int>(1))
            {
                var count = 500000;
                for (int i = 0; i < count; i++)
                {
                    lst.Add(i);
                }
                var enu = ValueList<int>.GetSlotEnumerator(lst);
                var j = 0;
                while (enu.MoveNext())
                {
                    for (int q = 0; q < enu.Current.Length; q++)
                    {
                        Assert.IsTrue(j < count);
                        Assert.AreEqual(j++, enu.Current[q]);
                    }
                }
            }
        }
        [TestMethod]
        public void GetSlotEmpty()
        {
            using (var lst = new ValueList<int>())
            {
                var s = lst.GetSlot(0);
                Assert.AreEqual(0, s.Length);
            }
        }
        [TestMethod]
        public void GetSlot()
        {
            using (var lst = new ValueList<int>(1))
            {
                var count = 500000;
                for (int i = 0; i < count; i++)
                {
                    lst.Add(i);
                }
                var j = 0;
                for (int i = 0; i < lst.BufferSlotIndex; i++)
                {
                    var s = lst.GetSlot(i);
                    for (int q = 0; q < s.Length; q++)
                    {
                        Assert.IsTrue(j < count);
                        Assert.AreEqual(j++, s[q]);
                    }
                }
            }
        }
        [TestMethod]
        public void ToArrayOffset()
        {
            using (var lst = new ValueList<int>())
            {
                for (int i = 0; i < 100000; i++)
                {
                    lst.Add(i);
                }
                var arr = new int[10];
                lst.ToArray(arr, 90000, arr.Length);
                for (int i = 0; i < arr.Length; i++)
                {
                    Assert.AreEqual(i + 90000, arr[i],i.ToString());
                }
            }
        }
        [TestMethod]
        public void ToArrayCount()
        {
            using (var lst = new ValueList<int>(1))
            {
                for (int i = 0; i < 100000; i++)
                {
                    lst.Add(i);
                }
                var arr = new int[10];
                lst.ToArray(arr, 0, arr.Length);
                for (int i = 0; i < arr.Length; i++)
                {
                    Assert.AreEqual(i, arr[i], i.ToString());
                }
            }
        }
        [TestMethod]
        public void ToArrayOffsetCount()
        {
            using (var lst = new ValueList<int>(1))
            {
                for (int i = 0; i < 100000; i++)
                {
                    lst.Add(i);
                }
                var arr = new int[10];
                lst.ToArray(arr, 90000, arr.Length);
                for (int i = 0; i < arr.Length; i++)
                {
                    Assert.AreEqual(i+ 90000, arr[i], i.ToString());
                }
            }
        }
        [TestMethod]
        public void WriteExists()
        {
            using (var lst = new ValueList<int>())
            {
                for (int i = 0; i < 40*2; i++)
                {
                    lst.Add(1);
                }
                var bs = new int[20];
                for (int i = 0; i < bs.Length; i++)
                {
                    bs[i] = i;
                }
                lst.Write(bs, 20, bs.Length);
                for (int i = 0; i < 20; i++)
                {
                    Assert.AreEqual(1, lst[i]);
                }
                for (int i = 0; i < bs.Length; i++)
                {
                    Assert.AreEqual(i, lst[i+20]);
                }
            }
        }
        [TestMethod]
        public void WriteOut()
        {
            using (var lst = new ValueList<int>())
            {
                for (int i = 0; i < 1; i++)
                {
                    lst.Add(99);
                }
                var bs = new int[20];
                for (int i = 0; i < bs.Length; i++)
                {
                    bs[i] = i;
                }
                lst.Write(bs, 0, bs.Length);
                for (int i = 0; i < 20; i++)
                {
                    Assert.AreEqual(i, lst[i]);
                }
            }
        }
        [TestMethod]
        public void SetSize()
        {
            using (var lst = new ValueList<int>())
            {
                for (int i = 0; i < 100; i++)
                {
                    lst.Add(i);
                }
                lst.SetSize(50);
                Assert.AreEqual(50, lst.Size);
                for (int i = 0; i < 50; i++)
                {
                    Assert.AreEqual(i, lst[i],i.ToString());
                }
            }
        }
        [TestMethod]
        public void WriteToStream()
        {
            using (var lst = new ValueList<byte>())
            {
                var buffer = new byte[1024 * 5];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)(i % byte.MaxValue);
                }
                lst.Add(buffer);
                var mem = new MemoryStream();
                lst.WriteToStream(mem);
                mem.Position = 0;
                var memBuffer=mem.ToArray();
                Assert.AreEqual(memBuffer.Length, mem.Length);
                Assert.IsTrue(memBuffer.SequenceEqual(buffer));
            }
        }
        [TestMethod]
        public async Task WriteToStreamAsync()
        {
            using (var lst = new ValueList<byte>())
            {
                var buffer = new byte[1024 * 5];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)(i % byte.MaxValue);
                }
                lst.Add(buffer);
                var mem = new MemoryStream();
                await lst.WriteToStreamAsync(mem);
                mem.Position = 0;
                var memBuffer = mem.ToArray();
                Assert.AreEqual(memBuffer.Length, mem.Length);
                Assert.IsTrue(memBuffer.SequenceEqual(buffer));
            }
        }
        [TestMethod]
        public void DangerousGetArrayThrowIfIndexOutOfRange()
        {
            using (var lst = new ValueList<byte>())
            {
                lst.Add(1);
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => lst.DangerousGetArray(2));
            }
        }
        [TestMethod]
        public void AddHasAnyCapacity()
        {
            using (var lst = new ValueList<int>())
            {
                lst.Add(0);
                lst.Add(Enumerable.Range(1, 999).Select(x => x).ToArray());
                Assert.AreEqual(lst.Size, 1000);
                var enu = ValueList<int>.GetEnumerator(lst);
                var q = 0;
                while (enu.MoveNext())
                {
                    Assert.AreEqual(q, enu.Current);
                    q++;
                }
            }
        }
        [TestMethod]
        public void RemoveIfObject_WillCleanRef()
        {
            using (var lst = new ValueList<object>())
            {
                lst.Add(new object());
                lst.RemoveLast();
                Assert.AreEqual(lst.Size, 0);
            }
        }
        [TestMethod]
        public void CreateWithInput()
        {
            var pool1 = ArrayPool<int>.Create();
            var pool2 = ArrayPool<int[]>.Create();
            using (var lst = new ValueList<int>(100000, pool1, pool2))
            {
                lst.Add(1);
                Assert.IsTrue(lst.TotalCapacity > 100000);
                Assert.AreEqual(pool1, lst.Pool);
                Assert.AreEqual(pool2, lst.PoolArray);
            }
        }
        [TestMethod]
        public void WriteToStreamNotByteType_ThrowException()
        {
            using (var lst = new ValueList<int>())
            {
                Assert.ThrowsException<InvalidCastException>(() => lst.WriteToStream(Stream.Null));
            }
        }
        [TestMethod]
        public async Task WriteToStreamAsyncNotByteType_ThrowException()
        {
            using (var lst = new ValueList<int>())
            {
                await Assert.ThrowsExceptionAsync<InvalidCastException>(() => lst.WriteToStreamAsync(Stream.Null));
            }
        }
        [TestMethod]
        public void AddAnyRemoveAll()
        {
            using (var lst = new ValueList<int>())
            {
                var p = 10_000;
                for (int i = 0; i < p; i++)
                {
                    lst.Add(i);
                }
                while (lst.Size!=0)
                {
                    lst.RemoveLast();
                    p--;
                    var enu = ValueList<int>.GetEnumerator(lst);
                    var w = 0;
                    Assert.AreEqual(p, lst.Size);
                    while (enu.MoveNext())
                    {
                        Assert.AreEqual(w, enu.Current);
                        w++;
                    }
                }
            }
        }
        [TestMethod]
        public void ToList_Empty()
        {
            using (var vl=new ValueList<int>())
            {
                var lst = vl.ToList();
                Assert.AreEqual(0, lst.Count);
                Assert.AreEqual(0, lst.Capacity);
            }
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(100)]
        [DataRow(1_000_000)]
        public void ToList_AddAndToList(int count)
        {
            using (var vl = new ValueList<int>())
            {
                for (int i = 0; i < count; i++)
                {
                    vl.Add(i);
                }
                var lst = vl.ToList();
                for (int i = 0; i < count; i++)
                {
                    Assert.AreEqual(i, lst[i], i.ToString());
                }
            }
        }
    }
}
