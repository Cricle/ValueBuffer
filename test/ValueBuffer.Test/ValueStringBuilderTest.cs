﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer.Test
{
    [TestClass]
    public class ValueStringBuilderTest
    {
        [TestMethod]
        public unsafe void Append()
        {
            var b = new ValueStringBuilder();
            b.Append('h');
            b.Append("e");
            b.Append('l',2);
            b.Append("o ".AsSpan());
            
            fixed (char* ptr= "wo")
            {
                b.Append(ptr, 2);
            }
            using (var mem1=new MemoryStream())
            using (var mem2 = new MemoryStream())
            using (var mem3 = new MemoryStream())
            {
                var sw1 = new StreamWriter(mem1);
                sw1.Write("r");
                sw1.Flush();
                mem1.Seek(0, SeekOrigin.Begin);
                b.Append(new StreamReader(mem1), 1024);

                var sw2 = new StreamWriter(mem2);
                sw2.Write("l");
                sw2.Flush();
                mem2.Seek(0, SeekOrigin.Begin);
                b.Append(new StreamReader(mem2),new char[10]);

                var sw3 = new StreamWriter(mem3);
                sw3.Write("d!");
                sw3.Flush();
                mem3.Seek(0, SeekOrigin.Begin);
                b.Append(new StreamReader(mem3));
            }
            var str = b.ToString();
            Assert.AreEqual("hello world!", str);
            b.Dispose();
        }
        [TestMethod]
        public void AppendFormat()
        {
            var b = new ValueStringBuilder();
            b.AppendFormat("h{0}l{1}o{2}w{3}r{4}d!", "e", "l", " ", "o", "l");
            Assert.AreEqual("hello world!", b.ToString());
            b.Dispose();
        }
        [TestMethod]
        public void AppendFormats()
        {
            var b = new ValueStringBuilder();
            b.AppendFormat("a{0}b", "c");
            Assert.AreEqual("acb", b.ToString());
            b.Dispose();

            b = new ValueStringBuilder();
            b.AppendFormat("a{0}b{1}", "c","d");
            Assert.AreEqual("acbd", b.ToString());
            b.Dispose();

            b = new ValueStringBuilder();
            b.AppendFormat("a{0}b{1}e{2}", "c", "d", "f");
            Assert.AreEqual("acbdef", b.ToString());
            b.Dispose();

            b = new ValueStringBuilder();
            b.AppendFormat("a{0}b{1}e{2}g{3}", "c", "d", "f","h");
            Assert.AreEqual("acbdefgh", b.ToString());
            b.Dispose();
        }
        [TestMethod]
        public void AppendFormatTime()
        {
            var b = new ValueStringBuilder();
            var time = new DateTime(2021, 11, 16);
            b.AppendFormat("{0:yyyy-MM-dd HH:mm:ss}", time);
            Assert.AreEqual(time.ToString("yyyy-MM-dd HH:mm:ss"), b.ToString());
            b.Dispose();
        }
        [TestMethod]
        public void AppendFormatCap()
        {
            var b = new ValueStringBuilder(1024);
            b.AppendFormat("{0}", "123.456789");
            Assert.AreEqual("123.456789", b.ToString());
            b.Dispose();
        }
        private string GetRandomString(int size)
        {
            var rand = new Random();
            var str = "0123456789QWERTYUIOPASDFGHJKLZXCVBNM,./;'[]=-)(*&^%#$@!qwertyuiopasdfghjklzxcvbnm<>?:{ }|\\，。、；’【】、《》？：“";
            return new string(Enumerable.Range(0, size).Select(x => str[rand.Next(0, str.Length)]).ToArray());
        }
        [TestMethod]
        public void AppendFormatLarge()
        {
            var rand = new Random();
            for (int j = 0; j < 10; j++)
            {
                var b = new ValueStringBuilder(1);
                var s = string.Empty;
                for (int i = 0; i < 1024; i++)
                {
                    var str = GetRandomString(rand.Next(1000, 2000));
                    s += str;
                    b.Append(str);
                }
                var target = b.ToString();
                Assert.AreEqual(target.Length, s.Length);
                for (int i = 0; i < target.Length; i++)
                {
                    Assert.AreEqual(target[i], s[i], i.ToString());
                }
                b.Dispose();
            }
        }
        [TestMethod]
        public void GivenNullAppend_MustThrowExceotion()
        {
            var b = new ValueStringBuilder(1024);
            ArgumentNullException ex = null;
            try
            {
                b.AppendFormat("", (object[])null);
            }
            catch(ArgumentNullException e)
            {
                ex = e;
            }
            finally
            {
                b.Dispose();
            }
            Assert.IsNotNull(ex);
        }
        [TestMethod]
        public void AppendDateTime()
        {
            using (var b = new ValueStringBuilder(1024))
            {
                var time = DateTime.Parse("2023-01-01 11:22:33");
                b.AppendDateTime(time);
                var exp = time.ToString("yyyy-MM-dd HH:mm:ss");
                Assert.AreEqual(exp, b.ToString());
            }
        }
        [TestMethod]
        public void AppendTime()
        {
            using (var b = new ValueStringBuilder(1024))
            {
                var time = DateTime.Parse("2023-01-01 11:22:33");
                b.AppendTime(time);
                var exp = time.ToString("HH:mm:ss");
                Assert.AreEqual(exp, b.ToString());
            }
        }
        [TestMethod]
        public void AppendDate()
        {
            using (var b = new ValueStringBuilder(1024))
            {
                var time = DateTime.Parse("2023-01-01 11:22:33");
                b.AppendDate(time);
                var exp = time.ToString("yyyy-MM-dd");
                Assert.AreEqual(exp, b.ToString());
            }
        }
    }
}
