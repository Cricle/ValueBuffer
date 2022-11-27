// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ValueBuffer
{
    /// <summary>
    /// https://source.dot.net/#System.Text.RegularExpressions/ValueStringBuilder.cs,157e1a7ce4de87da
    /// </summary>
    public ref partial struct ValueStringBuilder
    {
        public void AppendFormat(string format, object arg0) => AppendFormatHelper(null, format, new ParamsArray(arg0));

        public void AppendFormat(string format, object arg0, object arg1) => AppendFormatHelper(null, format, new ParamsArray(arg0, arg1));

        public void AppendFormat(string format, object arg0, object arg1, object arg2) => AppendFormatHelper(null, format, new ParamsArray(arg0, arg1, arg2));

        public void AppendFormat(string format, params object[] args)
        {
            if (args == null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormatHelper.
                string paramName = (format == null) ? nameof(format) : nameof(args);
                throw new ArgumentNullException(paramName);
            }

            AppendFormatHelper(null, format, new ParamsArray(args));
        }

        public void AppendFormat(IFormatProvider provider, string format, object arg0) => AppendFormatHelper(provider, format, new ParamsArray(arg0));

        public void AppendFormat(IFormatProvider provider, string format, object arg0, object arg1) => AppendFormatHelper(provider, format, new ParamsArray(arg0, arg1));

        public void AppendFormat(IFormatProvider provider, string format, object arg0, object arg1, object arg2) => AppendFormatHelper(provider, format, new ParamsArray(arg0, arg1, arg2));

        public void AppendFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (args == null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormatHelper.
                string paramName = (format == null) ? nameof(format) : nameof(args);
                throw new ArgumentNullException(paramName);
            }

            AppendFormatHelper(provider, format, new ParamsArray(args));
        }


        // Copied from StringBuilder, can't be done via generic extension
        // as ValueStringBuilder is a ref struct and cannot be used in a generic.
        internal void AppendFormatHelper(IFormatProvider provider, string format, ParamsArray args)
        {
            // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
            const int IndexLimit = 1000000; // Note:            0 <= ArgIndex < IndexLimit
            const int WidthLimit = 1000000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            int pos = 0;
            int len = format.Length;
            char ch = '\0';
            ICustomFormatter cf = (ICustomFormatter)provider?.GetFormat(typeof(ICustomFormatter));

            while (true)
            {
                while (pos < len)
                {
                    ch = format[pos];

                    pos++;
                    // Is it a closing brace?
                    if (ch == '}')
                    {
                        // Check next character (if there is one) to see if it is escaped. eg }}
                        if (pos < len && format[pos] == '}')
                        {
                            pos++;
                        }
                        else
                        {
                            // Otherwise treat it as an error (Mismatched closing brace)
                            ThrowFormatError();
                        }
                    }
                    // Is it a opening brace?
                    else if (ch == '{')
                    {
                        // Check next character (if there is one) to see if it is escaped. eg {{
                        if (pos < len && format[pos] == '{')
                        {
                            pos++;
                        }
                        else
                        {
                            // Otherwise treat it as the opening brace of an Argument Hole.
                            pos--;
                            break;
                        }
                    }
                    // If it's neither then treat the character as just text.
                    Append(ch);
                }

                //
                // Start of parsing of Argument Hole.
                // Argument Hole ::= { Index (, WS* Alignment WS*)? (: Formatting)? }
                //
                if (pos == len)
                {
                    break;
                }

                //
                //  Start of parsing required Index parameter.
                //  Index ::= ('0'-'9')+ WS*
                //
                pos++;
                // If reached end of text then error (Unexpected end of text)
                // or character is not a digit then error (Unexpected Character)
                if (pos == len || (ch = format[pos]) < '0' || ch > '9') ThrowFormatError();
                int index = 0;
                do
                {
                    index = index * 10 + ch - '0';
                    pos++;
                    // If reached end of text then error (Unexpected end of text)
                    if (pos == len)
                    {
                        ThrowFormatError();
                    }
                    ch = format[pos];
                    // so long as character is digit and value of the index is less than 1000000 ( index limit )
                }
                while (ch >= '0' && ch <= '9' && index < IndexLimit);

                // If value of index is not within the range of the arguments passed in then error (Index out of range)
                if (index >= args.Length)
                {
                    throw new FormatException();
                }

                // Consume optional whitespace.
                while (pos < len && (ch = format[pos]) == ' ') pos++;
                // End of parsing index parameter.

                //
                //  Start of parsing of optional Alignment
                //  Alignment ::= comma WS* minus? ('0'-'9')+ WS*
                //
                bool leftJustify = false;
                int width = 0;
                // Is the character a comma, which indicates the start of alignment parameter.
                if (ch == ',')
                {
                    pos++;

                    // Consume Optional whitespace
                    while (pos < len && format[pos] == ' ') pos++;

                    // If reached the end of the text then error (Unexpected end of text)
                    if (pos == len)
                    {
                        ThrowFormatError();
                    }

                    // Is there a minus sign?
                    ch = format[pos];
                    if (ch == '-')
                    {
                        // Yes, then alignment is left justified.
                        leftJustify = true;
                        pos++;
                        // If reached end of text then error (Unexpected end of text)
                        if (pos == len)
                        {
                            ThrowFormatError();
                        }
                        ch = format[pos];
                    }

                    // If current character is not a digit then error (Unexpected character)
                    if (ch < '0' || ch > '9')
                    {
                        ThrowFormatError();
                    }
                    // Parse alignment digits.
                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        // If reached end of text then error. (Unexpected end of text)
                        if (pos == len)
                        {
                            ThrowFormatError();
                        }
                        ch = format[pos];
                        // So long a current character is a digit and the value of width is less than 100000 ( width limit )
                    }
                    while (ch >= '0' && ch <= '9' && width < WidthLimit);
                    // end of parsing Argument Alignment
                }

                // Consume optional whitespace
                while (pos < len && (ch = format[pos]) == ' ') pos++;

                //
                // Start of parsing of optional formatting parameter.
                //
                object arg = args[index];

                ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null
                // Is current character a colon? which indicates start of formatting parameter.
                if (ch == ':')
                {
                    pos++;
                    int startPos = pos;

                    while (true)
                    {
                        // If reached end of text then error. (Unexpected end of text)
                        if (pos == len)
                        {
                            ThrowFormatError();
                        }
                        ch = format[pos];

                        if (ch == '}')
                        {
                            // Argument hole closed
                            break;
                        }
                        else if (ch == '{')
                        {
                            // Braces inside the argument hole are not supported
                            ThrowFormatError();
                        }

                        pos++;
                    }

                    if (pos > startPos)
                    {
                        itemFormatSpan = format.AsSpan(startPos, pos - startPos);
                    }
                }
                else if (ch != '}')
                {
                    // Unexpected character
                    ThrowFormatError();
                }

                // Construct the output for this arg hole.
                pos++;
                string s = null;
                string itemFormat = null;

                if (cf != null)
                {
                    if (itemFormatSpan.Length != 0)
                    {
                        itemFormat = itemFormatSpan.ToString();
                    }
                    s = cf.Format(itemFormat, arg, provider);
                }

                if (s == null)
                {
                    // Otherwise, fallback to trying IFormattable or calling ToString.
                    if (arg is IFormattable formattableArg)
                    {
                        if (itemFormatSpan.Length != 0)
                        {
                            if (itemFormat == null)
                            {
                                itemFormat = itemFormatSpan.ToString();
                            }
                        }
                        s = formattableArg.ToString(itemFormat, provider);
                    }
                    else if (arg != null)
                    {
                        s = arg.ToString();
                    }
                }
                // Append it to the final output of the Format String.
                if (s == null)
                {
                    s = string.Empty;
                }
                int pad = width - s.Length;
                if (!leftJustify && pad > 0)
                {
                    Append(' ', pad);
                }

                Append(s);
                if (leftJustify && pad > 0)
                {
                    Append(' ', pad);
                }
                // Continue to parse other characters.
            }
        }

        private static void ThrowFormatError()
        {
            throw new FormatException();
        }
    }
    public ref partial struct ValueStringBuilder
    {
        private static readonly ArrayPool<char> pool = ArrayPool<char>.Shared;

        private ValueList<char> _chars;

        public ValueStringBuilder(int initialCapacity)
        {
            _chars = new ValueList<char>(initialCapacity);
        }
        public ValueStringBuilder(in ValueList<char> chars)
        {
            _chars = chars;
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _chars.Size;
        }

        public int Capacity => _chars.TotalCapacity;

        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _chars[index];
            }
        }
