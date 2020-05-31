using System.Windows;

namespace ImageClusteringUI.Services
{
    public interface IWindowService
    {
        void ShowWindow<T>(object dataContext) where T : Window, new();
    }
}