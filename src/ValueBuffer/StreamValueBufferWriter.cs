using System.Buffers;
using System.IO;

namespace ValueBuffer
{
    public class StreamValueBufferWriter : ValueBufferWriter<byte>
    {
        public Stream Stream { get; }

        public StreamValueBufferWriter(Stream stream)
        {
            Stream = stream;
        }

        public StreamValueBufferWriter(int preallocateSize, Stream stream)
            : base(preallocateSize)
        {
            Stream = stream;

        }

        public StreamValueBufferWriter(ArrayPool<byte> pool, Stream stream) : base(pool)
        {
            Stream = stream;

        }

        public StreamValueBufferWriter(ArrayPool<byte> pool, int preallocateSize, Stream stream) : base(pool, preallocateSize)
        {
            Stream = stream;
        }
        public override void Advance(int count)
        {
            base.Advance(count);
#if NETSTANDARD2_0
            Stream.Write(CurrentBuffer,0, count);
#else
            Stream.Write(GetSpan());
#endif
            Reset();
        }
    }
}
