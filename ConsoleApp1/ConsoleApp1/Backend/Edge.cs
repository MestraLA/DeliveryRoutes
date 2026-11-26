namespace DeliveryRoutes.Backend
{
    internal class Edge
    {
        public string Destination { get; set; }
        public double Weight { get; set; }

        public Edge(string destination, double weight)
        {
            Destination = destination;
            Weight = weight;
        }
    }
}