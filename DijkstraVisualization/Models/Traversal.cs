using System.Collections.Generic;

namespace DijkstraVisualization.Models
{
    public class Traversal
    {
        public Traversal(IDictionary<string, List<string>> visitLog)
        {
            VisitLog = visitLog;
        }

        public IDictionary<string, List<string>> VisitLog { get; }
    }
}
