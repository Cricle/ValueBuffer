using System;
using System.Runtime.CompilerServices;

namespace ValueBuffer
{
    internal static class MagicConst
    {
        public const int DefaultSize = 32768;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BufferCheck(ref int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException("sizeHint", "size must be greater than 0");
            }
            if (sizeHint == 0)
            {
                sizeHint = DefaultSize;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BufferCheckMin(ref int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException("sizeHint", "size must be greater than 0");
            }
            if (sizeHint == 0)
            {
                sizeHint = 1;
                sizeHint = (int)BitOperations.RoundUpToPowerOf2((uint)sizeHint);
            }
        }
    }
}
