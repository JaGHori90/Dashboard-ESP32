using ApiClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using WpfViewModels.ViewModels;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var apiBaseUrl = config["ApiBaseUrl"]
                ?? throw new InvalidOperationException("ApiBaseUrl fehlt in appsettings.json.");
            var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
            ISensorReadingsApiClient apiClient = new SensorReadingsApiClient(httpClient);

            WindowController windowController = new WindowController();
            await windowController.ShowWindow(new SensorViewModel(windowController, apiClient));
        }
    }

}
