using System.Configuration;
using System.Data;
using System.Windows;
using WpfViewModels.ViewModels;
using ApiClient.Dtos;
using ApiClient.Contracts;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Net.Http;
using ApiClient.ApiClient;
using Microsoft.Extensions.Configuration.Json; // Add this using
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            _ = CheckForUpdatesAsync();

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(),optional: true)
                .Build();

            var apiBaseUrl = config["ApiBaseUrl"]
                ?? throw new InvalidOperationException("ApiBaseUrl fehlt in appsettings.json.");

            var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };

            // Schreibende Endpunkte (z.B. Cleanup) verlangen einen API-Key im Header.
            // Wert kommt aus User Secrets ("dotnet user-secrets set ApiKey ..." in WPF/Wpf),
            // steht nie im Repo. Ohne gesetzten Key bleibt der Header einfach weg -
            // die schreibenden Aufrufe liefern dann 401, bis der Key konfiguriert ist.
            var apiKey = config["ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            }

            ISensorApiClient sensorApiClient = new SensorApiClient(httpClient);
            IMeasurmentApiClient measurmentApiClient = new MeasurmentApiClient(httpClient);
            ICityWeatherApiClient cityWeatherApiClient = new CityWeatherApiClient(httpClient);

            WindowController windowController = new WindowController();
            await windowController.ShowWindow(new MainViewModel(windowController, sensorApiClient, measurmentApiClient,cityWeatherApiClient));
        }

        private static async Task CheckForUpdatesAsync()
        {
            try
            {
                var updateManager = new UpdateManager(
                    new GithubSource("https://github.com/JaGHori90/Dashboard-ESP32", null, false));

                var newVersion = await updateManager.CheckForUpdatesAsync();
                if (newVersion == null) return;

                await updateManager.DownloadUpdatesAsync(newVersion);
                updateManager.ApplyUpdatesAndRestart(newVersion);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update-Check fehlgeschlagen: {ex.Message}");
            }
        }
    }

}
