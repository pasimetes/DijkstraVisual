using DijkstraVisualization.Models.Enums;

namespace DijkstraVisualization.Events
{
    public class OnRunAlgorithmMessage
    {
        public OnRunAlgorithmMessage(string sourceId, Algorithm algorithm)
        {
            Algorithm = algorithm;
            SourceId = sourceId;
        }

        public string SourceId { get; }

        public Algorithm Algorithm { get; }
    }
}
