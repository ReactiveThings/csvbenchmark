using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ConsoleApp1
{
    [MemoryDiagnoser]
    public class CsvFileParserBenchmarks
    {
        private readonly NaiveCsvParser naive;
        private readonly ICsvFileParser csvHelper;
        private readonly SpanExtensionsRecordCsvParser spanExtensionsRecordCsvParser;
        private readonly PipelineCsvParser pipelineCsvParser;

        public PipelineCsvParserV2 pipelineCsvParserV2 { get; }

        private FastStringReaderCsvParser fastStringReaderCsvParser;
        private FastStringReaderExtensionsCsvParser fastStringReaderExtensionsCsvParser;
        private FastStringReaderExtensionsCsvParser1 fastStringReaderExtensionsCsvParser1;

        [Params(20, 1000, 1000000)]
        public int N { get; set; }


        private readonly string fileName = @"C:\Temp\benchmark.csv";
        public CsvFileParserBenchmarks()
        {
            var rand = new Random(1);
            var csvData = String.Join('\n', Enumerable.Range(0, 20).Select(p => $"{1},{2},{3},{4},{5}"));
            //var csvData = String.Join('\n', Enumerable.Range(0, 100000).Select(p => $"{rand.Next()},{rand.Next()},{rand.Next()},{rand.Next()},{rand.Next()}"));
            File.WriteAllText(fileName + 20, csvData, new UTF8Encoding(false));

            var csvData1 = String.Join('\n', Enumerable.Range(0, 1000).Select(p => $"{rand.Next()},{rand.Next()},{rand.Next()},{rand.Next()},{rand.Next()}"));
            File.WriteAllText(fileName + 1000, csvData1, new UTF8Encoding(false));

            var csvData2 = String.Join('\n', Enumerable.Range(0, 1000000).Select(p => $"{rand.Next()},{rand.Next()},{rand.Next()},{rand.Next()},{rand.Next()}"));
            File.WriteAllText(fileName + 1000000, csvData2, new UTF8Encoding(false));

            naive = new NaiveCsvParser();
            csvHelper = new CsvHelperCsvParser();
            spanExtensionsRecordCsvParser = new SpanExtensionsRecordCsvParser();
            pipelineCsvParser = new PipelineCsvParser();
            pipelineCsvParserV2 = new PipelineCsvParserV2();
            fastStringReaderCsvParser = new FastStringReaderCsvParser();
            fastStringReaderExtensionsCsvParser = new FastStringReaderExtensionsCsvParser();
            fastStringReaderExtensionsCsvParser1 = new FastStringReaderExtensionsCsvParser1();
        }

        [Benchmark]
        public Task<long> NaiveReadAll()
        {
            return naive.GetSumMemoryAsync(fileName + N);
        }

        [Benchmark]
        public Task<long> Naive()
        {
            return naive.GetSumAsync(fileName+N);
        }

        [Benchmark]
        public Task<long> CsvHelper()
        {
            return csvHelper.GetSumAsync(fileName + N);
        }

        [Benchmark]
        public Task<long> SpanStream()
        {
            return spanExtensionsRecordCsvParser.GetSumAsync(fileName + N);
        }

        [Benchmark()]
        public Task<long> SpanPipe()
        {
            return pipelineCsvParser.GetSumAsync(fileName + N);
        }

        [Benchmark(Baseline = true)]
        public Task<long> SpanPipeV2()
        {
            return pipelineCsvParserV2.GetSumAsync(fileName + N);
        }

        [Benchmark()]
        public Task<long> FastStringReaderCsvParser()
        {
            return fastStringReaderCsvParser.GetSumAsync(fileName + N);
        }

        [Benchmark()]
        public Task<long> FastStringReaderExtensionsCsvParser()
        {
            return fastStringReaderExtensionsCsvParser.GetSumAsync(fileName + N);
        }

        [Benchmark()]
        public Task<long> FastStringReaderExtensionsCsvParser1()
        {
            return fastStringReaderExtensionsCsvParser1.GetSumAsync(fileName + N);
        }


    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            await TestImplementationAsync();
            var summary = BenchmarkRunner.Run<CsvFileParserBenchmarks>();
        }

        private static async Task TestImplementationAsync()
        {
            var benchmark = new CsvFileParserBenchmarks();
            benchmark.N = 20;
            var naive = await benchmark.Naive();
            var csvHelper = await benchmark.CsvHelper();
            var spanStream = await benchmark.SpanStream();
            var spanPipe = await benchmark.SpanPipe();
            var spanPipev2 = await benchmark.SpanPipeV2();
            var fastStringReaderCsvParser = await benchmark.FastStringReaderCsvParser();
            var fastStringReaderExtensionsCsvParser = await benchmark.FastStringReaderExtensionsCsvParser();
            var fastStringReaderExtensionsCsvParser1 = await benchmark.FastStringReaderExtensionsCsvParser1();

            if (!new[] { csvHelper, spanStream, spanPipe, spanPipev2, fastStringReaderCsvParser, fastStringReaderExtensionsCsvParser, fastStringReaderExtensionsCsvParser1 }.All(r => r == naive))
            {
                throw new Exception($"Naive {naive} csvHelper: {csvHelper} SpanStream: {spanStream} spanPipe: {spanPipe} spanPipev2 {spanPipev2} fastStringReaderCsvParser {fastStringReaderCsvParser} fastStringReaderExtensionsCsvParser {fastStringReaderExtensionsCsvParser}");
            }
        }

        private static void TestImplementation()
        {
            var benchmark = new CsvParserBenchmarks();
            benchmark.TestImplementation();
        }
    }
}
