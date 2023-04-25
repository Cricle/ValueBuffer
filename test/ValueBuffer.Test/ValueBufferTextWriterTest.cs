using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public class ValueBufferTextWriterTest
    {
        [TestMethod]
        public void AppendAndToString()
        {
            using (var v=new ValueBufferTextWriter())
            {
                v.Write("hello");
                v.Write("world");
                Assert.AreEqual("helloworld", v.ToString());
            }
        }
        [TestMethod]
        public void AppendFormatAndToString()
        {
            using (var v = new ValueBufferTextWriter())
            {
                v.Write("hello {0}",444);
                v.Write("world");
                Assert.AreEqual("hello 444world", v.ToString());
            }
        }
    }
}
