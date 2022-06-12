using DijkstraVisualization.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DijkstraVisualization.Extensions
{
    public static class GraphExtensions
    {
        public static int[,] ToAdjacencyMatrix(this Graph graph)
        {
            var vertices = graph.GetAllVertices();
            var count = vertices.Count;
            var matrix = new int[count, count];

            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                {
                    if (i == j)
                        continue;

                    var edge = graph.GetEdge(vertices[i], vertices[j]);
                    if (edge != null)
                        matrix[i, j] = edge.Cost;

                }

            return matrix;
        }

        public static Dictionary<string, ICollection<Tuple<string, int>>> ToAdjacencyList(this Graph graph)
        {
            var vertices = graph.GetAllVertices();
            var edges = graph.GetAllEdges();
            var result = new Dictionary<string, ICollection<Tuple<string, int>>>();

            foreach (var vertex in vertices)
            {
                var connections = new List<Tuple<string, int>>();

                foreach (var edge in edges)
                    if (edge.From == vertex)
                        connections.Add(new Tuple<string, int>(edge.To.Id, edge.Cost));

                result.Add(vertex.Id, connections);
            }

            return result;
        }

        public static string[,] Print(this string[,] matrix)
        {
            var rows = matrix.GetLength(0);
            var columns = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (i == j)
                    {
                        Debug.Write(" x ");
                        continue;
                    }

                    Debug.Write($" {matrix[i, j]} ");
                }
                Debug.WriteLine("");
            }

            return matrix;
        }
    }
}
