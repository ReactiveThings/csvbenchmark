using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FastStringReaderExtensionsCsvParser : ICsvFileParser
    {
        public async Task<long> GetSumAsync(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var stringReader = new StreamReader(stream, Encoding.UTF8);

            long sum = 0;

            char[] buffer = ArrayPool<char>.Shared.Rent(1024);
            var memoryBuffer = Memory<char>.Empty;

            while (!stringReader.EndOfStream)
            {
                var restCount = memoryBuffer.Length;
                memoryBuffer.CopyTo(buffer);

                memoryBuffer = buffer.AsMemory();
                var bytesRead = await stringReader.ReadAsync(buffer, restCount, buffer.Length - restCount);

                if (bytesRead == 0)
                {
                    // EOF
                    break;
                }

                sum += SumRow(ref memoryBuffer, bytesRead + restCount, stringReader.EndOfStream);
            }

            return sum;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long SumRow(ref Memory<char> memoryBuffer, int count, bool endOfStream)
        {
            var spanBuffer = memoryBuffer.Span.Slice(0, count);
            var last = endOfStream ? spanBuffer.Length : spanBuffer.LastIndexOf('\n') + 1;

            var sum = GetSum(spanBuffer.Slice(0, last));
            memoryBuffer = memoryBuffer.Slice(last);
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
