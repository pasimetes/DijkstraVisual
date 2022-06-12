using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.Extensions;
using DijkstraVisualization.Models;
using DijkstraVisualization.Models.Dto;
using DijkstraVisualization.Models.Enums;
using DijkstraVisualization.Services;
using DijkstraVisualization.Util;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace DijkstraVisualization.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        private readonly Graph _graph;
        private readonly VertexPersistenceManager _vertexPersistenceManager;
        private readonly CollectionView _views;

        private ActiveView _activeView;

        public ShellViewModel()
        {
            _graph = Graph.Instance;
            _vertexPersistenceManager = new VertexPersistenceManager();
            ActiveView = ActiveView.Graph;

            ViewAdjacencyMatrixCommand = new RelayCommand(
                obj => ActiveView = ActiveView.AdjacencyMatrix,
                obj => ActiveView != ActiveView.AdjacencyMatrix);

            ViewAdjacencyListCommand = new RelayCommand(
                obj => ActiveView = ActiveView.AdjacencyList,
                obj => ActiveView != ActiveView.AdjacencyList);

            ViewGraphCommand = new RelayCommand(
                obj => ActiveView = ActiveView.Graph,
                obj => ActiveView != ActiveView.Graph);

            SaveGraphCommand = new RelayCommand(
                obj => SaveToFile(),
                obj => _graph.GetVerticesCount() > 0);

            LoadGraphCommand = new RelayCommand(
                obj => LoadFromFile(),
                obj => true);
        }

        public ICommand ViewAdjacencyMatrixCommand { get; }

        public ICommand ViewAdjacencyListCommand { get; }

        public ICommand ViewGraphCommand { get; }

        public ICommand SaveGraphCommand { get; }

        public ICommand LoadGraphCommand { get; }

        public ActiveView ActiveView
        {
            get => _activeView;
            set
            {
                _activeView = value;
                OnPropertyChanged();
            }
        }

        private void SaveToFile()
        {
            var exportObject = new IOGraphData
            {
                Vertices = _vertexPersistenceManager.GetPersistedVertices().ToList(),
                AdjacencyList = _graph.ToAdjacencyList()
            };

            var json = JsonConvert.SerializeObject(exportObject);
            File.WriteAllText("savedData.json", json);
        }

        private void LoadFromFile()
        {
            var json = File.ReadAllText("savedData.json");
            var importObject = JsonConvert.DeserializeObject<IOGraphData>(json);
            EventAggregator.Instance.Publish(new OnLoadGraphMessage(importObject));
        }
    }
}
