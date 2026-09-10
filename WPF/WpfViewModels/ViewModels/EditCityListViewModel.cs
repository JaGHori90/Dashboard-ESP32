using System.Collections.ObjectModel;
using WpfViewModels.Common;

namespace WpfViewModels.ViewModels
{
    public class EditCityListViewModel : BaseViewModel
    {
        private const int MaxCities = 5;

        public EditCityListViewModel(IWindowController windowController, ObservableCollection<string> SelectedCities) : base(windowController)
        {
            SelectCities = SelectedCities;
            SelectCities.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SubtitleText));

            AddCityCommand = new RelayCommand(o => AddCity(), o => !string.IsNullOrWhiteSpace(NewCityName) && SelectCities.Count < MaxCities);
            RemoveCityCommand = new RelayCommand(o => RemoveCity(o as string));
            CmdClose = new RelayCommand(o => WindowController.CloseWindow(this));
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

        private string _newCityName = "";

        public string NewCityName
        {
            get { return _newCityName; }
            set { _newCityName = value; OnPropertyChanged(); }
        }

        public string SubtitleText => $"{SelectCities.Count} von {MaxCities} Städten · Min / Max heute";

        public RelayCommand AddCityCommand { get; }
        public RelayCommand RemoveCityCommand { get; }
        public RelayCommand CmdClose { get; }

        private void AddCity()
        {
            var name = NewCityName.Trim();
            if (string.IsNullOrWhiteSpace(name) || SelectCities.Count >= MaxCities) return;
            SelectCities.Add(name);
            NewCityName = "";
        }

        private void RemoveCity(string? city)
        {
            if (city != null) SelectCities.Remove(city);
        }
    }
}
