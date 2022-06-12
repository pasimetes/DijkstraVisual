namespace DijkstraVisualization.Models
{
    public class Vertex
    {
        public Vertex(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public override string ToString()
        {
            return Id;
        }
    }
}
