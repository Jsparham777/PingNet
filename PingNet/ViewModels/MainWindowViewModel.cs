using Microsoft.Extensions.Options;
using PingNet.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly INetworkAnalyser _networkAnalyser;
        private ObservableCollection<string> _discoveredMachines = new();
        private bool _searching;
        private int _numberOfDevicesFound;

        /// <summary>
        /// Instantiates a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="options">Options read from the appsettings.json file.</param>
        public MainWindowViewModel(IOptions<AppSettings> options, INetworkAnalyser networkAnalyser)
        {
            _options = options.Value;
            IPAddressRange = _options.IPAddressRange;
            _networkAnalyser = networkAnalyser;
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
        /// Gets the number of devices found.
        /// </summary>
        public int NumberOfDevicesFound
        {
            get => _numberOfDevicesFound;
            set
            {
                _numberOfDevicesFound = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Starts the discovery background worker.
        /// </summary>
        private async Task ExecuteDiscover()
        {
            DiscoveredMachines.Clear();

            Searching = true;

            DiscoveredMachines = new ObservableCollection<string>(await _networkAnalyser.BroadcastAsync(IPAddressRange, 1000));

            NumberOfDevicesFound = DiscoveredMachines.Count;

            Searching = false;
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
