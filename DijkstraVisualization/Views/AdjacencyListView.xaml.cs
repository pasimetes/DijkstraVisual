using DijkstraVisualization.ViewModels;
using System.Windows.Controls;

namespace DijkstraVisualization.Views
{
    public partial class AdjacencyListView : UserControl
    {
        public AdjacencyListView()
        {
            DataContext = new AdjacencyListViewModel();
            InitializeComponent();
        }
    }
}
