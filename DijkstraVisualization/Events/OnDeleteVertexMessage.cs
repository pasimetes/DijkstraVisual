namespace DijkstraVisualization.Events
{
    public class OnDeleteVertexMessage
    {
        public OnDeleteVertexMessage(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
