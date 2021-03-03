using System;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    public static class StringSpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SplitEnumerator<char> FastSplit(this string str, char separator)
        {
            // LineSplitEnumerator is a struct so there is no allocation here
            return new SplitEnumerator<char>(str.AsSpan(), separator);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SplitEnumerator<T> FastSplit<T>(this ReadOnlySpan<T> str, T separator) where T : IEquatable<T>
        {
            // LineSplitEnumerator is a struct so there is no allocation here
            return new SplitEnumerator<T>(str, separator);
        }



        // Must be a ref struct as it contains a ReadOnlySpan<char>
        public ref struct SplitEnumerator<T> where T : IEquatable<T>
        {
            private ReadOnlySpan<T> _str;
            private readonly T separator;

            public SplitEnumerator(ReadOnlySpan<T> str, T separator)
            {
                _str = str;
                this.separator = separator;
                Current = default;
            }

            // Needed to be compatible with the foreach operator
            public SplitEnumerator<T> GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0) // Reach the end of the string
                    return false;

                var index = span.IndexOf(separator);
                if (index == -1) // The string is composed of only one line
                {
                    _str = ReadOnlySpan<T>.Empty; // The remaining string is an empty string
                    Current = span;
                    return true;
                }

                Current = span.Slice(0, index);
                _str = span.Slice(index + 1);
                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }


        public delegate T ParseRowFunc<out T>(int index, SplitEnumerator<char> columns);

        public static RowEnumerator<T> ParseCsv<T>(this string csvString, ParseRowFunc<T> parseRowAction, char rowSeparator = '\n', char columnSeparator = ',') where T : new()
        {
            return new RowEnumerator<T>(csvString, parseRowAction, rowSeparator, columnSeparator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RowEnumerator<T> ParseCsv<T>(this ReadOnlySpan<char> csvSpan, ParseRowFunc<T> parseRowAction, char rowSeparator = '\n', char columnSeparator = ',') where T : new()
        {
            return new RowEnumerator<T>(csvSpan, parseRowAction, rowSeparator, columnSeparator);
        }

        public ref struct RowEnumerator<T>
        {
            private readonly ParseRowFunc<T> parseRowFunc;
            private readonly char columnSeparator;
            private SplitEnumerator<char> rowsEnumerator;
            private int rowIndex;

            public RowEnumerator(ReadOnlySpan<char> str, ParseRowFunc<T> parseRowFunc, char rowSeparator = '\n', char columnSeparator = ',')
            {
                rowsEnumerator = str.FastSplit(rowSeparator);
                this.parseRowFunc = parseRowFunc;
                this.columnSeparator = columnSeparator;
                Current = default;
                rowIndex = 0;
            }

            // Needed to be compatible with the foreach operator
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public RowEnumerator<T> GetEnumerator() => this;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                ref SplitEnumerator<char> rows = ref rowsEnumerator;
                var moveNext = rows.MoveNext();
                Current = parseRowFunc(rowIndex++, rows.Current.FastSplit(columnSeparator));
                return moveNext;
            }

            public T Current { get; private set; }
        }
    }


}
