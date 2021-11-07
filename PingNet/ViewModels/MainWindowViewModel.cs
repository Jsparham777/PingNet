using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PingNet.ViewModels
{
    /// <summary>
    /// Main Window view model.
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly BackgroundWorker _backgroundWorker = new();
        private readonly List<string> _ipAddresses = new();
        private readonly AppSettings _options;
        private ObservableCollection<string> _discoveredMachines = new();
        private bool _searching = false;
        private bool _searchComplete = false;

        /// <summary>
        /// Instantiates a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="options">Options read from the appsettings.json file.</param>
        public MainWindowViewModel(IOptions<AppSettings> options)
        {
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _options = options.Value;
            IPAddressRange = _options.IPAddressRange;
        }

        /// <summary>
        /// Discover command.
        /// </summary>
        public RelayCommand Discover { get { return new RelayCommand(x => ExecuteDiscover()); } }
        
        /// <summary>
        /// Connect command.
        /// </summary>
        public RelayCommand Connect { get { return new RelayCommand(x => ExecuteConnect(x)); } }
        
        /// <summary>
        /// Exit command.
        /// </summary>
        public static RelayCommand Exit { get { return new RelayCommand(x => { Application.Current.Shutdown(); }); } }

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
        private void ExecuteDiscover()
        {






            _discoveredMachines.Clear();
            
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += DispatcherTimer_Tick;
            timer.Start();

            _backgroundWorker.RunWorkerAsync();
        }


        /// <summary>
        /// TEST
        /// </summary>
        /// <param name="ips"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
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

        private async Task DoWork()
        {
            string ipRange = _options.IPAddressRange;

            for (int i = 0; i < 256; i++)
            {
                _ipAddresses.Add($"{ipRange}.{i}");
            }



        }


        /// <summary>
        /// Pings all IP addresses within a given range and adds the connected machines to the <see cref="DiscoveredMachines"/> collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _searchComplete = false;
            string ipRange = _options.IPAddressRange;

            for (int i = 0; i < 256; i++)
            {
                _ipAddresses.Add($"{ipRange}.{i}");
            }

            Parallel.For(0, _ipAddresses.Count(), (i, loopState) =>
            {
                var ping = new Ping();
                var pingReply = ping.Send(_ipAddresses[i].ToString());

                if (pingReply.Status == IPStatus.Success)
                {
                    var address = pingReply.Address.ToString();

                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (pingReply.Status == IPStatus.Success)
                        {
                            var address = pingReply.Address.ToString();
                            var hostname = Dns.GetHostEntry(pingReply.Address).HostName;

                            var host = hostname.Replace(".Home", "");

                            var entry = $"{address} - {host}";
                            // Lock here. This isn't thread safe
                            lock (_lock)
                            {
                                if(!DiscoveredMachines.Contains(entry))
                                    DiscoveredMachines.Add(entry);
                            }
                        }
                    });
                }
            });
          
            _searchComplete = true;
        }

        private readonly object _lock = new object();

        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            Searching = _searchComplete == false ? true : false;
        }

        /// <summary>
        /// Starts the RDP session in full screen mode.
        /// </summary>
        private static void ExecuteConnect(object parameter)
        {
            string entry = parameter as string;
            string ip = entry.Split(" - ")[0];

            Process.Start(new ProcessStartInfo("mstsc", $"/f /v:{ip}"));
        }  
    }
}
