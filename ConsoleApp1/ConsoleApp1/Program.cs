
using System;
using DeliveryRoutes.Backend;

namespace DeliveryRoutes
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
                    Console.Write("Ingrese la ruta del archivo de ciudades: ");
                    cityPath = Console.ReadLine() ?? "";

                    Console.Write("Ingrese la ruta del archivo de paquetes: ");
                    packagesPath = Console.ReadLine() ?? "";

                    Console.Write("Ingrese la ubicación inicial: ");
                    start = Console.ReadLine() ?? "";
                }

                Graph city = CityFile.Load(cityPath);

                var packages = PackageFile.Load(
                    packagesPath,
                    city);

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
