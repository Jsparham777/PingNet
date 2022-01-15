using BenchmarkDotNet.Running;
using PingNet.Benchmarks;
using System;

namespace Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var results = BenchmarkRunner.Run<NetworkAnalyserBenchmark>();

            Console.WriteLine(results);
        }
    }
}
