using BenchmarkDotNet.Running;
using PerformanceChecker.Benchmark.Benchmarks;

namespace PerformanceChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<XmlReaderBenchmark>();
        }
    }
}
