namespace DijkstraVisualization.Events
{
    public class OnHoverCostLabel
    {
        public OnHoverCostLabel(string sourceId, string destinationId)
        {
            SourceId = sourceId;
            DestinationId = destinationId;
        }

        public string SourceId { get; }

        public string DestinationId { get; }
    }
}
