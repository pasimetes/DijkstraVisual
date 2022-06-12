using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.Extensions;
using DijkstraVisualization.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DijkstraVisualization.ViewModels
{
    public class AdjacencyMatrixViewModel : BaseViewModel,
        IHandle<OnCreateVertexMessage>,
        IHandle<OnCreateEdgeMessage>,
        IHandle<OnCostChanged>,
        IHandle<OnDeleteVertexMessage>,
        IHandle<OnRemoveEdgeMessage>
    {
        private readonly Graph _graph;

        public AdjacencyMatrixViewModel()
        {
            EventAggregator.Instance.Subsribe(this);

            _graph = Graph.Instance;
        }

        public IList<string> MatrixRepresentation => To1DArray(_graph.ToAdjacencyMatrix());

        public IList<Vertex> Vertices => _graph.GetAllVertices();

        public int VerticesCount => _graph.GetVerticesCount() + 1;

        public void Handle(OnCreateVertexMessage message)
        {
            OnPropertyChanged(nameof(MatrixRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
            OnPropertyChanged(nameof(Vertices));
        }

        public void Handle(OnCreateEdgeMessage message)
        {
            OnPropertyChanged(nameof(MatrixRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
        }

        public void Handle(OnCostChanged message)
        {
            OnPropertyChanged(nameof(MatrixRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
        }

        public void Handle(OnDeleteVertexMessage message)
        {
            OnPropertyChanged(nameof(MatrixRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
            OnPropertyChanged(nameof(Vertices));
        }

        public void Handle(OnRemoveEdgeMessage message)
        {
            OnPropertyChanged(nameof(MatrixRepresentation));
            OnPropertyChanged(nameof(VerticesCount));
        }

        private List<string> To1DArray(int[,] input)
        {
            if (VerticesCount == 1)
                return new List<string>();

            var result = new List<string>();
            result.Add("");
            foreach (var vertex in Vertices)
                result.Add(vertex.Id);

            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                result.Add(result[i + 1]);

                for (int j = 0; j <= input.GetUpperBound(1); j++)
                {
                    result.Add(input[i, j].ToString());
                }
            }
            // Step 3: return the new array.
            return result;
        }
    }
}
