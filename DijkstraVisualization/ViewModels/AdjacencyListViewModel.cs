using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.Extensions;
using DijkstraVisualization.Models;
using System;
using System.Collections.Generic;

namespace DijkstraVisualization.ViewModels
{
    public class AdjacencyListViewModel : BaseViewModel,
        IHandle<OnCreateVertexMessage>,
        IHandle<OnCreateEdgeMessage>,
        IHandle<OnCostChanged>,
        IHandle<OnDeleteVertexMessage>,
        IHandle<OnRemoveEdgeMessage>
    {
        private readonly Graph _graph;

        public AdjacencyListViewModel()
        {
            EventAggregator.Instance.Subsribe(this);
            _graph = Graph.Instance;
        }

        public IDictionary<string, ICollection<Tuple<string, int>>> ListRepresentation => _graph.ToAdjacencyList();

        public int VerticesCount => _graph.GetVerticesCount();

        public void Handle(OnCreateVertexMessage message)
        {
            OnPropertyChanged(nameof(ListRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
        }

        public void Handle(OnCreateEdgeMessage message)
        {
            OnPropertyChanged(nameof(ListRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
        }

        public void Handle(OnCostChanged message)
        {
            OnPropertyChanged(nameof(ListRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
        }

        public void Handle(OnDeleteVertexMessage message)
        {
            OnPropertyChanged(nameof(ListRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
        }

        public void Handle(OnRemoveEdgeMessage message)
        {
            OnPropertyChanged(nameof(ListRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
        }
    }
}
