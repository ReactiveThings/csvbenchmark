using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class SpanExtensionsRecordCsvParser : ICsvParser, ICsvFileParser
    {
        public long GetSum(string csvData)
        {
            long sum = 0;
            var rows = csvData.ParseCsv(ParseRow);
            foreach (var record in rows)
            {
                sum += record;
            }
            return sum;
        }

        private long ParseRow(int rowIndex, StringSpanExtensions.SplitEnumerator<char> columns)
        {
            long sum = 0;

            foreach (var column in columns)
            {
                sum += int.Parse(column);
            }

            return sum;
        }

        public async Task<long> GetSumAsync(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var sr = new StreamReader(stream, Encoding.UTF8);

            long sum = 0;
            string row;
            while ((row = await sr.ReadLineAsync()) != null)
            {
                sum += ParseRow(row);
            }

            return sum;
        }


        private long ParseRow(ReadOnlySpan<char> row)
        {
            long sum = 0;

            foreach (var column in row.FastSplit(','))
            {
                sum += int.Parse(column);
            }

            return sum;
        }

    }
}
