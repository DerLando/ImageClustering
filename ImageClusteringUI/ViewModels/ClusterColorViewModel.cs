using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using ImageClusteringUI.Commands;
using ImageClusteringUI.Services;
using ImageClusteringUI.Views;

namespace ImageClusteringUI.ViewModels
{
    public class ClusterColorViewModel : ViewModelBase
    {
        private Color _color;

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                RaisePropertyChanged(nameof(Color));
                RaisePropertyChanged(nameof(Brush));
            }
        }

        public Brush Brush
        {
            get => new SolidColorBrush(Color);
        }

        private int[] _superPixelIndices;

        public int[] SuperPixelIndices
        {
            get => _superPixelIndices;
        }

        public ClusterColorViewModel(Color color, IEnumerable<int> indices)
        {
            _color = color;
            _superPixelIndices = indices.ToArray();
            OpenColorPickerCommand = new RelayCommand(o => ClusterColorButtonClicked(null));
        }

        public ICommand OpenColorPickerCommand { get; }

        public void ClusterColorButtonClicked(object sender)
        {
            new WindowService().ShowWindow<SelectClusterColorWindow>(this);
        }
    }
}
