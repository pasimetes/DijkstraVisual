using DijkstraVisualization.Algorithms;
using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.Models;
using DijkstraVisualization.Models.Enums;

namespace DijkstraVisualization.ViewModels
{
    public class GraphViewModel : BaseViewModel,
        IHandle<OnCreateVertexMessage>,
        IHandle<OnCreateEdgeMessage>,
        IHandle<OnDeleteVertexMessage>,
        IHandle<OnCostChanged>,
        IHandle<OnRemoveEdgeMessage>,
        IHandle<OnRunAlgorithmMessage>
    {
        private readonly Graph _graph;

        public GraphViewModel()
        {
            _graph = Graph.Instance;
        }

        public void Handle(OnCreateVertexMessage message)
        {
            var vertex = new Vertex(message.Id);
            _graph.AddVertex(vertex);
        }

        public void Handle(OnDeleteVertexMessage message)
        {
            var vertex = _graph.GetVertexById(message.Id);
            _graph.RemoveVertex(vertex);
        }

        public void Handle(OnCreateEdgeMessage message)
        {
            var source = _graph.GetVertexById(message.SourceId);
            var destination = _graph.GetVertexById(message.DestinationId);
            _graph.AddEdge(source, destination, message.Cost);
        }

        public void Handle(OnCostChanged message)
        {
            var source = _graph.GetVertexById(message.SourceId);
            var destination = _graph.GetVertexById(message.DestinationId);
            _graph.UpdateCost(source, destination, message.Cost);
        }

        public void Handle(OnRemoveEdgeMessage message)
        {
            var source = _graph.GetVertexById(message.SourceId);
            var destination = _graph.GetVertexById(message.DestinationId);
            _graph.RemoveEdge(source, destination);
        }

        public void Handle(OnRunAlgorithmMessage message)
        {
            switch (message.Algorithm)
            {
                case Algorithm.Dijkstra:

                    var startFrom = _graph.GetVertexById(message.SourceId);
                    var shortestPaths = new Dijkstra(_graph).CalculateShortestPaths(startFrom);

                    EventAggregator.Instance.Publish(new OnCompletedDijkstraMessage(shortestPaths));

                    break;

                default: break;
            }
        }
    }
}
