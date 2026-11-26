
using System;
using System.Collections.Generic;
using System.IO;

namespace DeliveryRoutes.Backend
{
    internal static class PackageFile
    {
        public static List<Package> Load(string path, Graph city)
        {
            Dictionary<int, (string Pickup, string Delivery)> tasks =
                new Dictionary<int, (string Pickup, string Delivery)>();

            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string[] fields = line.Split(',');

                if (fields.Length != 3)
                {
                    throw new FormatException(
                        $"Invalid package format at line {i + 1}.");
                }

                string location = fields[0].Trim();
                string action = fields[1].Trim();

                if (!int.TryParse(fields[2].Trim(), out int packageId) ||
                    packageId <= 0)
                {
                    throw new FormatException(
                        $"Invalid package ID at line {i + 1}.");
                }

                if (string.IsNullOrWhiteSpace(location))
                {
                    throw new FormatException(
                        $"Invalid location at line {i + 1}.");
                }

                if (!city.ContainsVertex(location))
                {
                    throw new FormatException(
                        $"Location '{location}' does not exist at line {i + 1}.");
                }

                if (!action.Equals("Pickup", StringComparison.OrdinalIgnoreCase) &&
                    !action.Equals("Delivery", StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException(
                        $"Invalid action at line {i + 1}.");
                }

                if (!tasks.ContainsKey(packageId))
                {
                    tasks.Add(packageId, ("", ""));
                }

                (string Pickup, string Delivery) packageTasks = tasks[packageId];

                if (action.Equals("Pickup", StringComparison.OrdinalIgnoreCase))
                {
                    if (packageTasks.Pickup != "")
                    {
                        throw new FormatException(
                            $"Package {packageId} has more than one Pickup.");
                    }

                    packageTasks.Pickup = location;
                }
                else
                {
                    if (packageTasks.Delivery != "")
                    {
                        throw new FormatException(
                            $"Package {packageId} has more than one Delivery.");
                    }

                    packageTasks.Delivery = location;
                }

                tasks[packageId] = packageTasks;
            }

            List<Package> packages = new List<Package>();

            foreach (KeyValuePair<int, (string Pickup, string Delivery)> item in tasks)
            {
                if (item.Value.Pickup == "")
                {
                    throw new FormatException(
                        $"Package {item.Key} is missing its Pickup.");
                }

                if (item.Value.Delivery == "")
                {
                    throw new FormatException(
                        $"Package {item.Key} is missing its Delivery.");
                }

                packages.Add(new Package(
                    item.Key,
                    item.Value.Pickup,
                    item.Value.Delivery));
            }

            return packages;
        }
    }
}