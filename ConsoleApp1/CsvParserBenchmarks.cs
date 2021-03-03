using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace ConsoleApp1
{
    [MemoryDiagnoser]
    public class CsvParserBenchmarks
    {
        private readonly string csvData;
        private readonly ICsvParser naive;
        private readonly ICsvParser rawSpan;
        private readonly ICsvParser spanExtenstions;
        private readonly ICsvParser csvHelper;
        private readonly ICsvParser spanExtensionsRecordCsvParser;
        private readonly int N = 1000000;
        public CsvParserBenchmarks()
        {

            var rand = new Random(1);
            csvData = String.Join('\n', Enumerable.Range(0, N).Select(p => $"{rand.Next()},{rand.Next()},{rand.Next()},{rand.Next()},{rand.Next()}"));

            naive = new NaiveCsvParser();
            rawSpan = new RawSpanCsvParser();
            spanExtenstions = new SpanExtensionsCsvParser();
            csvHelper = new CsvHelperCsvParser();
            spanExtensionsRecordCsvParser = new SpanExtensionsRecordCsvParser();
        }

        //[Benchmark(Baseline = true)]
        public long Naive()
        {
            return naive.GetSum(csvData);
        }

        //[Benchmark]
        public long RawSpan()
        {
            return rawSpan.GetSum(csvData);
        }

        [Benchmark(Baseline = true)]
        public long SpanExtenstions()
        {
            return spanExtenstions.GetSum(csvData);
        }

        //[Benchmark]
        public long CsvHelper()
        {
            return csvHelper.GetSum(csvData);
        }

        [Benchmark]
        public long SpanRecord()
        {
            return spanExtensionsRecordCsvParser.GetSum(csvData);
        }

        public void TestImplementation()
        {
            var naive = this.Naive();
            var rawSpan = this.RawSpan();
            var spanExtenstions = this.SpanExtenstions();
            var csvHelper = this.CsvHelper();
            var spanRecord = this.SpanRecord();

            if (!new[] { rawSpan, spanExtenstions, csvHelper, spanRecord }.All(r => r == naive))
            {
                throw new Exception($"Naive {naive} rawSpan: {rawSpan} spanExtenstions: {spanExtenstions} csvHelper: {csvHelper} spanRecord: {spanRecord}");
            }
        }
    }
}
