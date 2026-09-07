using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using WpfViewModels.Common;
using ApiClient.Dtos;
using ApiClient.Contracts;
using System.Globalization;
using LiveChartsCore.SkiaSharpView;
using Axis = LiveChartsCore.SkiaSharpView.Axis;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore;

namespace WpfViewModels.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static readonly CultureInfo De = CultureInfo.GetCultureInfo("de-DE");

        private static readonly SKColor AccentTemperature = new(52, 216, 184);
        private static readonly SKColor AxisText = new(107, 114, 128);
        private static readonly SKColor GridLine = new(31, 41, 55);
        private static readonly SKColor CardBackground = new(17, 24, 39);

        private static readonly TimeSpan AutoRefreshInterval = TimeSpan.FromSeconds(900);

        private readonly ISensorApiClient _sensorApiClient;
        private readonly IMeasurmentApiClient _measurmentApiClient;
        private readonly ICityWeatherApiClient _cityWeatherApiClient;

        private DispatcherTimer? _autoRefreshTimer;

        public MainViewModel(IWindowController windowController, ISensorApiClient sensorApiClient, IMeasurmentApiClient measurmentApiClient, ICityWeatherApiClient cityWeatherApiClient) : base(windowController)
        {
            _sensorApiClient = sensorApiClient;
            _measurmentApiClient = measurmentApiClient;
            _cityWeatherApiClient = cityWeatherApiClient;
        }

        public override async Task InitializeDataAsync()
        {
            await LoadAllSensorsAsync();
            await LoadAllMeasurmentAsync();
            StartAutoRefresh();
        }

        private void StartAutoRefresh()
        {
            _autoRefreshTimer = new DispatcherTimer { Interval = AutoRefreshInterval };
            _autoRefreshTimer.Tick += async (_, _) => await LoadAllMeasurmentAsync();
            _autoRefreshTimer.Start(); 
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

        // Header

        private string _headerText = "Sensor · aktuellste Werte";

        public string HeaderText
        {
            get { return _headerText; }
            set { _headerText = value; OnPropertyChanged(); }
        }

        private string _updatedAtText = "-";

        public string UpdatedAtText
        {
            get { return _updatedAtText; }
            set { _updatedAtText = value; OnPropertyChanged(); }
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

        private string _temperatureDeltaText = "-";

        public string TemperatureDeltaText
        {
            get { return _temperatureDeltaText; }
            set { _temperatureDeltaText = value; OnPropertyChanged(); }
        }

        private string _humidityDeltaText = "-";

        public string HumidityDeltaText
        {
            get { return _humidityDeltaText; }
            set { _humidityDeltaText = value; OnPropertyChanged(); }
        }

        private string _airPressureDeltaText = "-";

        public string AirPressureDeltaText
        {
            get { return _airPressureDeltaText; }
            set { _airPressureDeltaText = value; OnPropertyChanged(); }
        }

        private string _temperatureRatingText = "-";

        public string TemperatureRatingText
        {
            get { return _temperatureRatingText; }
            set { _temperatureRatingText = value; OnPropertyChanged(); }
        }

        private string _humidityRatingText = "-";

        public string HumidityRatingText
        {
            get { return _humidityRatingText; }
            set { _humidityRatingText = value; OnPropertyChanged(); }
        }

        private string _airPressureRatingText = "-";

        public string AirPressureRatingText
        {
            get { return _airPressureRatingText; }
            set { _airPressureRatingText = value; OnPropertyChanged(); }
        }

        // chart header stats

        private string _chartSubtitleText = "stündliche Messwerte heute";

        public string ChartSubtitleText
        {
            get { return _chartSubtitleText; }
            set { _chartSubtitleText = value; OnPropertyChanged(); }
        }

        private string _tempMinText = "";

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

        private string _tempAvgText = "";

        public string TempAvgText
        {
            get { return _tempAvgText; }
            set { _tempAvgText = value; OnPropertyChanged(); }
        }

        private string _tempDeltaDodayText = "";

        public string TempDeltaTodayText
        {
            get { return _tempDeltaDodayText; }
            set { _tempDeltaDodayText = value; OnPropertyChanged(); }
        }
        // Wolrd Weather

        private string _city1NameText
            = "";

        public string City1Name
        {
            get { return _city1NameText; }
            set { _city1NameText = value; OnPropertyChanged(); }
        }

        private string _city1MinTempText;

        public string City1MinText
        {
            get { return _city1MinTempText; }
            set { _city1MinTempText = value; }
        }

        private string _city1MaxTempText;

        public string City1MaxTempText
        {
            get { return _city1MaxTempText; }
            set { _city1MaxTempText = value; OnPropertyChanged(); }
        }




        //

        public void CallWeatherApi()
        {

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

        // Calculation

        private void RecalculateDashboard()
        {
            if (MeasurmentsDtos.Count == 0) return;

            var ordered = MeasurmentsDtos.OrderBy(m => m.MeasuredAt).ToList();
            var latest = ordered.Last();

            var sensorName = Sensors.FirstOrDefault(s => s.Id == latest.SensorId)?.Name ?? "Sensor";
            HeaderText = $"{sensorName} · aktuellste Werte";
            ChartSubtitleText = $"{sensorName} · stündliche Messwerte heute";
            UpdatedAtText = latest.MeasuredAt.ToLocalTime().ToString("HH:mm", De);

            TemperatureText = latest.Temperature.ToString("0.0", De);
            HumidityText = latest.Humidity.ToString("0.0", De);
            AirPressureText = latest.AirPressure.ToString("0.0", De);

            TemperatureRatingText = RateTemperature(latest.Temperature);
            HumidityRatingText = RateHumidity(latest.Humidity);
            AirPressureRatingText = RatePressure(latest.AirPressure);

            // Heutiger Kalendertag (lokale Zeit) statt rollierendem Fenster: kurz nach
            // Mitternacht gibt es entsprechend wenige/keine Punkte - das ist dann korrekt so.
            var todayStartLocal = latest.MeasuredAt.ToLocalTime().Date;
            var window = ordered.Where(m => m.MeasuredAt.ToLocalTime() >= todayStartLocal).ToList();
            if (window.Count == 0)
            {
                window = ordered.TakeLast(1).ToList();
            }

            var oneHourAgo = latest.MeasuredAt.AddHours(-1);
            var baseline = window.LastOrDefault(m => m.MeasuredAt <= oneHourAgo) ?? window.First();

            TemperatureDeltaText = FormatHourDelta(latest.Temperature - baseline.Temperature, "°C");
            HumidityDeltaText = FormatHourDelta(latest.Humidity - baseline.Humidity, "%");
            AirPressureDeltaText = FormatHourDelta(latest.AirPressure - baseline.AirPressure, "hPa");

            // Zu Stundenwerten aggregieren: hält die X-Achse lesbar (ein Label pro Stunde
            // statt vieler fast identischer Zeitstempel) und passt zu "stündliche Messwerte".
            var buckets = window
                .Select(m => new { Local = m.MeasuredAt.ToLocalTime(), m.Temperature })
                .GroupBy(x => new DateTime(x.Local.Year, x.Local.Month, x.Local.Day, x.Local.Hour, 0, 0))
                .OrderBy(g => g.Key)
                .Select(g => new { Hour = g.Key, Temp = g.Average(x => x.Temperature) })
                .ToList();

            var chartTemps = buckets.Select(b => b.Temp).ToList();
            var min = chartTemps.Min();
            var max = chartTemps.Max();
            var avg = chartTemps.Average();
            var delta24h = chartTemps.Last() - chartTemps.First();

            TempMinText = min.ToString("0.0", De);
            TempMaxText = max.ToString("0.0", De);
            TempAvgText = avg.ToString("0.0", De);
            TempDeltaTodayText = (delta24h >= 0 ? "+" : "") + delta24h.ToString("0.0", De);

            TemperatureSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = chartTemps,
                    Fill = new LinearGradientPaint(
                        new[] { AccentTemperature.WithAlpha(90), AccentTemperature.WithAlpha(0) },
                        new SKPoint(0, 0), new SKPoint(0, 1)
                    ),
                    Stroke = new SolidColorPaint(AccentTemperature) { StrokeThickness = 3 },
                    GeometrySize = 0,
                    LineSmoothness = 0.65f,
                    IsHoverable = false
                },
                new LineSeries<double?>
                {
                    Values = BuildLastPointOnly(chartTemps),
                    Stroke = null,
                    Fill = null,
                    GeometrySize = 10,
                    GeometryFill = new SolidColorPaint(AccentTemperature),
                    GeometryStroke = new SolidColorPaint(CardBackground) { StrokeThickness = 2 },
                    IsHoverable = false
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = buckets.Select(b => b.Hour.ToString("HH:mm", De)).ToArray(),
                    TextSize = 11,
                    LabelsPaint = new SolidColorPaint(AxisText),
                    SeparatorsPaint = null
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    TextSize = 11,
                    LabelsPaint = new SolidColorPaint(AxisText),
                    SeparatorsPaint = new SolidColorPaint(GridLine) { StrokeThickness = 1 }
                }
            };
        }

        private static double?[] BuildLastPointOnly(List<double> values)
        {
            var result = new double?[values.Count];
            if (values.Count > 0)
            {
                result[^1] = values[^1];
            }
            return result;
        }

        private static string FormatHourDelta(double delta, string unit)
        {
            var sign = delta > 0 ? "+" : delta < 0 ? "" : "±";
            return $"{sign}{delta.ToString("0.0", De)} {unit} / 1 h";
        }

        // Grobe Einordnung für den Wohnraum-Komfort - keine wissenschaftliche Messung,
        // sondern eine Orientierung, damit man "41,5 % Luftfeuchtigkeit" einordnen kann.
        private static string RateTemperature(double celsius) => celsius switch
        {
            < 18 => "Kühl",
            < 20 => "Frisch",
            <= 24 => "Angenehm",
            <= 27 => "Warm",
            _ => "Zu warm"
        };

        private static string RateHumidity(double percent) => percent switch
        {
            < 30 => "Zu trocken",
            < 40 => "Trocken",
            <= 60 => "Angenehm",
            <= 70 => "Feucht",
            _ => "Zu feucht"
        };

        private static string RatePressure(double hPa) => hPa switch
        {
            < 1000 => "Tief",
            <= 1020 => "Normal",
            _ => "Hoch"
        };
    }
}
