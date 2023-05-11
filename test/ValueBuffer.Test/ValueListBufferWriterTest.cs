using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public class ValueListBufferWriterTest
    {
        [TestMethod]
        public void WriteWithBuffer()
        {
            using (var writer=new ValueListBufferWriter<int>())
            {
                var buffer=Enumerable.Range(0, 1_000_000).ToArray();
                writer.Write(buffer);
                var lst = writer.List;
                Assert.AreEqual(buffer.Length, lst.Size);
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
        public void WriteWithoutBuffer()
        {
            using (var writer = new ValueListBufferWriter<int>())
            {
                writer.Write(new int[] { 0 });
                writer.Write(new int[] { 1,2,3,4,5 });
                var lst = writer.List;
                Assert.AreEqual(6, lst.Size);
                var enu = ValueList<int>.GetEnumerator(lst);
                var q = 0;
                while (enu.MoveNext())
                {
                    Assert.AreEqual(q, enu.Current);
                    q++;
                }
            }
        }
    }
}
