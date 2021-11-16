// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ValueBuffer
{
    /// <summary>
    /// See like https://source.dot.net/#System.Text.RegularExpressions/ValueListBuilder.cs,8d0a83e8be16c39f
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial struct ValueList<T> : IDisposable
    {
        private static readonly ArrayPool<T> pool = ArrayPool<T>.Shared;
        private static readonly ArrayPool<T[]> poolTs = ArrayPool<T[]>.Shared;

        private bool? isValueType;
        private readonly int baseCapacity;
        internal T[][] bufferSlots;
        private T[] localBuffer;
        private int bufferSlotIndex;
        private int size;
        private int totalCapacity;

        private int localUsed;
        private int localCount;

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
            isValueType =null;
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
        public T[] ToArray()
        {
#if NET5_0_OR_GREATER
            var arr = GC.AllocateUninitializedArray<T>(size);
#else
            var arr = new T[size];
#endif
            ToArray(arr);
            return arr;
        }
        public void ToArray(T[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length < size)
            {
                throw new ArgumentException();
            }
            var point = 0;
            for (int i = 0; i < bufferSlotIndex - 1; i++)
            {
                var current = bufferSlots[i];
                Array.Copy(current, 0, buffer, point, current.Length);
                point += current.Length;
            }
            if (bufferSlotIndex != 0)
            {
                Array.Copy(localBuffer, 0, buffer, point, localUsed);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int min)
        {
            if (bufferSlots == null)
            {
                bufferSlots = poolTs.Rent(4);
            }
            Debug.Assert(bufferSlots != null);
            if ((uint)bufferSlots.Length <= (uint)bufferSlotIndex)
            {
                var newBufferSize = bufferSlots.Length * 2;
                var newBuffers = poolTs.Rent(newBufferSize);

                var old = bufferSlots;

                bufferSlots = newBuffers;
                Buffer.BlockCopy(old, 0, newBuffers, 0, old.Length);
                poolTs.Return(old);
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
                poolTs.Return(bufferSlots);
            }
            for (int i = 0; i < bufferSlotIndex; i++)
            {
                pool.Return(bufferSlots[i]);
            }
            this = default;
        }

    }

}