using System.Collections.Generic;

namespace DijkstraVisualization.Models
{
    public class ShortestPaths : Traversal
    {
        public ShortestPaths(
            IDictionary<string, int> distances,
            IDictionary<string, List<string>> visitLog,
            IDictionary<string, string> paths)
            : base(visitLog)
        {
            Distances = distances;
            Paths = paths;
        }

        public IDictionary<string, int> Distances { get; }

        public IDictionary<string, string> Paths { get; }
    }
}
