using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FastStringReaderCsvParser : ICsvFileParser
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
                var bytesRead = await stringReader.ReadAsync(memoryBuffer.Slice(restCount));
                memoryBuffer = memoryBuffer.Slice(0, bytesRead + restCount);
                if (bytesRead == 0)
                {
                    // EOF
                    break;
                }

                sum += SumRow(ref memoryBuffer);

                if (stringReader.EndOfStream)
                {
                    sum += GetSumOfColums(memoryBuffer.Span);
                }
            }

            return sum;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long SumRow(ref Memory<char> memoryBuffer)
        {
            var spanBuffer = memoryBuffer.Span;
            var linePosition = -1;
            long sum = 0;
            do
            {
                linePosition = spanBuffer.IndexOf('\n');

                if (linePosition >= 0)
                {
                    sum += GetSumOfColums(spanBuffer.Slice(0, linePosition));
                    spanBuffer = spanBuffer.Slice(linePosition + 1);
                }
            }
            while (linePosition >= 0);

            memoryBuffer = memoryBuffer.Slice(memoryBuffer.Length - spanBuffer.Length);

            return sum;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
