using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class NaiveCsvParser : ICsvParser, ICsvFileParser
    {
        public long GetSum(string csvData)
        {
            var rows = csvData.Split('\n');

            long sum = 0;
            foreach (var row in rows)
            {
                var columns = row.Split(",");
                foreach (var column in columns)
                {
                    sum += int.Parse(column);
                }
            }
            return sum;
        }

        public async Task<long> GetSumMemoryAsync(string filePath)
        {
            var csvData = await File.ReadAllTextAsync(filePath);
            var rows = csvData.Split('\n');

            long sum = 0;
            foreach (var row in rows)
            {
                var columns = row.Split(",");
                foreach (var column in columns)
                {
                    sum += int.Parse(column);
                }
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
                var columns = row.Split(",");
                foreach (var column in columns)
                {
                    sum += int.Parse(column);
                }
            }

            return sum;
        }
    }
}
