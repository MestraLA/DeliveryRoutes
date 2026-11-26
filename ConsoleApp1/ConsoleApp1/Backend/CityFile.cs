
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DeliveryRoutes.Backend
{
    internal class CityFile
    {
        public static Graph Load(string path)
        {
            Graph city = new Graph();
            HashSet<string> streets = new HashSet<string>();

            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string[] fields = line.Split(',');

                if (fields.Length != 4)
                {
                    throw new FormatException(
                        $"Invalid format at line {i + 1}.");
                }

                string origin = fields[0].Trim();
                string destination = fields[1].Trim();
                string direction = fields[3].Trim();

                if (!double.TryParse(
                    fields[2].Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double distance) ||
                               !double.IsFinite(distance) ||
                               distance <= 0)
                {
                    throw new FormatException(
                        $"Invalid distance at line {i + 1}.");
                }

                if (string.IsNullOrWhiteSpace(origin) ||
                    string.IsNullOrWhiteSpace(destination))
                {
                    throw new FormatException(
                        $"Invalid location at line {i + 1}.");
                }

                if (origin == destination)
                {
                    throw new FormatException(
                        $"A street cannot connect a location to itself at line {i + 1}.");
                }

                if (!direction.Equals("TwoWay", StringComparison.OrdinalIgnoreCase) &&
                    !direction.Equals("OneWay", StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException(
                        $"Invalid street direction at line {i + 1}.");
                }

                string firstLocation = origin;
                string secondLocation = destination;

                if (string.Compare(firstLocation, secondLocation,
                    StringComparison.Ordinal) > 0)
                {
                    firstLocation = destination;
                    secondLocation = origin;
                }

                string streetKey = firstLocation + "|" + secondLocation;

                if (!streets.Add(streetKey))
                {
                    throw new FormatException(
                        $"Duplicate street at line {i + 1}.");
                }

                city.AddEdge(origin, destination, distance);

                if (direction.Equals("TwoWay",
                    StringComparison.OrdinalIgnoreCase))
                {
                    city.AddEdge(destination, origin, distance);
                }
            }

            return city;
        }
    }
}
