namespace DijkstraVisualization.Models
{
    public class Edge
    {
        public Edge(Vertex from, Vertex to, int cost)
        {
            From = from;
            To = to;
            Cost = cost;
        }

        public Vertex From { get; }

        public Vertex To { get; }

        public int Cost { get; }

        public override string ToString()
        {
            return $"{From} -> {To} = {Cost}";
        }
    }
}
