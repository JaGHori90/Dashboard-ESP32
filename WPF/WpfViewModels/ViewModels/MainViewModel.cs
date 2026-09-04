using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WpfViewModels.Common;
using ApiClient.Dtos;
using ApiClient.Contracts;
using System.Globalization;
using OxyPlot.Series;
using OxyPlot.Wpf;
using LiveChartsCore.SkiaSharpView;
using Axis = LiveChartsCore.SkiaSharpView.Axis;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Windows.Ink;
using LiveChartsCore;


namespace WpfViewModels.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static readonly CultureInfo De = CultureInfo.GetCultureInfo("de-DE");

        private readonly ISensorApiClient _sensorApiClient;
        private readonly IMeasurmentApiClient _measurmentApiClient;
        public MainViewModel(IWindowController windowController, ISensorApiClient sensorApiClient, IMeasurmentApiClient measurmentApiClient) : base(windowController)
        {
            _sensorApiClient = sensorApiClient;
            _measurmentApiClient = measurmentApiClient;
        }

        public override async Task InitializeDataAsync()
        {
            await LoadAllSensorsAsync();
            await LoadAllMeasurmentAsync();
        }

        private async Task LoadAllMeasurmentAsync()
        {
            MeasurmentsDtos = new ObservableCollection<MeasurmentDto>(await _measurmentApiClient.GetAllAsync());
            RecalculateDashboard();
        }

        private async Task LoadAllSensorsAsync()
        {
            Sensors = new ObservableCollection<SensorDto>(await _sensorApiClient.GetAllAsync());
        }

        private ObservableCollection<SensorDto> _sensors = new();

        public ObservableCollection<SensorDto> Sensors
        {
            get { return _sensors; }
            set { _sensors = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MeasurmentDto> _measurmentDtos = new();

        public ObservableCollection<MeasurmentDto> MeasurmentsDtos
        {
            get { return _measurmentDtos; }
            set { _measurmentDtos = value;
                OnPropertyChanged();
            }
        }



        // Curent values

        private string _temperatureText = "-";

        public string TemperatureText
        {
            get { return _temperatureText; }
            set { _temperatureText = value; OnPropertyChanged(); }
        }

        private string _humidityText = "-";

        public string HumidityText
        {
            get { return _humidityText; }
            set { _humidityText = value; OnPropertyChanged(); }
        }

        private string _airPressureText = "-";

        public string AirPressureText
        {
            get { return _airPressureText; }
            set { _airPressureText = value; OnPropertyChanged(); }
        }

        private string _temperatureDeltaText = "x";

        public string TemperatureDeltaText
        {
            get { return _temperatureDeltaText; }
            set { _temperatureDeltaText = value; OnPropertyChanged(); }
        }

       

        // chart header stats

        private string _tempMinText ="";

        public string TempMinText
        {
            get { return _tempMinText; }
            set { _tempMinText = value; OnPropertyChanged(); }
        }

        private string _tempMaxText = "";

        public string TempMaxText
        {
            get { return _tempMaxText; }
            set { _tempMaxText = value; OnPropertyChanged(); }
        }

        private string _tempDeltaDodayText = "";

        public string TempDeltaTodayText
        {
            get { return _tempDeltaDodayText; }
            set { _tempDeltaDodayText = value; OnPropertyChanged(); }
        }

        // Chart series

        private ISeries[] _temperatureSeries = Array.Empty<ISeries>();

        public ISeries[] TemperatureSeries
        {
            get { return _temperatureSeries; }
            set { _temperatureSeries = value; OnPropertyChanged(); }
        }

        private Axis[] _xAxes = Array.Empty<Axis>();

        public Axis[] XAxes
        {
            get { return _xAxes; }
            set { _xAxes = value; OnPropertyChanged(); }
        }

        private Axis[] _yAxes = Array.Empty<Axis>();

        public Axis[] YAxes
        {
            get { return _yAxes; }
            set { _yAxes = value; OnPropertyChanged(); }
        }

        // Calulation

        private void RecalculateDashboard()
        {
            if (MeasurmentsDtos.Count == 0) return;

            var ordered = MeasurmentsDtos.OrderBy(m => m.MeasuredAt).ToList();
            var latest = ordered.Last();

            TemperatureText = latest.Temperature.ToString("0.0",De);
            HumidityText = latest.Humidity.ToString("0.0",De);
            AirPressureText = latest.AirPressure.ToString("0.0",De);

            var todayStartLocal = DateTime.Now.Date;
            var todayStartUtc = todayStartLocal.ToUniversalTime();
            var today = ordered.Where(m=>m.MeasuredAt >= todayStartUtc).ToList();
            if (today.Count == 0) today = ordered.TakeLast(1).ToList();

            var temps = today.Select(m => m.Temperature).ToList();
            var min = temps.Min();
            var max = temps.Max();
            var deltaToday = temps.Last() - temps.First();

            TempMinText = min.ToString("0.0", De);
            TempMaxText = max.ToString("0.0",De);
            TempDeltaTodayText = (deltaToday >= 0 ? "+" : "") + deltaToday.ToString("0.0", De);


            TemperatureSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = temps,
                    Fill = new LinearGradientPaint(
                        new[] { new SKColor(52, 26, 184, 90), new SKColor(52, 216, 184, 0) },
                        new SKPoint(0, 0), new SKPoint(0, 1)
                    ),
                    Stroke = new SolidColorPaint(new SKColor(52, 216, 184))
                    {
                        StrokeThickness = 3,              
                    },
                     GeometrySize = 0,
                     LineSmoothness = 0.65f
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = today.Select(m=>m.MeasuredAt.ToString("HH:mm")).ToArray(),
                    TextSize = 11,
                    LabelsPaint = new SolidColorPaint(new SKColor(140,150,165)),
                    SeparatorsPaint = null
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    TextSize = 11,
                    LabelsPaint = new SolidColorPaint(new SKColor(140,150,165)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(30,38,54))
                }
            };
        }

       
    }
}
