using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace PingNet.ViewModels
{
    /// <summary>
    /// Main Window view model.
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly AppSettings _options;
        private ObservableCollection<string> _discoveredMachines = new();
        private bool _searching;

        /// <summary>
        /// Instantiates a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="options">Options read from the appsettings.json file.</param>
        public MainWindowViewModel(IOptions<AppSettings> options)
        {
            _options = options.Value;
            IPAddressRange = _options.IPAddressRange;
        }

        /// <summary>
        /// Discover command.
        /// </summary>
        public RelayCommand Discover => new(async x => await ExecuteDiscover());

        /// <summary>
        /// Connect command.
        /// </summary>
        public static RelayCommand Connect => new(x => ExecuteConnect(x));

        /// <summary>
        /// Exit command.
        /// </summary>
        public static RelayCommand Exit => new(x => { Application.Current.Shutdown(); });

        /// <summary>
        /// The IP address range.
        /// </summary>
        public string IPAddressRange { get; set; }

        /// <summary>
        /// Gets and sets the discovered machines collection.
        /// </summary>
        public ObservableCollection<string> DiscoveredMachines
        {
            get => _discoveredMachines;
            set
            {
                _discoveredMachines = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets and sets the searching status.
        /// </summary>
        public bool Searching
        {
            get => _searching;
            set
            {
                _searching = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Starts the discovery background worker.
        /// </summary>
        private async Task ExecuteDiscover()
        {
            DiscoveredMachines.Clear();
            await BroadcastAsync();
        }

        /// <summary>
        /// Pings all IP addresses within a given range and adds the connected machines to the <see cref="DiscoveredMachines"/> collection.
        /// </summary>
        private async Task BroadcastAsync()
        {
            Searching = true;

            var addresses = ConstructAddresses(_options.IPAddressRange);

            var replies = await PingAsync(addresses);

            foreach (var reply in replies)
            {
                DiscoveredMachines.Add(FormatAddress(reply));
            }

            Searching = false;
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

        /// <summary>
        /// Pings all given IP addresses and awaits a successful reply.
        /// </summary>
        /// <param name="ips">IP address list.</param>
        /// <param name="timeout">Ping timeout.</param>
        /// <returns>A list of successfully ping'd IP addresses.</returns>
        public static async Task<List<PingReply>> PingAsync(List<IPAddress> ips, int timeout = 5000)
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
        /// Starts the RDP session in full screen mode.
        /// </summary>
        private static void ExecuteConnect(object parameter)
        {
            string entry = parameter as string;
            string ip = entry.Split(" - ")[0];

            _ = Process.Start(new ProcessStartInfo("mstsc", $"/f /v:{ip}"));
        }
    }
}
