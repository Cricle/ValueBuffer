using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ValueBuffer
{
    public static class EncodingHelper
    {
        private readonly static ArrayPool<byte> pool = ArrayPool<byte>.Shared;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EncodingResult SharedEncoding(string str)
        {
            return SharedEncoding(str, Encoding.UTF8, 0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EncodingResult SharedEncoding(string str, Encoding encoding)
        {
            return SharedEncoding(str, encoding, 0);
        }
        public unsafe static EncodingResult SharedEncoding(string str, Encoding encoding, int startIndex)
        {
            var strLen = str.Length;
            var cs = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(str.AsSpan(startIndex)));
            var byteCount = encoding.GetByteCount(cs, strLen);
            var bytes = pool.Rent(byteCount);
            var bytesReceived = encoding.GetBytes(cs, strLen, (byte*)Unsafe.AsPointer(ref bytes[0]), byteCount);
            Debug.Assert(bytesReceived == byteCount);
            return new EncodingResult(bytes, bytesReceived);
        }
    }
}
