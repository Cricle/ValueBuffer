// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ValueBuffer
{
    public partial struct ValueList<T>
    {
        public static ValueEnumerator GetEnumerator(in ValueList<T> lst)
        {
            return new ValueEnumerator(lst);
        }
        public static SlotEnumerator GetSlotEnumerator(in ValueList<T> lst)
        {
            return new SlotEnumerator(lst);
        }
        public ref struct SlotEnumerator
        {
            private Span<T[]>.Enumerator curentEnum;
            private int used;
            private int hasIndex;

            internal SlotEnumerator(in ValueList<T> list)
            {
                hasIndex = list.bufferSlotIndex;
                curentEnum = list.bufferSlots.AsSpan(0, hasIndex).GetEnumerator();
                used = list.localUsed;
            }
            public T[] CurrentArray
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    if (IsLast)
                    {
                        return Current.ToArray();
                    }
                    return curentEnum.Current;
                }
            }
            public bool IsLast
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return hasIndex == 0;
                }
            }
            public ReadOnlySpan<T> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    var arr = curentEnum.Current;
                    if (hasIndex == 0)
                    {
                        return new ReadOnlySpan<T>(arr, 0, used);
                    }
                    return new ReadOnlySpan<T>(arr, 0, arr.Length);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (curentEnum.MoveNext())
                {
                    hasIndex--;
                    return true;
                }
                return false;
            }
        }
        public ref struct ValueEnumerator
        {
            private readonly ValueList<T> list;
            private readonly Span<T[]> slotSpan;
            private int current;
            private bool hasValue;
            private Span<T>.Enumerator curentEnum;

            internal ValueEnumerator(in ValueList<T> list)
            {
                hasValue = list.bufferSlotIndex > 0;
                current = 0;
                if (hasValue)
                {
                    hasValue = true;
                    this.list = list;
                    slotSpan = list.bufferSlots.AsSpan();
                    var first = slotSpan[0];
                    int count;
                    if (list.bufferSlotIndex == 1)
                    {
                        count = list.localUsed;
                    }
                    else
                    {
                        count = first.Length;
                    }
                    curentEnum = first.AsSpan(0, count).GetEnumerator();
                }
                else
                {
                    Unsafe.SkipInit(out this.list);
                    curentEnum = Span<T>.Empty.GetEnumerator();
                    slotSpan = Span<T[]>.Empty;
                }
            }
            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return ref curentEnum.Current;
                }
            }

            public bool MoveNext()
            {
                if (!hasValue)
                {
                    return false;
                }
                if (curentEnum.MoveNext())
                {
                    return true;
                }
                if (list.bufferSlotIndex > current + 1)
                {
                    current++;
                    int count;
                    var now = slotSpan[current];
                    if (list.bufferSlotIndex == current + 1)
                    {
                        count = list.localUsed;
                    }
                    else
                    {
                        count = now.Length;
                    }
                    curentEnum = new Span<T>(now, 0, count).GetEnumerator();
                    return curentEnum.MoveNext();
                }
                return false;
            }
        }
        
    }
}
