using System;
using System.Runtime.CompilerServices;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using System.Text.Unicode;
using System;
using System.Diagnostics;
using System.Linq;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
Stopwatch sw = new Stopwatch();

var csvData = String.Join('\n', Enumerable.Repeat(0, 100000000).Select(p => $"1,2,3,4,5"));
// var encoding = Encoding.UTF8;
// var rows = encoding.GetBytes(csvData);


sw.Start();

// var sum = Enumerable.Repeat(0, 96)
// .AsParallel()
// .Select(p => GetSum(rows.AsSpan())).Sum();
var sum = GetStringSum(csvData);

sw.Stop();
Console.WriteLine(sum);
Console.WriteLine("Elapsed={0}",sw.Elapsed);

long GetStringSum(string csvData)
{
    var encoding = Encoding.UTF8;
    var rows = encoding.GetBytes(csvData).AsSpan();

    return GetSum(rows);
}

long GetSum(Span<byte> rows)
{
    long sum = 0;
    foreach (var row in rows.FastSplit((byte)'\n'))
    {
        var columns = row.FastSplit((byte)',');
        foreach (var column in columns)
        {
            Utf8Parser.TryParse(column, out int num, out int _);
            sum += num;
        }
    }
    return sum;
}

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SplitEnumerator<T> FastSplit<T>(this Span<T> str, T separator) where T : IEquatable<T>
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
    }