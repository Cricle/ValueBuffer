using System;
using System.Buffers;
using System.Runtime.CompilerServices;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
