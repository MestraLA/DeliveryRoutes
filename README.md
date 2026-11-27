# DeliveryRoutes

## Descripción

DeliveryRoutes es una aplicación de consola desarrollada en C# que permite planificar rutas para recoger y entregar paquetes dentro de una ciudad.

La ciudad se representa mediante un grafo dirigido y ponderado. El sistema calcula recorridos teniendo en cuenta las distancias entre ubicaciones y la condición de que cada paquete debe recogerse antes de entregarse.

## Tecnologías utilizadas

- C#
- .NET 10
- Visual Studio
- Grafos dirigidos y ponderados
- Algoritmo de Dijkstra
- Archivos de texto plano

## Estructura del proyecto

```text
DeliveryRoutes/
│
├── .gitignore
├── README.md
│
└── ConsoleApp1/
    │
    ├── Backend/
    │   ├── CityFile.cs
    │   ├── Edge.cs
    │   ├── Graph.cs
    │   ├── Package.cs
    │   ├── PackageFile.cs
    │   ├── RoutePlanner.cs
    │   ├── RoutePrinter.cs
    │   ├── ShortestPaths.cs
    │   └── Stop.cs
    │
    ├── city.txt
    ├── city_mini.txt
    ├── packages.txt
    ├── packages_mini.txt
    └── Program.cs
```

## Archivos de entrada

El programa utiliza dos archivos para representar la ciudad y los paquetes.

### city.txt

Contiene las calles de la ciudad. Cada línea representa una conexión entre dos ubicaciones.

Formato:

```text
Origen,Destino,Distancia,Direccion
```

Direcciones:

- `TwoWay`: permite circular en ambos sentidos.
- `OneWay`: permite circular únicamente desde el origen hasta el destino.

### packages.txt

Contiene las tareas de recogida y entrega de paquetes.

Formato:

```text
Ubicacion,Accion,IdPaquete
```

Acciones:

- `Pickup`: recoger un paquete.
- `Delivery`: entregar un paquete.

Cada paquete debe tener una recogida y una entrega.

### Archivos de prueba

El proyecto incluye dos escenarios:

- `city.txt` y `packages.txt`: escenario principal con 30 paquetes.
- `city_mini.txt` y `packages_mini.txt`: escenario pequeño para probar la planificación exacta.

## Algoritmos utilizados

### Grafo

La ciudad se representa mediante un grafo dirigido y ponderado. Las ubicaciones son vértices y las calles son aristas con una distancia asociada.

### Dijkstra

Calcula los caminos más cortos entre las ubicaciones de interés.

### Greedy

Utiliza una estrategia de vecino más cercano para construir una ruta inicial.

### Inserción

Construye una ruta alternativa mediante inserción de paradas, respetando las restricciones de recogida y entrega.

### Reinicios aleatorios

Se realizan diferentes reinicios para buscar recorridos de menor distancia.

Durante este proceso se aplican operaciones de mejora y se conserva la mejor ruta válida encontrada.

## Restricciones de la ruta

- Todos los paquetes deben recogerse.
- Todos los paquetes deben entregarse.
- Un paquete no puede entregarse antes de recogerse.
- Los desplazamientos deben utilizar caminos existentes.
- La ruta termina en la última ubicación de entrega.

## Instalación y ejecución

### Requisitos

- .NET SDK 10.0
- Visual Studio

### Compilar el proyecto

Desde la carpeta de la solución:

```powershell
dotnet clean
dotnet build
```

### Ejecutar el escenario principal

Desde `C:\estudio\DeliveryRoutes\ConsoleApp1`:

```powershell
dotnet run --project .\ConsoleApp1\DeliveryRoutes.csproj -- .\ConsoleApp1\city.txt .\ConsoleApp1\packages.txt D
```

Los argumentos representan:

1. Archivo de la ciudad.
2. Archivo de paquetes.
3. Ubicación inicial.

En este ejemplo, la ubicación inicial es `D`.

## Resultados

Al ejecutar el programa se muestran:

- Distancia de la ruta Greedy.
- Distancia de la ruta por inserción.
- Resultados de los reinicios aleatorios.
- Mejor distancia encontrada.
- Paradas del recorrido.
- Paquetes recogidos y entregados.
- Caminos utilizados entre ubicaciones.
- Distancia total.

## Autor
Adrian David Mestra Lopez.
Proyecto académico desarrollado en C#.