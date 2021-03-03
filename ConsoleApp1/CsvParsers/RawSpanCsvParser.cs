using System;

namespace ConsoleApp1
{
    public class RawSpanCsvParser : ICsvParser
    {
        public long GetSum(string csvData)
        {
            var rows = csvData.AsSpan();

            long sum = 0;
            int index;
            do
            {
                index = rows.IndexOf('\n');

                var row = rows.Slice(0, index == -1 ? rows.Length : index);

                sum += GetSumOfColums(row);

                rows = rows.Slice(index + 1);

            } while (index != -1);

            return sum;
        }

        private long GetSumOfColums(ReadOnlySpan<char> row)
        {
            long sum = 0;
            int index;
            do
            {
                index = row.IndexOf(',');

                var column = row.Slice(0, index == -1 ? row.Length : index);

                sum += int.Parse(column);

                row = row.Slice(index + 1);
            } while (index != -1);

            return sum;
        }
    }
}
