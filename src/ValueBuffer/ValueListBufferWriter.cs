using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ValueBuffer
{
    public class ValueListBufferWriter<T> : IBufferWriter<T>, IDisposable
    {
        private readonly ArrayPool<T> pool;
        private T[] currentBuffer;

        private ValueList<T> list;

        public ref ValueList<T> List => ref list;

        public ValueListBufferWriter(int? capacity = null)
            : this(ArrayPool<T>.Shared, capacity)
        {

        }
        public ValueListBufferWriter(ArrayPool<T> pool, int? capacity = null)
        {
            this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
            currentBuffer = null;
            if (capacity == null)
            {
                list = new ValueList<T>();
            }
            else
            {
                list = new ValueList<T>(capacity.Value);
            }

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            if (isFromList)
            {
                list.DangerousAdvance(count);
            }
            else
            {
                list.Add(currentBuffer, 0, count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (currentBuffer != null)
            {
                pool.Return(currentBuffer);
                currentBuffer = null;
            }
            list.Dispose();
        }
        private bool isFromList;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            isFromList = false;
            MagicConst.BufferCheckMin(ref sizeHint);
            if (list.DangerousCanGetMemory(ref sizeHint))
            {
                isFromList = true;
                return list.VeryDangerousGetMemory(sizeHint);
            }
            if (currentBuffer == null || (uint)sizeHint > (uint)currentBuffer.Length)
            {
                Resize(sizeHint);
            }
            return new Memory<T>(currentBuffer, 0, sizeHint);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> GetSpan(int sizeHint = 0)
        {
            return GetMemory(sizeHint).Span;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resize(int sizeHint)
        {
            var nar = pool.Rent((int)Math.Max(BitOperations.RoundUpToPowerOf2((uint)sizeHint), MagicConst.DefaultSize));
            if (currentBuffer != null)
            {
                pool.Return(currentBuffer);
            }
            currentBuffer = nar;
        }
    }
}
