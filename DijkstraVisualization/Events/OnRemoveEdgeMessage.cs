namespace DijkstraVisualization.Events
{
    public class OnRemoveEdgeMessage
    {
        public OnRemoveEdgeMessage(string sourceId, string destinationId)
        {
            SourceId = sourceId;
            DestinationId = destinationId;
        }

        public string SourceId { get; }

        public string DestinationId { get; }
    }
}
