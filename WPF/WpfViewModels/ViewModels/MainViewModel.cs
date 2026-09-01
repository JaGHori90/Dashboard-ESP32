
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WpfViewModels.Common;
using ApiClient.Dtos;

namespace WpfViewModels.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(IWindowController windowController) : base(windowController)
        {
        }

        public override async Task InitializeDataAsync()
        {
            await LoadAllSensorsAsync();
        }

        private async Task LoadAllSensorsAsync()
        {
            
        }

        private ObservableCollection<SensorDto> _Sensors;

        public ObservableCollection<SensorDto> Sensors
        {
            get { return _Sensors; }
            set { _Sensors = value; }
        }

    }
}
