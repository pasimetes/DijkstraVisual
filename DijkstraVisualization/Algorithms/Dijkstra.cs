using DijkstraVisualization.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DijkstraVisualization.Algorithms
{
    public class Dijkstra
    {
        private readonly Graph _graph;
        private readonly IDictionary<Vertex, int> _distances;
        private readonly IDictionary<Vertex, ICollection<Vertex>> _visitLog;
        private readonly ICollection<Vertex> _verticesToVisit;
        private readonly IDictionary<Vertex, Vertex> _shortestPaths;

        public Dijkstra(Graph graph)
        {
            _graph = graph;
            _distances = new Dictionary<Vertex, int>();
            _visitLog = new Dictionary<Vertex, ICollection<Vertex>>();
            _verticesToVisit = new List<Vertex>();
            _shortestPaths = new Dictionary<Vertex, Vertex>();
        }

        public ShortestPaths CalculateShortestPaths(Vertex from)
        {
            Initialize(from);

            while (_verticesToVisit.Any())
            {
                var subject = GetSubject();

                UpdateCosts(subject);
            }

            return new ShortestPaths(
                _distances.ToDictionary(
                    k => k.Key.Id,
                    v => v.Value),
                _visitLog.ToDictionary(
                    k => k.Key.Id,
                    v => v.Value.Select(v => v.Id).ToList()),
                _shortestPaths.ToDictionary(
                    k => k.Key.Id,
                    v => v.Value.Id));
        }

        private void Initialize(Vertex source)
        {
            _distances.Clear();
            _verticesToVisit.Clear();
            _visitLog.Clear();
            _shortestPaths.Clear();

            _distances[source] = 0;

            foreach (var vertex in _graph.GetAllVertices())
            {
                if (!Equals(vertex, source))
                {
                    _distances[vertex] = int.MaxValue;
                }

                _verticesToVisit.Add(vertex);
            }
        }

        private Vertex GetSubject()
        {
            var subject = default(Vertex);
            var flag = true;

            foreach (var vertex in _verticesToVisit)
            {
                if (flag || _distances[vertex] < _distances[subject])
                {
                    subject = vertex;
                    flag = false;
                }
            }

            return subject;
        }

        private void UpdateCosts(Vertex from)
        {
            foreach (var to in _graph.GetNeighbours(from))
            {
                var cost = _distances[from] + _graph.GetCost(from, to);
                if (cost < _distances[to])
                {
                    _distances[to] = cost;
                    _shortestPaths[to] = from;
                }

                if (_visitLog.ContainsKey(from))
                    _visitLog[from].Add(to);
                else
                    _visitLog[from] = new List<Vertex> { to };

                _verticesToVisit.Remove(from);
            }
        }
    }
}
