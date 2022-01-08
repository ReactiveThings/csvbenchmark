using System;
using System.Buffers.Text;
using System.Text;

namespace ConsoleApp1
{
    public interface IByteParser
    {
        long GetSum(Span<byte> csvData);
    }
    public class RawSpanCsvParser : ICsvParser, IByteParser
    {
        public long GetSum(Span<byte> rows)
        {
            long sum = 0;
            int index;
            do
            {
                index = rows.IndexOf((byte)'\n');

                var row = rows.Slice(0, index == -1 ? rows.Length : index);

                sum += GetSumOfColums(row);

                rows = rows.Slice(index + 1);

            } while (index != -1);

            return sum;
        }

        public long GetSum(string csvData)
        {
            var encoding = Encoding.UTF8;
            var rows = encoding.GetBytes(csvData).AsSpan();

            return GetSum(rows);
        }



        private long GetSumOfColums(ReadOnlySpan<byte> row)
        {
            long sum = 0;
            int index;
            do
            {
                index = row.IndexOf((byte)',');

                var column = row.Slice(0, index == -1 ? row.Length : index);

                Utf8Parser.TryParse(column, out int num, out int _);
                sum += num;

                row = row.Slice(index + 1);
            } while (index != -1);

            return sum;
        }
    }
}
