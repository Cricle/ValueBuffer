using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers;

namespace ValueBuffer.Test
{
    [TestClass]
    public class ValueBufferWriterTest
    {
        [TestMethod]
        public void AdvanceOutOfRange()
        {
            using (var writer=new ValueBufferWriter<byte>())
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Advance(int.MaxValue));
            }
        }
        [TestMethod]
        public void GetAndWrite()
        {
            using (var writer = new ValueBufferWriter<byte>())
            {
                writer.Write(new byte[] { 1, 2, 3 });
                var sp = writer.GetSpan();
                Assert.AreEqual(3, sp.Length);
                Assert.AreEqual(1, sp[0]);
                Assert.AreEqual(2, sp[1]);
                Assert.AreEqual(3, sp[2]);
            }
        }
        [TestMethod]
        public void GetWriteOutOfBuffer()
        {
            using (var writer = new ValueBufferWriter<int>())
            {
                writer.Write(new int[] { 0});
                for (int i = 1; i < 1_000_001; i++)
                {
                    writer.Write(new int[] { i });
                }
                var sp = writer.GetSpan();
                Assert.AreEqual(1_000_001, sp.Length);
                for (int i = 0; i < sp.Length; i++)
                {
                    Assert.AreEqual(i, sp[i]);
                }
            }
        }
    }
}
