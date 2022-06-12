using DijkstraVisualization.Algorithms;
using DijkstraVisualization.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DijkstraVisualization
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //TestSeed1();
        }

        private void TestSeed1()
        {
            var graph = new Graph();

            var v0 = new Vertex("v0");
            var v1 = new Vertex("v1");
            var v2 = new Vertex("v2");
            var v3 = new Vertex("v3");
            var v4 = new Vertex("v4");
            var v5 = new Vertex("v5");
            var v6 = new Vertex("v6");
            var v7 = new Vertex("v7");

            graph.AddVertex(v0);
            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v3);
            graph.AddVertex(v4);
            graph.AddVertex(v5);
            graph.AddVertex(v6);
            graph.AddVertex(v7);

            graph.AddEdge(v0, v3, 1);
            graph.AddEdge(v3, v1, 7);
            graph.AddEdge(v3, v5, 1);
            graph.AddEdge(v5, v1, 8);
            graph.AddEdge(v5, v7, 1);
            graph.AddEdge(v1, v2, 7);
            graph.AddEdge(v1, v6, 2);
            graph.AddEdge(v6, v4, 3);
            graph.AddEdge(v2, v4, 4);

            var dij = new Dijkstra(graph);
            dij.CalculateShortestPaths(v0);

            //var result = dij.GetShortestPathToDestination(v5);
        }

        //public static ShortestPath GetShortestPathToDestination(Vertex destination)
        //{
        //    var path = new List<Vertex>();

        //    foreach (var entry in _visitLog)
        //    {
        //        if (Equals(entry.Key, destination))
        //        {
        //            path.Add(destination);
        //            break;
        //        }

        //        path.Add(entry.Key);
        //    }

        //    return new ShortestPath(path, _distances[destination]);
        //}
    }
}
