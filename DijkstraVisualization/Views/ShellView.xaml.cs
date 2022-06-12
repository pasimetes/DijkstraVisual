using DijkstraVisualization.ViewModels;
using System.Windows;

namespace DijkstraVisualization.Views
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            DataContext = new ShellViewModel();
            InitializeComponent();
        }
    }
}