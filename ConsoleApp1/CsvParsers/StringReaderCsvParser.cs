using System;
using System.IO;

namespace ConsoleApp1
{
    public class StringReaderCsvParser : ICsvParser
    {
        public long GetSum(string csvData)
        {
            var stringReader = new StringReader(csvData);

            long sum = 0;
            string row;

            while ((row = stringReader.ReadLine()) != null)
            {
                sum += GetSumOfColums(row);
            }

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
