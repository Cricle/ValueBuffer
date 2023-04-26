using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ValueBuffer
{
    public class ValueBufferWriter<T> : IBufferWriter<T>,IDisposable
    {
        private readonly ArrayPool<T> pool;
        private T[] currentBuffer;
        private int position;
        private int length;
        public const int DefaultSize = 32768;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Resize(int sizeHint)
        {
            var nar = pool.Rent(sizeHint);
            if (currentBuffer != null)
            {
                Array.Copy(currentBuffer, 0, nar, 0, currentBuffer.Length < nar.Length ? currentBuffer.Length : nar.Length);
                pool.Return(currentBuffer);
            }
            currentBuffer = nar;
        }
        public ValueBufferWriter() : this(ArrayPool<T>.Shared)
        {

        }
        public ValueBufferWriter(int preallocateSize) : this(ArrayPool<T>.Shared, preallocateSize)
        {

        }
        public ValueBufferWriter(ArrayPool<T> pool) : this(pool, DefaultSize)
        {
        }
        public ValueBufferWriter(ArrayPool<T> pool, int preallocateSize)
        {
            if (preallocateSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(preallocateSize), "size must be greater than 0");
            }
            this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
            currentBuffer = null;
            position = 0;
            length = 0;
            Resize(preallocateSize);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Advance(int count)
        {
            if (position + count > currentBuffer.Length)
            {
                throw new ArgumentOutOfRangeException("advance too many(" + count.ToString() + ")");
            }
            position += count;
            if (length < position)
            {
                length = position;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (currentBuffer != null)
            {
                pool.Return(currentBuffer);
                currentBuffer = null;
                position = 0;
                length = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException("sizeHint", "size must be greater than 0");
            }
            if (sizeHint == 0)
            {
                sizeHint = DefaultSize;
            }
            if (position + sizeHint > currentBuffer.Length)
            {
                Resize(position + sizeHint);
            }
            return currentBuffer.AsMemory(position, sizeHint);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> GetSpan(int sizeHint = 0)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException("sizeHint", "size must be greater than 0");
            }
            if (sizeHint == 0)
            {
                sizeHint = DefaultSize;
            }
            if (position + sizeHint > currentBuffer.Length)
            {
                Resize(position + sizeHint);
            }
            return currentBuffer.AsSpan(position, sizeHint);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> GetSpan()
        {
            return currentBuffer.AsSpan(0, length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<T> GetMemory()
        {
            return currentBuffer.AsMemory(0, length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset(int preallocateSize)
        {
            if (preallocateSize < 0)
            {
                throw new ArgumentOutOfRangeException("preallocateSize", "size must be greater than 0");
            }
            pool.Return(currentBuffer);
            currentBuffer = pool.Rent(preallocateSize);
            Reset();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            length = 0;
            position = 0;
        }
    }
}
