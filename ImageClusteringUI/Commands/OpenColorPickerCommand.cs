using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageClusteringUI.Services;
using ImageClusteringUI.Views;

namespace ImageClusteringUI.Commands
{
    public class OpenColorPickerCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            new WindowService().ShowWindow<SelectClusterColorWindow>(parameter);
        }
    }
}
