namespace DijkstraVisualization.Events
{
    public class OnLeaveVertex
    {
        public OnLeaveVertex(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
