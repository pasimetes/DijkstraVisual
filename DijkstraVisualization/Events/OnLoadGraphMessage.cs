using DijkstraVisualization.Models.Dto;

namespace DijkstraVisualization.Events
{
    public class OnLoadGraphMessage
    {
        public OnLoadGraphMessage(IOGraphData data)
        {
            Data = data;
        }

        public IOGraphData Data { get; }
    }
}
