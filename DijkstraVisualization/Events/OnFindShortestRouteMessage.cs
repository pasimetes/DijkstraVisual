namespace DijkstraVisualization.Events
{
    public class OnFindShortestRouteMessage
    {
        public OnFindShortestRouteMessage(string destinationId)
        {
            DestinationId = destinationId;
        }

        public string DestinationId { get; }
    }
}
