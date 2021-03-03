using System;
using System.Buffers;
using System.Buffers.Text;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class PipelineCsvParserV2 : ICsvFileParser
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

                sum += ProcessLine(ref buffer);

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                {
                    sum += ParseRow(buffer.FirstSpan);
                    break;
                }
            }

            await reader.CompleteAsync();
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ProcessLine(ref ReadOnlySequence<byte> buffer)
        {
            long str = 0;

            if (buffer.IsSingleSegment)
            {
                var span = buffer.FirstSpan;
                int consumed;
                while (span.Length > 0)
                {
                    var newLine = span.IndexOf((byte)'\n');

                    if (newLine == -1) break;

                    var line = span.Slice(0, newLine);
                    str += ParseRow(line);

                    consumed = line.Length + 1;
                    span = span.Slice(consumed);
                    buffer = buffer.Slice(consumed);
                }
            }
            else
            {
                var sequenceReader = new SequenceReader<byte>(buffer);

                while (!sequenceReader.End)
                {
                    while (sequenceReader.TryReadTo(out ReadOnlySpan<byte> line, (byte)'\n'))
                    {
                        str += ParseRow(line);
                    }

                    buffer = buffer.Slice(sequenceReader.Position);
                    sequenceReader.Advance(buffer.Length);
                }
            }

            return str;
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
