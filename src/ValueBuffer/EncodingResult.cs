using System;
using System.Buffers;

namespace ValueBuffer
{
    public readonly struct EncodingResult : IDisposable
    {
        public readonly byte[] Buffers;

        public readonly int Count;

        public Span<byte> Span => Buffers.AsSpan(0, Count);

        public Memory<byte> Memory => Buffers.AsMemory(0, Count);

        internal EncodingResult(byte[] buffers, int count)
        {
            Buffers = buffers;
            Count = count;
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffers);
        }
        public override string ToString()
        {
            return $"{{Count: {Count}}}";
        }
    }
}
