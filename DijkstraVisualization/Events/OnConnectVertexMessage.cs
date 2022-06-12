namespace DijkstraVisualization.Events
{
    public class OnConnectVertexMessage
    {
        public OnConnectVertexMessage(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
