using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace PingNet.Services
{
    public class NetworkAnalyser : INetworkAnalyser
    {
        /// <summary>
        /// Pings all IP addresses within a given range and adds the connected machines to the <see cref="DiscoveredMachines"/> collection.
        /// </summary>
        /// <param name="ipAddressRange"></param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task<List<string>> BroadcastAsync(string ipAddressRange, int timeout = 5000)
        {
            var discoveredMachines = new List<string>();

            var addresses = ConstructAddresses(ipAddressRange);

            var replies = await PingAsync(addresses, timeout);

            foreach (var reply in replies)
            {
                discoveredMachines.Add(FormatAddress(reply));
            }

            return discoveredMachines;
        }

        /// <summary>
        /// Pings all given IP addresses and awaits a successful reply.
        /// </summary>
        /// <param name="ips">IP address list.</param>
        /// <param name="timeout">Ping timeout.</param>
        /// <returns>A list of successfully ping'd IP addresses.</returns>
        public async Task<List<PingReply>> PingAsync(List<IPAddress> ips, int timeout = 5000)
        {
            var pingTasks = ips.Select(async ip =>
            {
                using Ping ping = new();
                return await ping.SendPingAsync(ip, timeout);
            });

            var results = await Task.WhenAll(pingTasks);

            return results.Where(x => x.Status == IPStatus.Success).ToList();
        }

        /// <summary>
        /// Formats a reply's IP address, appending the host name.
        /// </summary>
        /// <param name="reply">The <see cref="PingReply"/> to format.</param>
        /// <returns>The formatted IP address with appended host name.</returns>
        private static string FormatAddress(PingReply reply)
        {
            var hostname = Dns.GetHostEntry(reply.Address).HostName.Replace(".Home", "");

            var entry = $"{reply.Address} - {hostname}";

            return entry;
        }

        /// <summary>
        /// Constructs a list of all possible IP addresses within a given range. (last octet 0 to 255)
        /// </summary>
        /// <param name="range">The first three octets of an IP address.</param>
        /// <returns>The IP address list.</returns>
        private static List<IPAddress> ConstructAddresses(string range)
        {
            List<IPAddress> addresses = new();

            for (int i = 0; i < 256; i++)
            {
                addresses.Add(IPAddress.Parse($"{range}.{i}"));
            }

            return addresses;
        }
    }
}
