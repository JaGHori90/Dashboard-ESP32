using ApiClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WpfViewModels.Common;

namespace WpfViewModels.ViewModels
{
    // Zeigt Sensordaten an, die ausschließlich über SensorReadingsApiClient (HTTP) von der
    // Database-API geladen werden - kein direkter Datenbankzugriff aus der WPF-App.
    public class SensorViewModel : BaseViewModel
    {
        private readonly SensorReadingsApiClient _apiClient;

        private string _deviceId = "esp32-balcony";
        public string DeviceId
        {
            get { return _deviceId; }
            set { SetProperty(ref _deviceId, value); }
        }

        private ObservableCollection<SensorReadingDto> _readings = new();
        public ObservableCollection<SensorReadingDto> Readings
        {
            get { return _readings; }
            set { SetProperty(ref _readings, value); }
        }

        private SensorReadingDto? _latest;
        public SensorReadingDto? Latest
        {
            get { return _latest; }
            set { SetProperty(ref _latest, value); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { SetProperty(ref _statusMessage, value); }
        }

        public RelayCommand CmdRefresh { get; }

        public SensorViewModel(IWindowController windowController, SensorReadingsApiClient apiClient)
            : base(windowController)
        {
            _apiClient = apiClient;
            CmdRefresh = new RelayCommand(async obj => await RefreshAsync());
        }

        public override async Task InitializeDataAsync()
        {
            await RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            try
            {
                var readings = await _apiClient.GetAllAsync(DeviceId, take: 100);
                Readings = new ObservableCollection<SensorReadingDto>(readings);
                Latest = Readings.FirstOrDefault();
                StatusMessage = $"Zuletzt aktualisiert: {DateTime.Now:HH:mm:ss} ({Readings.Count} Werte)";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler beim Laden der Sensordaten von der API: {ex.Message}";
            }
        }
    }
}
