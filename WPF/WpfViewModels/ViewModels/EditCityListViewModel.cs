using System.Collections.ObjectModel;
using WpfViewModels.Common;

namespace WpfViewModels.ViewModels
{
    public class EditCityListViewModel : BaseViewModel
    {
        public EditCityListViewModel(IWindowController windowController,ObservableCollection<string> SelectedCities) : base(windowController)
        {
            SelectCities = SelectedCities;
        }

        public override Task InitializeDataAsync()
        {
            return Task.CompletedTask; 
        }

        private ObservableCollection<string> _selectCities = new();

        public ObservableCollection<string> SelectCities
        {
            get { return _selectCities; }
            set { _selectCities = value; OnPropertyChanged(); }
        }

    }
}