using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public class ValueBufferMemoryStreamTest
    {
        [TestMethod]
        public void WriteEmpty()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.Write(new byte[0], 0, 0);
                Assert.AreEqual(0, mem.Position);
                Assert.AreEqual(0, mem.Buffer.Size);
            }
        }
        [TestMethod]
        public void WriteMore()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                var count = 40;
                var buffer = new byte[40000];
                for (int i = 0; i < count; i++)
                {
                    mem.Write(buffer, 0, buffer.Length);
                }
                Assert.AreEqual(count * buffer.Length, mem.Position);
                Assert.AreEqual(count * buffer.Length, mem.Buffer.Size);
            }
        }
        [TestMethod]
        public void ToCharts()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                var buffer = Encoding.UTF8.GetBytes("hello worlds");
                mem.Write(buffer, 0, buffer.Length);
                var sb = mem.ToStringBuilder();
                var str = sb.ToString();
                Assert.AreEqual("hello worlds", str);
                sb.Dispose();
            }
        }
        [TestMethod]
        public void Strings()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.WriteString("hello!!!啊啊啊", Encoding.UTF8, 0);
                var sb = mem.ToStringBuilder();
                var str = sb.ToString();
                Assert.AreEqual("hello!!!啊啊啊", str);
                sb.Dispose();
            }
        }
        [TestMethod]
        public void ToStrings()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.WriteString("hello!!!啊啊啊", Encoding.UTF8, 0);
                var str = mem.ToString();
                Assert.AreEqual("hello!!!啊啊啊", str);
            }
        }
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CopyTo(bool async)
        {
            using (var mem = new ValueBufferMemoryStream())
            using (var mem1 = new MemoryStream())
            {
                var idx = 0;
                for (int i = 0; i < 50; i++)
                {
                    var buffer = new byte[10000];
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        buffer[j] = (byte)idx++;
                    }
                    mem.Write(buffer, 0, buffer.Length);
                }
                mem.Seek(0, SeekOrigin.Begin);
                if (async)
                {
                    await mem.CopyToAsync(mem1);
                }
                else
                {
                    mem.CopyTo(mem1);
                }
                Assert.AreEqual(50 * 10000, mem1.Length);
                mem1.Position = 0;
                var buffer1 = mem1.GetBuffer();
                for (int i = 0; i < mem.Buffer.Size; i++)
                {
                    Assert.AreEqual(mem.Buffer[i], buffer1[i], i.ToString());
                }
            }
        }
#if NET5_0_OR_GREATER
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ReadSpan(bool async)
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                var buffer = new byte[100];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i]= (byte)i;
                }
                if (async)
                {
                    await mem.WriteAsync(buffer);
                }
                else
                {
                    mem.Write(buffer);
                }
                mem.Position = 90;
                var sp = new byte[20];
                var read = async ? await mem.ReadAsync(sp) : mem.Read(sp);
                Assert.AreEqual(10, read);
                for (int i = 0; i < 10; i++)
                {
                    Assert.AreEqual(i + 90, sp[i]);
                }
                for (int i = 10; i < sp.Length; i++)
                {
                    Assert.AreEqual(0, sp[i]);
                }
            }
        }
#endif
        [TestMethod]
        public void IsEOF()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                Assert.IsTrue(mem.IsEOF);
                mem.WriteByte(1);
                Assert.IsTrue(mem.IsEOF);
                mem.Position = 0;
                Assert.IsFalse(mem.IsEOF);
            }
        }
        [TestMethod]
        public void WriteByteNotExists()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.WriteByte(1);
                Assert.AreEqual(1, mem.Position);
                Assert.AreEqual(1, mem.Buffer.Size);
                Assert.AreEqual(1, mem.Buffer[0]);
            }
        }
        [TestMethod]
        public void WriteByteExists()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.WriteByte(1);
                mem.Position = 0;
                mem.WriteByte(2);
                Assert.AreEqual(1, mem.Position);
                Assert.AreEqual(1, mem.Buffer.Size);
                Assert.AreEqual(2, mem.Buffer[0]);
            }
        }
        [TestMethod]
        public void CreateWithParamters()
        {
            using (var mem = new ValueBufferMemoryStream(1024))
            {
                Assert.IsTrue(mem.CanRead);
                Assert.IsTrue(mem.CanSeek);
                Assert.IsTrue(mem.CanWrite);
            }
            var pool=ArrayPool<byte>.Create();
            var lst = new ValueList<byte>(1024, pool, ArrayPool<byte[]>.Shared);
            using (var mem = new ValueBufferMemoryStream(lst))
            {
                Assert.AreEqual(pool, mem.Buffer.Pool);
            }
        }
        [TestMethod]
        public void LengthMustEqualsSize()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
                Assert.AreEqual(4, mem.Length);
                Assert.IsTrue(mem.IsEOF);
                mem.Seek(0, SeekOrigin.Begin);
                Assert.IsFalse(mem.IsEOF);
                mem.Flush();
            }
        }
        [TestMethod]
        public void Read()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                var datas = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                mem.Write(datas, 0, datas.Length);
                var read = new byte[3];
                mem.Position = 0;
                var res = mem.Read(read, 0, 3);
                Assert.AreEqual(3, res);
                Assert.AreEqual(1, read[0]);
                Assert.AreEqual(2, read[1]);
                Assert.AreEqual(3, read[2]);

                read = new byte[3];
                mem.Position = 3;
                res = mem.Read(read, 0, 3);
                Assert.AreEqual(3, res);
                Assert.AreEqual(4, read[0]);
                Assert.AreEqual(5, read[1]);
                Assert.AreEqual(6, read[2]);

                read = new byte[3];
                mem.Position = 6;
                var actual = datas.Skip(6).ToArray();
                res = mem.Read(read, 0, 3);
                Assert.AreEqual(2, res);
                Assert.AreEqual(actual[0], read[0]);
                Assert.AreEqual(actual[1], read[1]);

                mem.Position = 8;
                res = mem.Read(read, 0, 1);
                Assert.AreEqual(0, res);
            }
        }
        [TestMethod]
        public void Seek()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 0, 7);
                mem.Position = 0;
                mem.Seek(2, SeekOrigin.Current);
                Assert.AreEqual(2, mem.Position);

                mem.Seek(2, SeekOrigin.Current);
                Assert.AreEqual(4, mem.Position);

                mem.Seek(2, SeekOrigin.Current);
                Assert.AreEqual(6, mem.Position);

                mem.Seek(2, SeekOrigin.Current);
                Assert.AreEqual(7, mem.Position);

                mem.Seek(2, SeekOrigin.Current);
                Assert.AreEqual(7, mem.Position);


                mem.Position = 0;
                mem.Seek(0, SeekOrigin.End);
                Assert.AreEqual(7, mem.Position);
            }
        }
        [TestMethod]
        public void SetLength()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 0, 7);
                mem.SetLength(1);
                Assert.AreEqual(1, mem.Buffer.Size);
            }
        }
        [TestMethod]
        public void WriteString()
        {
            using (var mem = new ValueBufferMemoryStream())
            {
                mem.WriteString("hello");
                mem.WriteString("world",Encoding.UTF8);
                var str=mem.Buffer.AsString();
                Assert.AreEqual("helloworld", str);
            }
        }
    }
}
