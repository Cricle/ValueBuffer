using System;
using System.IO;
using System.Runtime.CompilerServices;

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
        
        public ValueStringBuilder ToStringBuilder()
        {            
            return new ValueStringBuilder(Unsafe.As<ValueList<byte>, ValueList<char>>(ref buffer));
        }

        protected override void Dispose(bool disposing)
        {
            buffer.Dispose();
        }
    }
}
