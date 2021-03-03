using System;
using System.Buffers;
using System.Buffers.Text;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class PipelineCsvParser : ICsvFileParser
    {
        public async Task<long> GetSumAsync(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            var reader = PipeReader.Create(stream, new StreamPipeReaderOptions());
            long sum = 0;
            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;
                long? rowSum;
                while ((rowSum = ReadLine(ref buffer)) != null)
                {
                    sum += rowSum.Value;
                }

                if (result.IsCompleted)
                {
                    sum = ReadLastLine(sum, ref buffer);
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            await reader.CompleteAsync();
            return sum;
        }

        private static long ReadLastLine(long sum, ref ReadOnlySequence<byte> buffer)
        {
            var sequenceReader = new SequenceReader<byte>(buffer);
            buffer = buffer.Slice(sequenceReader.UnreadSpan.Length);
            sum += ParseRow(sequenceReader.UnreadSpan);
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long? ReadLine(ref ReadOnlySequence<byte> buffer)
        {
            var reader = new SequenceReader<byte>(buffer);
            if (reader.TryReadTo(out ReadOnlySpan<byte> row, (byte)'\n'))
            {
                buffer = buffer.Slice(reader.Position);
                return ParseRow(row);
            }
            return null;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ParseRow(ReadOnlySpan<byte> row)
        {
            long sum = 0;
            foreach (var column in row.FastSplit((byte)','))
            {
                Utf8Parser.TryParse(column, out int num, out int _);
                sum += num;
            }

            return sum;
        }

    }
}