#if NET5_0_OR_GREATER
        private static readonly SpanAction<char, ValueList<char>> CopyBlockAction = CopyBlock;
        private static void CopyBlock(Span<char> x,ValueList<char> y)
        {
            var pos = 0;
            var len = y.BufferSlotIndex;
            for (int i = 0; i < len - 2; i++)
            {
                var now = y.bufferSlots[i];
                var nowLen = now.Length;
                new ReadOnlySpan<char>(now, 0, nowLen)
                    .CopyTo(x.Slice(pos));
                pos += nowLen;
            }
            var last = y.bufferSlots[len - 1];
            new ReadOnlySpan<char>(last, 0, y.LocalUsed)
                .CopyTo(x.Slice(pos));
        }
#endif
        public override string ToString()
        {
#if NET5_0_OR_GREATER
            return string.Create(_chars.Size, _chars, CopyBlockAction);
#else
            var buffer = pool.Rent(_chars.Size);
            try
            {
                ToString(buffer);
                return new string(buffer,0, _chars.Size);
            }
            finally
            {
                pool.Return(buffer);
            }
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToString(char[] buffer)
        {
            _chars.ToArray(buffer);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c)
        {
            _chars.Add(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string s)
        {
            if (s == null)
            {
                return;
            }
            _chars.Add(s.AsSpan());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c, int count)
        {
            _chars.Add(c, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Append(char* value, int length)
        {
            _chars.Add(new ReadOnlySpan<char>(value, length));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ReadOnlySpan<char> value)
        {
            _chars.Add(value);
        }
        private const int DefaultBufferSize = 1024;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Append(StreamReader stream)
        {
            Append(stream, DefaultBufferSize);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Append(StreamReader stream, int bufferSize)
        {
            if (bufferSize == -1)
            {
                bufferSize = DefaultBufferSize;
            }
            var buffer = pool.Rent(bufferSize);
            try
            {
                Append(stream, buffer);
            }
            finally
            {
                pool.Return(buffer);
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Append(StreamReader stream,char[] buffer)
        {
            var actualSize = buffer.Length;
            var index = 0;
            while (true)
            {
                var read = stream.Read(buffer, index, actualSize);
                if (read != actualSize)
                {
                    Append(buffer.AsSpan(0, read));
                    break;
                }
                else
                {
                    Append(buffer);
                }
            }
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendSpan(int length)
        {
            _chars.Add('\0', length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            _chars.Dispose();
            this = default;
        }
    }

}