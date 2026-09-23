using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfViewModels.Common
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected IWindowController WindowController { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public abstract Task InitializeDataAsync();

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName=null)
        {
            if (PropertyChanged != null) 
            { 
              PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetProperty<T>
            (ref T member, T value, [CallerMemberName] string? propertyName=null)
        {
            if (object.Equals(member, value) || propertyName == null)
                return;
            member = value;
            OnPropertyChanged(propertyName);
        }

        public BaseViewModel(IWindowController windowController)
        {
            this.WindowController = windowController;
        }
    }
}
