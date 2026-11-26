
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryRoutes.Backend
{
    internal static class RoutePrinter
    {
        public static void Print(
            Graph city,
            RoutePlanner planner,
            List<Stop> route,
            string start)
        {
            string current = start;
            int stopNumber = 1;
            double totalDistance = 0;

            int index = 0;

            while (index < route.Count)
            {
                string location = route[index].Location;

                List<Stop> tasks = new List<Stop>();

                // Group all consecutive tasks at the same location.
                while (index < route.Count &&
                       route[index].Location == location)
                {
                    tasks.Add(route[index]);
                    index++;
                }

                List<Stop> deliveries = tasks
                    .Where(s => s.Kind == StopKind.Delivery)
                    .ToList();

                List<Stop> pickups = tasks
                    .Where(s => s.Kind == StopKind.Pickup)
                    .ToList();

                List<string> path = planner.PathBetween(
                    current,
                    location);

                double legDistance = 0;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    foreach (Edge edge in city.GetEdges(path[i]))
                    {
                        if (edge.Destination == path[i + 1])
                        {
                            legDistance += edge.Weight;
                            break;
                        }
                    }
                }

                totalDistance += legDistance;

                Console.WriteLine();
                Console.WriteLine($"Parada {stopNumber}:");

                if (current == location)
                {
                    Console.WriteLine(
                        $"En {location}: sin desplazamiento.");
                }
                else
                {
                    Console.WriteLine(
                        $"Ir de {current} a {location} " +
                        $"({legDistance:F0} m).");
                }

                foreach (Stop task in deliveries)
                {
                    Console.WriteLine(
                        $"  Entregar paquete {task.PackageId}.");
                }

                foreach (Stop task in pickups)
                {
                    Console.WriteLine(
                        $"  Recoger paquete {task.PackageId}.");
                }

                Console.WriteLine(
                    $"  Camino: {string.Join(" -> ", path)}");

                current = location;
                stopNumber++;
            }

            Console.WriteLine();
            Console.WriteLine("Todos los paquetes fueron recogidos y entregados.");

            Console.WriteLine(
                $"Recorrido total: {totalDistance / 1000:F2} km");
        }
    }
}
