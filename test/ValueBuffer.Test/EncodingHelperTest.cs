using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public class EncodingHelperTest
    {
        [TestMethod]
        public void EncodingString()
        {
            var str = "hello world";
            using (var res=EncodingHelper.SharedEncoding(str))
            {
                var exp = Encoding.UTF8.GetBytes(str);
                Assert.AreEqual(res.Count, exp.Length);
                Assert.IsTrue(res.Span.SequenceEqual(exp));
                Assert.IsTrue(res.Memory.Span.SequenceEqual(exp));
                Assert.IsTrue(res.Buffers.AsSpan(0,res.Count).SequenceEqual(exp));
            }
        }
        [TestMethod]
        public void EncodingToStringEquals()
        {
            var str = "hello world";
            using (var res = EncodingHelper.SharedEncoding(str))
            using (var res1 = EncodingHelper.SharedEncoding(str))
            {
                Assert.AreEqual(res.ToString(), res1.ToString());
            }
        }
        [TestMethod]
        public void EncodingWithEncodingToStringEquals()
        {
            var str = "hello world";
            using (var res = EncodingHelper.SharedEncoding(str,Encoding.ASCII))
            {
                var exp = Encoding.ASCII.GetBytes(str);
                Assert.AreEqual(res.Count, exp.Length);
                Assert.IsTrue(res.Span.SequenceEqual(exp));
                Assert.IsTrue(res.Memory.Span.SequenceEqual(exp));
                Assert.IsTrue(res.Buffers.AsSpan(0, res.Count).SequenceEqual(exp));
            }
        }
    }
}
