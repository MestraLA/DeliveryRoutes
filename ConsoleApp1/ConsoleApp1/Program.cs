
using System;
using DeliveryRoutes.Backend;

namespace DeliveryRoutes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PRUEBA: NUEVO PROGRAM EJECUTADO");
            try
            {
                string cityPath;
                string packagesPath;
                string start;

                if (args.Length >= 3)
                {
                    cityPath = args[0];
                    packagesPath = args[1];
                    start = args[2];
                }
                else
                {
                    Console.Write("Enter the city file path: ");
                    cityPath = Console.ReadLine() ?? "";

                    Console.Write("Enter the packages file path: ");
                    packagesPath = Console.ReadLine() ?? "";

                    Console.Write("Enter the starting location: ");
                    start = Console.ReadLine() ?? "";
                }

                Graph city = CityFile.Load(cityPath);

                var packages = PackageFile.Load(
                    packagesPath,
                    city);
                Console.WriteLine($"Paquetes cargados: {packages.Count}");
                


                RoutePlanner planner = new RoutePlanner(
                    city,
                    packages,
                    start);

                var route = planner.Plan();

                RoutePrinter.Print(
                    city,
                    planner,
                    route,
                    start);

            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
