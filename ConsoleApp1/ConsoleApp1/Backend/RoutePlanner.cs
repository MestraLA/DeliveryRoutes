
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryRoutes.Backend
{
    internal class RoutePlanner
    {
        private readonly Graph city;
        private readonly IReadOnlyList<Package> packages;
        private readonly string start;

        private Dictionary<string, ShortestPaths> shortestPaths;

        public RoutePlanner(
            Graph city,
            IReadOnlyList<Package> packages,
            string start)
        {
            this.city = city;
            this.packages = packages;
            this.start = start;

            if (!city.ContainsVertex(start))
            {
                throw new ArgumentException(
                    "Starting location does not exist in the city.");
            }

            shortestPaths = new Dictionary<string, ShortestPaths>();
        }

        public double TotalDistance(List<Stop> route)
        {
            double totalDistance = 0;
            string current = start;

            foreach (Stop stop in route)
            {
                // Calculate the shortest path if it has not been calculated yet.
                if (!shortestPaths.ContainsKey(current))
                {
                    shortestPaths[current] = ShortestPaths.Dijkstra(city, current);
                }

                double distance = shortestPaths[current].GetDistanceTo(stop.Location);

                if (double.IsPositiveInfinity(distance))
                {
                    throw new InvalidOperationException(
                        $"There is no valid path from '{current}' to '{stop.Location}'.");
                }

                totalDistance += distance;
                current = stop.Location;
            }

            return totalDistance;
        }

        public List<string> PathBetween(string from, string to)
        {
            if (!city.ContainsVertex(from))
            {
                throw new ArgumentException(
                    $"Location '{from}' does not exist.");
            }

            if (!city.ContainsVertex(to))
            {
                throw new ArgumentException(
                    $"Location '{to}' does not exist.");
            }

            if (!shortestPaths.ContainsKey(from))
            {
                shortestPaths[from] = ShortestPaths.Dijkstra(city, from);
            }

            List<string> path = shortestPaths[from].GetPathTo(to);

            if (path.Count == 0)
            {
                throw new InvalidOperationException(
                    $"There is no valid path from '{from}' to '{to}'.");
            }

            return path;
        }


        public bool IsValid(List<Stop> route)
        {
            if (route == null)
            {
                return false;
            }

            Dictionary<int, Package> packageById =
                new Dictionary<int, Package>();

            foreach (Package package in packages)
            {
                if (packageById.ContainsKey(package.Id))
                {
                    return false;
                }

                packageById.Add(package.Id, package);
            }

            HashSet<int> pickedPackages = new HashSet<int>();
            HashSet<int> deliveredPackages = new HashSet<int>();

            foreach (Stop stop in route)
            {
                if (!packageById.ContainsKey(stop.PackageId))
                {
                    return false;
                }

                Package package = packageById[stop.PackageId];

                if (stop.Kind == StopKind.Pickup)
                {
                    if (stop.Location != package.Pickup ||
                        pickedPackages.Contains(stop.PackageId) ||
                        deliveredPackages.Contains(stop.PackageId))
                    {
                        return false;
                    }

                    pickedPackages.Add(stop.PackageId);
                }
                else if (stop.Kind == StopKind.Delivery)
                {
                    if (stop.Location != package.Delivery ||
                        !pickedPackages.Contains(stop.PackageId) ||
                        deliveredPackages.Contains(stop.PackageId))
                    {
                        return false;
                    }

                    deliveredPackages.Add(stop.PackageId);
                }
                else
                {
                    return false;
                }
            }

            return pickedPackages.Count == packageById.Count &&
                   deliveredPackages.Count == packageById.Count;
        }

        private List<Stop> PlanGreedy()

        {
            List<Stop> route = new List<Stop>();

            HashSet<int> pickedPackages = new HashSet<int>();
            HashSet<int> deliveredPackages = new HashSet<int>();

            string current = start;

            while (deliveredPackages.Count < packages.Count)
            {
                if (!shortestPaths.ContainsKey(current))
                {
                    shortestPaths[current] = ShortestPaths.Dijkstra(city, current);
                }

                Stop? bestStop = null;
                double bestDistance = double.PositiveInfinity;

                foreach (Package package in packages)
                {
                    Stop candidate;
                    double distance;

                    if (!pickedPackages.Contains(package.Id))
                    {
                        candidate = new Stop(
                            package.Pickup,
                            StopKind.Pickup,
                            package.Id);

                        distance = shortestPaths[current]
                            .GetDistanceTo(package.Pickup);
                    }
                    else if (!deliveredPackages.Contains(package.Id))
                    {
                        candidate = new Stop(
                            package.Delivery,
                            StopKind.Delivery,
                            package.Id);

                        distance = shortestPaths[current]
                            .GetDistanceTo(package.Delivery);
                    }
                    else
                    {
                        continue;
                    }

                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestStop = candidate;
                    }
                }

                if (bestStop == null ||
                    double.IsPositiveInfinity(bestDistance))
                {
                    throw new InvalidOperationException(
                        "No valid next stop could be found.");
                }

                route.Add(bestStop.Value);
                current = bestStop.Value.Location;

                if (bestStop.Value.Kind == StopKind.Pickup)
                {
                    pickedPackages.Add(bestStop.Value.PackageId);
                }
                else
                {
                    deliveredPackages.Add(bestStop.Value.PackageId);
                }
            }

            if (!IsValid(route))
            {
                throw new InvalidOperationException(
                    "The generated route is invalid.");
            }

            return route;
        }

        public List<Stop> Plan()
        {
            Console.WriteLine("Entrando en Plan()");
            Console.WriteLine($"Paquetes: {packages.Count}");

            if (packages.Count <= 3)
            {
                Console.WriteLine("EJECUTANDO PLAN EXACT");
                return PlanExact();
            }

            Console.WriteLine("EJECUTANDO PLAN GREEDY E INSERTION");

            List<Stop> greedyRoute = PlanGreedy();
            List<Stop> insertionRoute = PlanInsertion(packages);

            double greedyDistance = TotalDistance(greedyRoute);
            double insertionDistance = TotalDistance(insertionRoute);

            Console.WriteLine($"Distancia Greedy: {greedyDistance / 1000:F2} km");
            Console.WriteLine($"Distancia Insercion: {insertionDistance / 1000:F2} km");

            Console.WriteLine("EJECUTANDO REINICIOS ALEATORIOS");

            List<Stop> bestRoute = PlanWithRestarts();

            if (!IsValid(bestRoute))
            {
                throw new InvalidOperationException(
                    "The final route is invalid.");
            }

            return bestRoute;
        }   
        private List<Stop> PlanInsertion(
    IReadOnlyList<Package> packageOrder)
        {
            List<Stop> route = new List<Stop>();

            foreach (Package package in packageOrder)
            {
                double bestDistance = double.PositiveInfinity;
                List<Stop>? bestRoute = null;

                for (int pickupIndex = 0;
                     pickupIndex <= route.Count;
                     pickupIndex++)
                {
                    for (int deliveryIndex = pickupIndex + 1;
                         deliveryIndex <= route.Count + 1;
                         deliveryIndex++)
                    {
                        List<Stop> candidateRoute = new List<Stop>(route);

                        candidateRoute.Insert(
                            pickupIndex,
                            new Stop(package.Pickup, StopKind.Pickup, package.Id));

                        candidateRoute.Insert(
                            deliveryIndex,
                            new Stop(package.Delivery, StopKind.Delivery, package.Id));

                        double distance = TotalDistance(candidateRoute);

                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestRoute = candidateRoute;
                        }
                    }
                }

                if (bestRoute == null)
                {
                    throw new InvalidOperationException(
                        $"No valid insertion was found for package {package.Id}.");
                }

                route = bestRoute;
            }

            if (!IsValid(route))
            {
                throw new InvalidOperationException(
                    "The insertion algorithm generated an invalid route.");
            }

            return route;
        }
        private List<Stop> PlanWithRestarts()
        {
            Random random = new Random(42);

            List<Stop> bestRoute = PlanGreedy();

            double bestDistance = TotalDistance(bestRoute);

            const int restartCount = 100;

            for (int attempt = 1; attempt <= restartCount; attempt++)
            {
                List<Package> packageOrder = packages.ToList();

                for (int i = packageOrder.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);

                    Package temp = packageOrder[i];
                    packageOrder[i] = packageOrder[j];
                    packageOrder[j] = temp;
                }

                List<Stop> candidateRoute = PlanInsertion(packageOrder);

                candidateRoute = ImproveByRelocation(candidateRoute);

                candidateRoute = ImproveByStopMove(candidateRoute);

                double candidateDistance = TotalDistance(candidateRoute);

                Console.WriteLine(
                    $"Reinicio {attempt}: {candidateDistance / 1000:F2} km");

                if (candidateDistance < bestDistance &&
                    IsValid(candidateRoute))
                {
                    bestDistance = candidateDistance;
                    bestRoute = new List<Stop>(candidateRoute);
                }
            }

            Console.WriteLine(
                $"Mejor distancia despues de reinicios: {bestDistance / 1000:F2} km");

            return bestRoute;
        }
        private List<Stop> ImproveByRelocation(List<Stop> initialRoute)
        {
            List<Stop> bestRoute = new List<Stop>(initialRoute);

            double bestDistance = TotalDistance(bestRoute);
            bool improved = true;

            while (improved)
            {
                improved = false;

                foreach (Package package in packages)
                {
                    List<Stop> reducedRoute = bestRoute
                        .Where(stop => stop.PackageId != package.Id)
                        .ToList();

                    double packageBestDistance = bestDistance;
                    List<Stop>? packageBestRoute = null;

                    for (int pickupIndex = 0;
                         pickupIndex <= reducedRoute.Count;
                         pickupIndex++)
                    {
                        for (int deliveryIndex = pickupIndex + 1;
                             deliveryIndex <= reducedRoute.Count + 1;
                             deliveryIndex++)
                        {
                            List<Stop> candidateRoute =
                                new List<Stop>(reducedRoute);

                            candidateRoute.Insert(
                                pickupIndex,
                                new Stop(
                                    package.Pickup,
                                    StopKind.Pickup,
                                    package.Id));

                            candidateRoute.Insert(
                                deliveryIndex,
                                new Stop(
                                    package.Delivery,
                                    StopKind.Delivery,
                                    package.Id));

                            double candidateDistance =
                                TotalDistance(candidateRoute);

                            if (candidateDistance < packageBestDistance)
                            {
                                packageBestDistance = candidateDistance;
                                packageBestRoute = candidateRoute;
                            }
                        }
                    }

                    if (packageBestRoute != null)
                    {
                        bestRoute = packageBestRoute;
                        bestDistance = packageBestDistance;
                        improved = true;
                    }
                }
            }

            if (!IsValid(bestRoute))
            {
                throw new InvalidOperationException(
                    "The relocation algorithm generated an invalid route.");
            }

            return bestRoute;
        }

        private List<Stop> ImproveByStopMove(List<Stop> initialRoute)
        {
            List<Stop> bestRoute = new List<Stop>(initialRoute);

            double bestDistance = TotalDistance(bestRoute);
            bool improved = true;

            while (improved)
            {
                improved = false;

                for (int originalIndex = 0;
                     originalIndex < bestRoute.Count;
                     originalIndex++)
                {
                    Stop stop = bestRoute[originalIndex];

                    List<Stop> reducedRoute = new List<Stop>(bestRoute);
                    reducedRoute.RemoveAt(originalIndex);

                    for (int newIndex = 0;
                         newIndex <= reducedRoute.Count;
                         newIndex++)
                    {
                        List<Stop> candidateRoute =
                            new List<Stop>(reducedRoute);

                        candidateRoute.Insert(newIndex, stop);

                        if (!IsValid(candidateRoute))
                        {
                            continue;
                        }

                        double candidateDistance =
                            TotalDistance(candidateRoute);

                        if (candidateDistance < bestDistance)
                        {
                            bestDistance = candidateDistance;
                            bestRoute = candidateRoute;
                            improved = true;
                            break;
                        }
                    }

                    if (improved)
                    {
                        break;
                    }
                }
            }

            if (!IsValid(bestRoute))
            {
                throw new InvalidOperationException(
                    "The stop movement algorithm generated an invalid route.");
            }

            return bestRoute;
        }


        private List<Stop> PlanExact()
        {
            double bestDistance = double.PositiveInfinity;

            List<Stop>? bestRoute = null;
            List<Stop> currentRoute = new List<Stop>();

            HashSet<int> pickedPackages = new HashSet<int>();
            HashSet<int> deliveredPackages = new HashSet<int>();

            SearchExact(
                start,
                pickedPackages,
                deliveredPackages,
                currentRoute,
                0,
                ref bestDistance,
                ref bestRoute);

            if (bestRoute == null)
            {
                throw new InvalidOperationException(
                    "No valid route could be found.");
            }

            return bestRoute;
        }

        private void SearchExact(
    string current,
    HashSet<int> pickedPackages,
    HashSet<int> deliveredPackages,
    List<Stop> currentRoute,
    double currentDistance,
    ref double bestDistance,
    ref List<Stop>? bestRoute)
        {
            if (deliveredPackages.Count == packages.Count)
            {
                if (currentDistance < bestDistance)
                {
                    bestDistance = currentDistance;
                    bestRoute = new List<Stop>(currentRoute);
                }

                return;
            }

            if (!shortestPaths.ContainsKey(current))
            {
                shortestPaths[current] =
                    ShortestPaths.Dijkstra(city, current);
            }

            foreach (Package package in packages)
            {
                Stop candidate;

                if (!pickedPackages.Contains(package.Id))
                {
                    candidate = new Stop(
                        package.Pickup,
                        StopKind.Pickup,
                        package.Id);
                }
                else if (!deliveredPackages.Contains(package.Id))
                {
                    candidate = new Stop(
                        package.Delivery,
                        StopKind.Delivery,
                        package.Id);
                }
                else
                {
                    continue;
                }

                double distance = shortestPaths[current]
                    .GetDistanceTo(candidate.Location);

                if (double.IsPositiveInfinity(distance))
                {
                    continue;
                }

                double newDistance = currentDistance + distance;

                if (newDistance >= bestDistance)
                {
                    continue;
                }

                HashSet<int> newPicked =
                    new HashSet<int>(pickedPackages);

                HashSet<int> newDelivered =
                    new HashSet<int>(deliveredPackages);

                if (candidate.Kind == StopKind.Pickup)
                {
                    newPicked.Add(candidate.PackageId);
                }
                else
                {
                    newDelivered.Add(candidate.PackageId);
                }

                currentRoute.Add(candidate);

                SearchExact(
                    candidate.Location,
                    newPicked,
                    newDelivered,
                    currentRoute,
                    newDistance,
                    ref bestDistance,
                    ref bestRoute);

                currentRoute.RemoveAt(currentRoute.Count - 1);
            }
        }
    }

}
