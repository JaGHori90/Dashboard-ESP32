using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfViewModels.Common
{
    public interface IWindowController
    {
        Task ShowWindow(BaseViewModel viewModel);
        void CloseWindow(BaseViewModel viewModel);
    }
}
