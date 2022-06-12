using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.Util;
using System.Windows.Input;

namespace DijkstraVisualization.ViewModels
{
    public class CostLabelViewModel : BaseViewModel
    {
        private int _cost;

        public CostLabelViewModel(string sourceId, string destinationId, int cost)
        {
            SourceId = sourceId;
            DestinationId = destinationId;

            _cost = cost;

            IncrementCostCommand = new RelayCommand(IncrementCost, (obj) => Cost < 99);
            DecrementCostCommand = new RelayCommand(DecrementCost, (obj) => true);
        }

        public ICommand IncrementCostCommand { get; }

        public ICommand DecrementCostCommand { get; }

        public string SourceId { get; }

        public string DestinationId { get; }

        public int Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                OnPropertyChanged();

                EventAggregator.Instance.Publish(
                    new OnCostChanged(
                        SourceId,
                        DestinationId,
                        value));
            }
        }

        private void IncrementCost(object sender)
        {
            Cost++;
        }

        private void DecrementCost(object sender)
        {
            if (Cost - 1 == 0)
            {
                EventAggregator.Instance.Publish(
                    new OnRemoveEdgeMessage(
                        SourceId,
                        DestinationId));
                return;
            }
            Cost--;
        }
    }
}
