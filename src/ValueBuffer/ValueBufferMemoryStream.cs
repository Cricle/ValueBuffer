using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ValueBuffer
{
    public class ValueBufferMemoryStream : Stream
    {
        private long position;

        private ValueList<byte> buffer;

        public ValueList<byte> Buffer => buffer;

        public ValueBufferMemoryStream(in ValueList<byte> buffer)
        {
            this.buffer = buffer;
        }

        public ValueBufferMemoryStream()
        {
            buffer = new ValueList<byte>();
        }
        public ValueBufferMemoryStream(int size)
        {
            buffer = new ValueList<byte>(size);
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => buffer.Size;

        public override long Position 
        {
            get => position;
            set
            {
                position = value;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var c = Math.Min(this.buffer.Size - offset - (int)position, count);
            if (c == 0)
            {
                return 0;
            }
            position += c;
            this.buffer.ToArray(buffer,offset,c);
            return c;
        }
        public int WriteString(string s)
        {
            return WriteString(s, Encoding.UTF8, 0);
        }
        public int WriteString(string s, Encoding encoding)
        {
            return WriteString(s, encoding, 0);
        }
        public int WriteString(string s,Encoding encoding,int startIndex)
        {
            using (var bs = EncodingHelper.SharedEncoding(s, encoding, startIndex))
            {
                buffer.Write(bs.Buffers, 0, bs.Count);
                return bs.Count;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = 0;
                    break;
                case SeekOrigin.Current:
                    position = Math.Min(buffer.Size, offset);
                    break;
                case SeekOrigin.End:
                    position = buffer.Size;
                    break;
                default:
                    break;
            }
            return position;
        }

        public override void SetLength(long value)
        {
            buffer.SetSize((int)value);
            position = Math.Min(position, buffer.Size);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.buffer.Write(buffer.AsSpan(offset, count), (int)position, count);
            position += count;
        }

        public byte[] ToArray()
        {
            return buffer.ToArray();
        }
        public override string ToString()
        {
            return ToString(Encoding.UTF8);
        }
        public string ToString(Encoding encoding)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(buffer.Size);
            buffer.ToArray(bytes);
            var charCount = encoding.GetCharCount(bytes, 0, buffer.Size);
            var charts = ArrayPool<char>.Shared.Rent(charCount);
            try
            {
                encoding.GetChars(bytes, 0, buffer.Size, charts, 0);
                return new string(charts, 0, charCount);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
                ArrayPool<char>.Shared.Return(charts);
            }
        }
        public ValueStringBuilder ToStringBuilder()
        {
            return ToStringBuilder(Encoding.UTF8);
        }
        public ValueStringBuilder ToStringBuilder(Encoding encoding)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(buffer.Size);
            buffer.ToArray(bytes);
            var charCount = encoding.GetCharCount(bytes, 0, buffer.Size);
            var charts = ArrayPool<char>.Shared.Rent(charCount);
            try
            {
                var bs = new ValueList<char>(charCount);
                encoding.GetChars(bytes, 0, buffer.Size, charts, 0);
                bs.Add(charts,0,charCount);
                return new ValueStringBuilder(bs);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes); 
                ArrayPool<char>.Shared.Return(charts);
            }

        }

        protected override void Dispose(bool disposing)
        {
            buffer.Dispose();
        }
    }
}
