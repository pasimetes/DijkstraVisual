using DijkstraVisualization.Views;
using System.Windows.Shapes;

namespace DijkstraVisualization.ViewModels
{
    public class ConnectionArrow
    {
        public ConnectionArrow(Line line, VertexView source, VertexView destination)
        {
            Line = line;
            Source = source;
            Destination = destination;
        }

        public Line Line { get; }

        public VertexView Source { get; }

        public VertexView Destination { get; }

        public override string ToString()
        {
            return Source.Id + " -> " + Destination.Id;
        }
    }
}
