using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ValueBuffer
{
    public class ValueBufferMemoryStream : Stream
    {
        private long position;

        private ValueList<byte> buffer;

        public ref ValueList<byte> Buffer =>ref  buffer;

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

        public bool IsEOF => position >= buffer.Size;

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
#if NET5_0_OR_GREATER
        public override int Read(Span<byte> buffer)
        {
            var count = Math.Min(buffer.Length, this.buffer.Size - position);
            if (count <= 0)
            {
                return 0;
            }
            this.buffer.ToArray(buffer, (int)position, (int)count);
            position+= count;
            return (int)count;
        }
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return ValueTask.FromResult(Read(buffer.Span));
        }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            this.buffer.Write(buffer, (int)position, buffer.Length);
            position += buffer.Length;
        }
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            this.buffer.Write(buffer.Span, (int)position, buffer.Length);
            position += buffer.Length;
            return default;
        }
#endif
        public override void WriteByte(byte value)
        {
            if (IsEOF)
            {
                this.buffer.Add(value);
            }
            else
            {
                buffer.GetSlot(buffer.BufferSlotIndex - 1)[buffer.LocalUsed - 1] = value;
            }
            position++;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(ReadOnlySpan<byte> bytes)
        {
            this.buffer.Add(bytes);
            position += bytes.Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(byte[] buffer, int offset, int count)
        {
            this.buffer.Add(buffer.AsSpan(offset, count));
            position += count;
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
            return buffer.AsString();
        }
        public string ToString(Encoding encoding)
        {
            return buffer.AsString(encoding);
        }
        public new void CopyTo(Stream destination)
        {
            for (int i = 0; i < buffer.BufferSlotIndex; i++)
            {
                var arr = buffer.DangerousGetArray(i);
                if (i != buffer.BufferSlotIndex - 1)
                {
                    destination.Write(arr, 0, arr.Length);
                }
                else
                {
                    destination.Write(arr, 0, buffer.LocalUsed);
                }
            }
        }
        public
#if !NETSTANDARD2_0
            override 
#else 
            new
#endif
            void CopyTo(Stream destination, int bufferSize)
        {
            CopyTo(destination);
        }
        public new Task CopyToAsync(Stream destination)
        {
            CopyTo(destination);
            return Task.CompletedTask;
        }
        public new Task CopyToAsync(Stream destination, int bufferSize)
        {
            CopyTo(destination);
            return Task.CompletedTask;
        }
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CopyTo(destination, bufferSize);
            return Task.CompletedTask;
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
