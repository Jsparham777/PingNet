using BenchmarkDotNet.Attributes;
using PingNet.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PingNet.Benchmarks
{
    [MemoryDiagnoser]
    public class NetworkAnalyserBenchmark
    {
        private NetworkAnalyser _networkAnalyser = new();

        [Benchmark]
        public void Broadcast()
        {
            string ipRange = "192.168.1";

            _networkAnalyser.BroadcastAsync(ipRange, 1000);
        }

        [Benchmark]
        public void Ping()
        {
            var ipList = new List<IPAddress>() 
            { 
                IPAddress.Parse("192.168.1.1"),
                IPAddress.Parse("192.168.1.2")
            };

            _networkAnalyser.PingAsync(ipList, 1000);
        }
    }
}
