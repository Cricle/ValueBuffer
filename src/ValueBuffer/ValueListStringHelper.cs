using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace ValueBuffer
{
    public static class ValueListStringHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsString(this in ValueList<byte> buffer)
        {
            return AsString(buffer,Encoding.UTF8);
        }
        public static string AsString(this in ValueList<byte> buffer,Encoding encoding)
        {
            if (buffer.Size==0)
            {
                return string.Empty;
            }
            var bytes = ArrayPool<byte>.Shared.Rent(buffer.Size);
            buffer.ToArray(bytes);
            var charCount = encoding.GetCharCount(bytes, 0, buffer.Size);
            var charts = ArrayPool<char>.Shared.Rent(charCount);
            try
            {
                encoding.GetChars(bytes, 0, buffer.Size, charts, 0);
                return new string(charts, 0, charCount);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
                ArrayPool<char>.Shared.Return(charts);
            }
        }
    }
}
