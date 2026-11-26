
using System;
using System.Collections.Generic;

namespace DeliveryRoutes.Backend
{
    internal class ShortestPaths
    {
        private Dictionary<string, double> distances;
        private Dictionary<string, string> previous;

        private ShortestPaths()
        {
            distances = new Dictionary<string, double>();
            previous = new Dictionary<string, string>();
        }

        public static ShortestPaths Dijkstra(Graph graph, string source)
        {
            if (!graph.ContainsVertex(source))
            {
                throw new ArgumentException("Source location does not exist.");
            }

            ShortestPaths result = new ShortestPaths();

            PriorityQueue<string, double> queue =
                new PriorityQueue<string, double>();

            result.distances[source] = 0;
            queue.Enqueue(source, 0);

            while (queue.Count > 0)
            {
                queue.TryDequeue(out string? current, out double currentDistance);

                if (current == null)
                {
                    break;
                }

                if (currentDistance > result.distances[current])
                {
                    continue;
                }

                List<Edge> edges = graph.GetEdges(current);

                foreach (Edge edge in edges)
                {
                    double newDistance = currentDistance + edge.Weight;

                    if (!result.distances.ContainsKey(edge.Destination) ||
                        newDistance < result.distances[edge.Destination])
                    {
                        result.distances[edge.Destination] = newDistance;
                        result.previous[edge.Destination] = current;

                        queue.Enqueue(edge.Destination, newDistance);
                    }
                }
            }

            return result;
        }

        public double GetDistanceTo(string destination)
        {
            if (distances.ContainsKey(destination))
            {
                return distances[destination];
            }

            return double.PositiveInfinity;
        }

        public List<string> GetPathTo(string destination)
        {
            List<string> path = new List<string>();

            if (!distances.ContainsKey(destination))
            {
                return path;
            }

            string current = destination;

            while (previous.ContainsKey(current))
            {
                path.Add(current);
                current = previous[current];
            }

            path.Add(current);
            path.Reverse();

            return path;
        }
    }
}
