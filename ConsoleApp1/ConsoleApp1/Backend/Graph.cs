using System.Collections.Generic;

namespace DeliveryRoutes.Backend
{
    internal class Graph
    {
        private Dictionary<string, List<Edge>> adjacencyList;

        public Graph()
        {
            adjacencyList = new Dictionary<string, List<Edge>>();
        }

        public void AddVertex(string location)
        {
            if (!adjacencyList.ContainsKey(location))
            {
                adjacencyList.Add(location, new List<Edge>());
            }
        }

        public void AddEdge(string origin, string destination, double weight)
        {
            AddVertex(origin);
            AddVertex(destination);

            adjacencyList[origin].Add(new Edge(destination, weight));
        }

        public bool ContainsVertex(string location)
        {
            return adjacencyList.ContainsKey(location);
        }

        public List<Edge> GetEdges(string location)
        {
            if (adjacencyList.ContainsKey(location))
            {
                return adjacencyList[location];
            }

            return new List<Edge>();
        }
    }
}