using ApiClient;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
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
        private readonly ISensorReadingsApiClient _apiClient;

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

        private ISeries[] _temperatureSeries = Array.Empty<ISeries>();
        public ISeries[] TemperatureSeries
        {
            get { return _temperatureSeries; }
            set { SetProperty(ref _temperatureSeries, value); }
        }

        private Axis[] _xAxes = new[] { new Axis { Name = "Zeitpunkt" } };
        public Axis[] XAxes
        {
            get { return _xAxes; }
            set { SetProperty(ref _xAxes, value); }
        }

        private Axis[] _yAxes = new[] { new Axis { Name = "Temperatur (°C)" } };
        public Axis[] YAxes
        {
            get { return _yAxes; }
            set { SetProperty(ref _yAxes, value); }
        }

        public RelayCommand CmdRefresh { get; }

        public SensorViewModel(IWindowController windowController, ISensorReadingsApiClient apiClient)
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

                var chronological = readings.OrderBy(r => r.Timestamp).ToList();
                TemperatureSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Name = "Temperatur (°C)",
                        Values = chronological.Select(r => r.Temperature).ToArray(),
                        GeometrySize = 4
                    }
                };
                XAxes = new[]
                {
                    new Axis
                    {
                        Name = "Zeitpunkt",
                        Labels = chronological.Select(r => r.Timestamp.ToString("HH:mm:ss")).ToArray()
                    }
                };
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler beim Laden der Sensordaten von der API: {ex.Message}";
            }
        }
    }
}
