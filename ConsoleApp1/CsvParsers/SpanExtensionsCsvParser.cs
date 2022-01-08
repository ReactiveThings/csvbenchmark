using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using System.Text.Unicode;

namespace ConsoleApp1
{
    public class SpanBytesExtensionsCsvParser : ICsvParser, IByteParser
    {
        public long GetSum(Span<byte> rows)
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

        public long GetSum(string csvData)
        {
            var encoding = Encoding.UTF8;
            var rows = encoding.GetBytes(csvData).AsSpan();

            return GetSum(rows);
        }


        public List<Foo> Parse(string csvData)
        {
            var rows = csvData.FastSplit('\n');

            var result = new List<Foo>();

            foreach (var row in rows)
            {
                var foo = new Foo();
                var columns = row.FastSplit(',');
                int columnIndex = 0;
                foreach (var column in columns)
                {
                    ParseColum(foo, columnIndex, column);

                    columnIndex++;
                }
                result.Add(foo);
            }
            return result;
        }

        public void ParseColum(Foo foo, int index, ReadOnlySpan<char> column)
        {
            switch (index)
            {
                case 0:
                    foo.A = int.Parse(column);
                    break;
                case 1:
                    foo.B = int.Parse(column);
                    break;
                case 2:
                    foo.C = int.Parse(column);
                    break;
                case 3:
                    foo.D = int.Parse(column);
                    break;
                case 4:
                    foo.E = int.Parse(column);
                    break;
                default:
                    break;
            }
        }
    }
    public class SpanExtensionsCsvParser : ICsvParser
    {
        public long GetSum(string csvData)
        {
            var rows = csvData.FastSplit('\n');

            long sum = 0;
            foreach (var row in rows)
            {
                var columns = row.FastSplit(',');
                foreach (var column in columns)
                {
                    sum += int.Parse(column);
                }
            }
            return sum;
        }

        public List<Foo> Parse(string csvData)
        {
            var rows = csvData.FastSplit('\n');

            var result = new List<Foo>();

            foreach (var row in rows)
            {
                var foo = new Foo();
                var columns = row.FastSplit(',');
                int columnIndex = 0;
                foreach (var column in columns)
                {
                    ParseColum(foo, columnIndex, column);

                    columnIndex++;
                }
                result.Add(foo);
            }
            return result;
        }

        public void ParseColum(Foo foo, int index, ReadOnlySpan<char> column)
        {
            switch (index)
            {
                case 0:
                    foo.A = int.Parse(column);
                    break;
                case 1:
                    foo.B = int.Parse(column);
                    break;
                case 2:
                    foo.C = int.Parse(column);
                    break;
                case 3:
                    foo.D = int.Parse(column);
                    break;
                case 4:
                    foo.E = int.Parse(column);
                    break;
                default:
                    break;
            }
        }
    }
}
