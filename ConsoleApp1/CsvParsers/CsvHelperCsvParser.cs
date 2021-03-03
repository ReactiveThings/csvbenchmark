using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;

namespace ConsoleApp1
{
    public class CsvHelperCsvParser : ICsvParser, ICsvFileParser
    {
        public long GetSum(string csvData)
        {
            var csv = new CsvReader(new StringReader(csvData), CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = false;
            long sum = 0;
            while (csv.Read())
            {
                sum += csv.GetField<int>(0);
                sum += csv.GetField<int>(1);
                sum += csv.GetField<int>(2);
                sum += csv.GetField<int>(3);
                sum += csv.GetField<int>(4);
            }

            return sum;
        }

        public async Task<long> GetSumAsync(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);
            using var streamReader = new StreamReader(fileStream);
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = false;
            long sum = 0;
            while (await csv.ReadAsync())
            {
                sum += csv.GetField<int>(0);
                sum += csv.GetField<int>(1);
                sum += csv.GetField<int>(2);
                sum += csv.GetField<int>(3);
                sum += csv.GetField<int>(4);
            }

            return sum;
        }
    }
}
