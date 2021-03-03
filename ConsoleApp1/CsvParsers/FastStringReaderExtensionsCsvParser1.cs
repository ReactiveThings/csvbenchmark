using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FastStringReaderExtensionsCsvParser1 : ICsvFileParser
    {
        public async Task<long> GetSumAsync(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var stringReader = new StreamReader(stream, Encoding.UTF8);

            long sum = 0;

            char[] buffer = ArrayPool<char>.Shared.Rent(1024);
            int restCount = 0;

            while (!stringReader.EndOfStream)
            {
                Array.Copy(buffer, buffer.Length - restCount, buffer, 0, restCount);

                var bytesRead = await stringReader.ReadAsync(buffer, restCount, buffer.Length - restCount);

                if (bytesRead == 0)
                {
                    // EOF
                    break;
                }

                sum += SumRow(buffer, bytesRead + restCount, stringReader.EndOfStream, out restCount);
            }

            return sum;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long SumRow(char[] memoryBuffer, int count, bool endOfStream, out int restCount)
        {
            var spanBuffer = memoryBuffer.AsSpan(0, count);
            var last = endOfStream ? spanBuffer.Length : spanBuffer.LastIndexOf('\n') + 1;

            var sum = GetSum(spanBuffer.Slice(0, last));
            restCount = memoryBuffer.Length - last;
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetSum(ReadOnlySpan<char> csvData)
        {
            long sum = 0;
            var rows = csvData.ParseCsv(ParseRow);
            foreach (var record in rows)
            {
                sum += record;
            }
            return sum;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long ParseRow(int rowIndex, StringSpanExtensions.SplitEnumerator<char> columns)
        {
            long sum = 0;

            foreach (var column in columns)
            {
                sum += int.Parse(column);
            }

            return sum;
        }
    }
}
