using Core.Contracts;
using Core.Entities;
using Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WpfViewModels.Common;

namespace WpfViewModels.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _firstName = string.Empty;  //Eingabefeld Vorname
        private string _lastName = string.Empty;   //Eingabefeld Nachname

        private ObservableCollection<Sensor> _employees = new(); //Liste der Mitarbeiter

        public string FirstName
        {
            get { return _firstName; }
            set {
                    _firstName = value;
                    OnPropertyChanged();
            }
        }


        public string LastName
        {
            get { return _lastName; }
            set { 

                SetProperty(ref _lastName, value);
            }
        }

        

        public ObservableCollection<Sensor> Employees
        {
            get { return _employees; }
            set { _employees = value;
              OnPropertyChanged();
            }
        }

        private Sensor? _selectedEmp;

        public Sensor? SelectedEmp
        {
            get { return _selectedEmp; }
            set { 
                _selectedEmp = value;
                
                //FirstName = (_selectedEmp?.FirstName)??"";
                //LastName = (_selectedEmp?.LastName) ?? "";

                //Alternativ zu obigen zwei Zeilen
                if (_selectedEmp!=null)
                {
                    FirstName = _selectedEmp.FirstName;
                    LastName = _selectedEmp.LastName;
                }
                else
                {
                    FirstName = "";
                    LastName = "";
                }
                OnPropertyChanged();
            }
        }



        public async override Task InitializeDataAsync()
        {
            await LoadEmployeesAsync();
        }

        public async Task LoadEmployeesAsync()
        {
            using IUnitOfWork uow = new UnitOfWork();
            var emps = await uow.EmployeeRepository.GetAllAsync();
            Employees = new ObservableCollection<Sensor>(emps);

        }

        //Commands
        public RelayCommand CmdSaveChanges { get; set; }
        public RelayCommand CmdEditActivities { get; set; }

        public MainViewModel(IWindowController windowController): base(windowController) 
        {
            CmdSaveChanges = new RelayCommand(
                  async obj => {
                      if (SelectedEmp!=null)
                      {
                          using (IUnitOfWork uow = new UnitOfWork())
                          {
                              SelectedEmp.FirstName = _firstName;
                              SelectedEmp.LastName = _lastName;
                              uow.EmployeeRepository.Update(SelectedEmp);
                              await uow.SaveChangesAsync();
                              await LoadEmployeesAsync();
                          }
                      }
                  },
                  obj =>
                  {
                      return (SelectedEmp != null) && (LastName!="");
                  }
                ) ;

            CmdEditActivities = new RelayCommand(
                  obj => {
                      
                      if (SelectedEmp != null)
                      {
                          WindowController.ShowWindow(new ActivityViewModel(windowController, SelectedEmp));
                      }
                  },
                  obj => { return SelectedEmp!=null; }


                );
        }
    }
}
