using System;
using System.Collections.Generic;

namespace DijkstraVisualization.Models.Dto
{
    public class IOGraphData
    {
        public ICollection<VertexWithLocation> Vertices { get; set; }

        public IDictionary<string, ICollection<Tuple<string, int>>> AdjacencyList { get; set; }
    }
}
