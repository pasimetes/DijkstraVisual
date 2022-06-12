namespace DijkstraVisualization.Events
{
    public class OnHoverVertex
    {
        public OnHoverVertex(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
