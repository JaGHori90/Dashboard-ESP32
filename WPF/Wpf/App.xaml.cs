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
            WindowController windowController = new WindowController();
            await windowController.ShowWindow(new MainViewModel(windowController));
        }
    }

}
