namespace DijkstraVisualization.Events
{
    public class OnCreateVertexMessage
    {
        public OnCreateVertexMessage(string id, int row, int column)
        {
            Id = id;
            Row = row;
            Column = column;
        }

        public string Id { get; }

        public int Row { get; }

        public int Column { get; }
    }
}
