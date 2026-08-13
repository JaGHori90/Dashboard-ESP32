using Core.Entities;
using Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfViewModels.Common;

namespace WpfViewModels.ViewModels
{
    public class ActivityViewModel : BaseViewModel
    {
        Sensor _actEmp;

        private ObservableCollection<Activity> _activites = new();

        public ObservableCollection<Activity> Activities
        {
            get { return _activites; }
            set
            {
                _activites = value;
                OnPropertyChanged();
            }
        }

        //Inhalt der ComboBox Mitarbeiterbeiterauswahl
        private ObservableCollection<Sensor> _employees = new();

        public ObservableCollection<Sensor> Employees
        {
            get { return _employees; }
            set { 
                SetProperty(ref _employees, value);
            }
        }

        //Aktive Auswahl in Combo-Box Mitarbeiter
        private Sensor? _selectedEmp;

        public Sensor? SelectedEmp
        {
            get { return _selectedEmp; }
            set {
                SetProperty(ref _selectedEmp, value);
                _ = LoadActivities();
               
            }
        }


        public ActivityViewModel(IWindowController windowController, Sensor actEmp) : base(windowController)
        {
            _actEmp = actEmp;
        }

        public async override Task InitializeDataAsync()
        {
            await LoadActivities();
            await LoadEmployees();

            SelectedEmp = Employees.SingleOrDefault(emp=>emp.Id==_actEmp.Id);
        }

        private async Task LoadEmployees()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                Employees = new ObservableCollection<Sensor>
                    (await uow.EmployeeRepository.GetAllAsync());

            }
        }

        public async Task LoadActivities()
        {
            if (SelectedEmp != null)
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    List<Activity> activities = await uow.ActivityRepository.GetAllByEmpIdAsync(SelectedEmp.Id);
                    Activities = new ObservableCollection<Activity>(activities);
                }
            }

        }
    }
}
