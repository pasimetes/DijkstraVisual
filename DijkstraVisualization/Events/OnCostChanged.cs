namespace DijkstraVisualization.Events
{
    public class OnCostChanged
    {
        public OnCostChanged(string sourceId, string destinationId, int cost)
        {
            Cost = cost;
            SourceId = sourceId;
            DestinationId = destinationId;
        }

        public int Cost { get; }

        public string SourceId { get; }

        public string DestinationId { get; }
    }
}
