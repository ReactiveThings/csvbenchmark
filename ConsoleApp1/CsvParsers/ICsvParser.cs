using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface ICsvParser
    {
        long GetSum(string csvData);
    }

    public interface ICsvFileParser
    {
        Task<long> GetSumAsync(string filePath);
    }
}
