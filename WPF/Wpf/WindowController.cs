using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfViewModels.Common;
using WpfViewModels.ViewModels;

namespace Wpf
{
    public class WindowController : IWindowController
    {
        Dictionary<BaseViewModel, Window> _windows =new();
        public void CloseWindow(BaseViewModel viewModel)
        {
            if (_windows.ContainsKey(viewModel))
            {
                _windows[viewModel].Close();
                _windows.Remove(viewModel);
            }
        }

        public async Task ShowWindow(BaseViewModel viewModel)
        {
            Window? window = null;
            if (viewModel is SensorViewModel)
            {
                window = new SensorWindow();
            }
            else
            {
                throw new ArgumentException("ViewModel not supported");
            }

            _windows.Add(viewModel, window);

            window.DataContext = viewModel;
            await viewModel.InitializeDataAsync();
            window.ShowDialog();
        }
    }
}
