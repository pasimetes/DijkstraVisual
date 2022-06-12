using DijkstraVisualization.Models;

namespace DijkstraVisualization.Events
{
    public class OnCompletedDijkstraMessage
    {
        public OnCompletedDijkstraMessage(ShortestPaths shortestPaths)
        {
            Result = shortestPaths;
        }

        public ShortestPaths Result { get; }
    }
}
