using DijkstraVisualization.ViewModels;

namespace DijkstraVisualization.Models.UI
{
    public class VertexViewModel : BaseViewModel
    {
        public VertexViewModel(string id, int row, int column)
        {
            Id = id;
            Row = row;
            Column = column;
        }

        public string Id { get; }

        public int Row { get; }

        public int Column { get; }
    }
}
