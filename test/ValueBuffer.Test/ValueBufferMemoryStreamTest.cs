using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            using (var mem=new ValueBufferMemoryStream())
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
                var buffer=new byte[40000];
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
    }
}
