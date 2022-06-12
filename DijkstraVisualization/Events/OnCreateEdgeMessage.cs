namespace DijkstraVisualization.Events
{
    public class OnCreateEdgeMessage
    {
        public OnCreateEdgeMessage(string sourceId, string destinationId, int cost)
        {
            SourceId = sourceId;
            DestinationId = destinationId;
            Cost = cost;
        }

        public string SourceId { get; }

        public string DestinationId { get; }

        public int Cost { get; }
    }
}
