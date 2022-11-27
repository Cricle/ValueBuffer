// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ValueBuffer
{
    /// <summary>
    /// See like https://source.dot.net/#System.Text.RegularExpressions/ValueListBuilder.cs,8d0a83e8be16c39f
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial struct ValueList<T> : IDisposable,IEnumerable<T>
    {
        private static readonly ArrayPool<T> defaultPool = ArrayPool<T>.Shared;
        private static readonly ArrayPool<T[]> defaultPoolTs = ArrayPool<T[]>.Shared;

        private bool? isValueType;
        private int baseCapacity;
        internal T[][] bufferSlots;
        private T[] localBuffer;
        private int bufferSlotIndex;
        private int size;
        private int totalCapacity;

        private int localUsed;
        private int localCount;
        private ArrayPool<T> pool;
        private ArrayPool<T[]> poolArray;

        public ArrayPool<T> Pool => pool;
        public ArrayPool<T[]> PoolArray => poolArray;

        public int Size 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return size;
            }
        }
        public int TotalCapacity => totalCapacity;

        public int LocalUsed => localUsed;
        public int LocalCount => localCount;

        public int BufferSlotIndex => bufferSlotIndex;
        public ValueList(int capacity,ArrayPool<T> pool,ArrayPool<T[]> poolTs)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Capacity must more than zero!");
            }
            baseCapacity = capacity;
            bufferSlots = null;
            localBuffer = null;
            bufferSlotIndex = 0;
            size = 0;
            totalCapacity = 0;
            localUsed = 0;
            localCount = 0;
            isValueType = null;
            this.pool = pool;
            this.poolArray = poolTs;
        }
        public ValueList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Capacity must more than zero!");
            }
            baseCapacity = capacity;
            bufferSlots = null;
            localBuffer = null;
            bufferSlotIndex = 0;
            size = 0;
            totalCapacity = 0;
            localUsed = 0;
            localCount = 0;
            isValueType = null;
            pool = defaultPool;
            poolArray = defaultPoolTs;
        }

        public T this[int index]
        {
            get
            {
                if (index > size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                GetIndex(index, out var block, out var idx);
                return block[idx];
            }
            set
            {
                if (index > size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                GetIndex(index, out var block, out var idx);
                block[idx] = value;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetIndex(int index, out T[] block, out int idx)
        {
            idx = index;
            var first = 0;
            block = bufferSlots[first];
            var len = block.Length;
            while (first < bufferSlotIndex && len <= idx)
            {
                idx -= len;
                first++;
                block = bufferSlots[first];
                len = block.Length;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if ((uint)totalCapacity < (uint)size + 1U)
            {
                Grow(1);
            }
            localBuffer[localUsed++] = item;
            size++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckIsValueType()
        {
            if (!isValueType.HasValue)
            {
                isValueType = typeof(T).IsValueType;
            }
            return isValueType.Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SwithPrevBuffer()
        {
            if (localUsed != 0 && !CheckIsValueType())
            {
                localBuffer.AsSpan().Fill(default);
            }
            if (bufferSlotIndex != 1)
            {
                pool.Return(localBuffer);
            }
            bufferSlots[bufferSlotIndex - 1] = null;
            var preCount = localUsed;
            var prev = bufferSlots[bufferSlotIndex - 2];
            localBuffer = prev;
            localUsed = prev.Length;
            localCount = localUsed;
            bufferSlotIndex--;
            return preCount;
        }

        public Span<T> GetSlot(int index)
        {
            if (bufferSlots==null)
            {
                return Span<T>.Empty;
            }
            if (index >= bufferSlotIndex)
            {
                throw new ArgumentOutOfRangeException($"index out of {bufferSlots.Length - 1}");
            }
            return index == bufferSlotIndex - 1 ? bufferSlots[index].AsSpan(0, localUsed) : bufferSlots[index];
        }

        public void RemoveLast(int length)
        {
            var olen = length;
            var notValueType = !CheckIsValueType();
            while (length > 0)
            {
                if ((uint)localUsed <= (uint)length)
                {
                    length -= SwithPrevBuffer();
                }
                else
                {
                    if (notValueType)
                    {
                        localBuffer.AsSpan(localUsed - length, length).Fill(default);
                    }
                    localUsed -= length;
                    length -= length;
                }
            }
            size -= olen;
        }
        public void RemoveLast()
        {
            if ((uint)localUsed == 0U)
            {
                SwithPrevBuffer();
            }
            if (!typeof(T).IsValueType)
            {
                localBuffer[localUsed] = default;
            }
            localUsed--;
            size--;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item,int count)
        {
            if (count == 0)
            {
                return;
            }
            if (count < 0)
            {
                throw new ArgumentException("count must more than 0");
            }
            int itemUsed = 0;
            if ((uint)totalCapacity <= (uint)size + (uint)count)
            {
                if (totalCapacity != 0)
                {
                    var avaliable = totalCapacity - size;
                    localBuffer
                        .AsSpan()
                        .Slice(localUsed, avaliable)
                        .Fill(item);
                    itemUsed = avaliable;
                    localUsed = localCount - 1;
                    Grow(count - avaliable);
                }
                else
                {
                    Grow(count);
                }
            }
            localBuffer
                .AsSpan()
                .Slice(localUsed, count - itemUsed)
                .Fill(item);
            localUsed += count - itemUsed;
            size += count;
        }
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(List<T> items)
        {
            var sp = CollectionsMarshal.AsSpan(items);
            Add(sp);
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T[] items, int start, int length)
        {
            Add(new ReadOnlySpan<T>(items, start, length));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T[] items, int start)
        {
            Add(new ReadOnlySpan<T>(items, start, items.Length - start));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T[] items)
        {
            Add(new ReadOnlySpan<T>(items, 0, items.Length));
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Add(in ReadOnlySpan<T> items)
        {
            int itemUsed = 0;
            if (totalCapacity <= size + items.Length)
            {
                if (totalCapacity != 0)
                {
                    var avaliable = totalCapacity - size;
                    items.Slice(0, avaliable)
                        .CopyTo(new Span<T>(localBuffer, localUsed, avaliable));
                    itemUsed = avaliable;
                    localUsed = localCount - 1;
                    Grow(items.Length - avaliable);
                }
                else
                {
                    Grow(items.Length);
                }
            }
            items.Slice(itemUsed)
                .CopyTo(localBuffer.AsSpan(localUsed));
            localUsed += items.Length - itemUsed;
            size += items.Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray(int offset, int count)
        {
#if NET5_0_OR_GREATER
            var arr = GC.AllocateUninitializedArray<T>(size);
#else
            var arr = new T[size];
#endif
            ToArray(arr, 0, size);
            return arr;
        }
        public void Write(in Span<T> buffer,int offset, int count)
        {
            var point = 0;
            var offsetSlot = SkipSlot(ref offset);
            for (; offsetSlot < bufferSlotIndex; offsetSlot++)
            {
                var current = bufferSlots[offsetSlot];
                var currentLength = offsetSlot == bufferSlotIndex - 1 ? localUsed : current.Length;
                var target = current.AsSpan(offset,currentLength - offset);
                var copySize = Math.Min(target.Length, count);
                buffer.Slice(point, copySize).CopyTo(target);
                offset = 0;
                count -= copySize;
                point += copySize;
                if (count == 0)
                {
                    return;
                }
            }
            Add(buffer.Slice(point, buffer.Length - point));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            return ToArray(0, size);
        }
        public void ToArray(T[] buffer)
        {
            if (buffer==null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            ToArray(buffer, 0, size);
        }
        public int SkipSlot(ref int offset)
        {
            var offsetSlot = 0;
            for (; offsetSlot < bufferSlotIndex; offsetSlot++)
            {
                var current = bufferSlots[offsetSlot];
                var currentLength = offsetSlot == bufferSlotIndex - 1 ? localUsed : current.Length;
                if (currentLength > offset)
                {
                    break;
                }
                offset -= current.Length;
            }
            return offsetSlot;
        }
        public void ToArray(in Span<T> buffer,int offset,int count)
        {
            if (buffer.IsEmpty)
            {
                return;
            }

            if (offset+count > size)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer));
            }
            var point = 0;
            var offsetSlot = SkipSlot(ref offset);
            for (; offsetSlot < bufferSlotIndex; offsetSlot++)
            {
                var current = bufferSlots[offsetSlot];
                var currentLength = offsetSlot == bufferSlotIndex - 1 ? localUsed : current.Length;
                var copySize=Math.Min(currentLength, count);
                current.AsSpan(offset, copySize).CopyTo(buffer.Slice(point,copySize));
                offset -= copySize;
                offset = Math.Max(0, offset);
                count -= copySize;
                point += copySize;
                if (count==0)
                {
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int min)
        {
            if (pool==null)
            {
                pool = defaultPool;
            }
            if (poolArray==null)
            {
                poolArray = defaultPoolTs;
            }
            if (bufferSlots == null)
            {
                bufferSlots = poolArray.Rent(32768);
            }
            Debug.Assert(bufferSlots != null);
            if ((uint)bufferSlots.Length <= (uint)bufferSlotIndex)
            {
                var newBufferSize = bufferSlots.Length * 2;
                var newBuffers = poolArray.Rent(newBufferSize);

                var old = bufferSlots;

                bufferSlots = newBuffers;
                Buffer.BlockCopy(old, 0, newBuffers, 0, old.Length);
                poolArray.Return(old);
            }
            Debug.Assert(bufferSlots.Length > bufferSlotIndex);
            uint allocSize;

            if (size == 0)
            {
                allocSize = Math.Max(32768U, (uint)min);
                if (bufferSlotIndex == 0)
                {
                    allocSize = Math.Max((uint)baseCapacity, allocSize);
                }
            }
            else
            {
                allocSize = Math.Max((uint)(size * 2), (uint)min);
            }
#if NET6_0_OR_GREATER
            if (allocSize > Array.MaxLength)
#else
            if(allocSize> 0x7FFFFFC7)
#endif
            {
                allocSize = 0x7FFFFFC7;
            }
            var localBuffer = pool.Rent((int)allocSize);

            var len = localBuffer.Length;

            totalCapacity += len;
            this.localBuffer = localBuffer;
            localCount = len;
            localUsed = 0;
            bufferSlots[bufferSlotIndex] = localBuffer;
            bufferSlotIndex++;
        }
        public void Dispose()
        {
            if (bufferSlots != null)
            {
                poolArray.Return(bufferSlots);
            }
            for (int i = 0; i < bufferSlotIndex; i++)
            {
                pool.Return(bufferSlots[i]);
            }
            this = default;
        }
        public void Clear()
        {
            Dispose();
        }
        public void SetSize(int size)
        {
            if (size > this.size)
            {
                throw new ArgumentOutOfRangeException(nameof(size),"must <= Size");
            }
            if (size == this.size)
            {
                return;
            }
            if (size == 0)
            {
                Clear();
            }
            else
            {
                RemoveLast(this.size - size);
            }
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(bufferSlots, size);
        }
        struct Enumerator: IEnumerator<T>
        {
            private readonly T[][] bufferSlots;
            private int slotIndex;
            private int index;
            private T[] slot;
            private readonly int size;
            private int current;
            private readonly bool alwayFalse;

            public Enumerator(in T[][] bufferSlots,int size)
            {
                this.bufferSlots = bufferSlots;
                index = -1;
                slotIndex = 0;
                this.size = size;
                current = 0;
                if (size == 0)
                {
                    slot = null;
                }
                else
                {
                    slot = bufferSlots[0];
                }
                alwayFalse = size == 0 || slot == null || bufferSlots.Length == 0;
            }

            public T Current => bufferSlots[slotIndex][index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                if (current >= size || alwayFalse)
                {
                    return false;
                }
                index++;
                if ((uint)index >= (uint)slot.Length)
                {
                    slotIndex++;
                    slot = bufferSlots[slotIndex];
                    index = 0;
                }
                current++;
                return current <= size;
            }

            public void Reset()
            {
                slot = null;
                index = -1;
                slotIndex = 0;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }

}