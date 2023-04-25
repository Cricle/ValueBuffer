using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ValueBuffer
{
    public class ValueBufferTextWriter : TextWriter
    {
        private ValueList<char> chars;

        public ValueBufferTextWriter()
            : this(Encoding.UTF8)
        {
        }
        public ValueBufferTextWriter(Encoding encoding)
        {
            Encoding = encoding;
        }

        public ValueBufferTextWriter(in ValueList<char> chars, Encoding encoding)
        {
            this.chars = chars;
            Encoding = encoding;
        }

        public override Encoding Encoding { get; }

        public override void Write(char[] buffer)
        {
            chars.Add(buffer);
        }
        public override void Write(char[] buffer, int index, int count)
        {
            chars.Add(buffer, index, count);
        }
        public override string ToString()
        {
            var cs = ArrayPool<char>.Shared.Rent(chars.Size);
            try
            {
                chars.ToArray(cs);
                return new string(cs, 0, chars.Size);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(cs);
            }
        }
        public override void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            chars.Dispose();
            base.Dispose(disposing);
        }
    }
}
