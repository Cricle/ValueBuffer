﻿using System;

namespace ValueBuffer
{
    internal static class MagicConst
    {
        public const int DefaultSize = 32768;

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
    }
}
