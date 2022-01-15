using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace PingNet.Services
{
    public interface INetworkAnalyser
    {
        Task<List<string>> BroadcastAsync(string ipAddressRange, int timeout = 5000);
        Task<List<PingReply>> PingAsync(List<IPAddress> ips, int timeout = 5000);
    }
}