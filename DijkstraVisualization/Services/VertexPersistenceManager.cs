using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.Models.Dto;
using System.Collections.Generic;
using System.Linq;

namespace DijkstraVisualization.Services
{
    public class VertexPersistenceManager :
        IHandle<OnCreateVertexMessage>,
        IHandle<OnDeleteVertexMessage>
    {
        private readonly ICollection<VertexWithLocation> _vertices;

        public VertexPersistenceManager()
        {
            EventAggregator.Instance.Subsribe(this);

            _vertices = new List<VertexWithLocation>();
        }

        public IEnumerable<VertexWithLocation> GetPersistedVertices() => _vertices;

        public void Handle(OnDeleteVertexMessage message)
        {
            var vertexToRemove = _vertices.SingleOrDefault(v => v.Id == message.Id);
            _vertices.Remove(vertexToRemove);
        }

        public void Handle(OnCreateVertexMessage message)
        {
            _vertices.Add(
                new VertexWithLocation
                {
                    Id = message.Id,
                    Row = message.Row,
                    Column = message.Column
                });
        }
    }
}
