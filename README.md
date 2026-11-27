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

## Ejemplo pequeño - Sección 6

Se ejecutó el escenario pequeño con 3 paquetes y posición inicial A.

**Comando utilizado:**

`dotnet run --project .\ConsoleApp1\ConsoleApp1\DeliveryRoutes.csproj -- .\ConsoleApp1\ConsoleApp1\city_mini.txt .\ConsoleApp1\ConsoleApp1\packages_mini.txt A`

**Resultado obtenido:**

- Parada 1: A → C (400 m). Recoger paquete 1.
- Parada 2: C → G (620 m). Entregar paquete 1 y recoger paquete 3.
- Parada 3: G → E (270 m). Recoger paquete 2.
- Parada 4: E → A (550 m). Entregar paquete 3.
- Parada 5: A → D (600 m). Entregar paquete 2.

Todos los paquetes fueron recogidos y entregados.

**Recorrido total: 2,44 km.**

La ruta obtenida coincide con la distancia óptima del ejemplo del taller.

## Estrategia utilizada para encontrar la ruta

El programa utiliza una combinación de algoritmos exactos y heurísticos para encontrar recorridos de corta distancia, respetando las condiciones de recogida y entrega.

### 1. Dijkstra: caminos más cortos

Se utiliza el algoritmo de Dijkstra para calcular las distancias mínimas entre las ubicaciones importantes.

Como el grafo es dirigido, los recorridos de ida y vuelta pueden tener distancias diferentes. Por eso se calculan los caminos respetando el sentido de las calles.

### 2. Greedy: ruta inicial

Se utiliza una estrategia de vecino más cercano. Desde la ubicación actual, el programa selecciona la tarea válida más cercana.

Una tarea válida puede ser recoger un paquete pendiente o entregar uno que ya fue recogido.

Esta estrategia permite construir rápidamente una primera ruta, aunque no garantiza la menor distancia.

### 3. Inserción y mejoras locales

Se construye una ruta mediante inserción de paquetes, buscando las posiciones que produzcan el menor incremento de distancia y respetando que cada recogida ocurra antes de su entrega.

Después se aplican dos mejoras:

- Reubicación de paquetes: se retiran la recogida y la entrega de un paquete y se buscan nuevas posiciones para ambas tareas.
- Movimiento de paradas: se mueve una parada individual a otra posición, siempre conservando la validez de la ruta.

### 4. Reinicios aleatorios

El programa realiza 100 reinicios utilizando una semilla fija (`Random(42)`).

En cada reinicio se cambia el orden de los paquetes, se construye una nueva ruta y se aplican las mejoras locales.

Finalmente, se conserva la ruta válida de menor distancia encontrada.

La semilla fija permite reproducir los resultados.

### 5. ¿Por qué no se prueban todas las combinaciones?

Para 30 paquetes existen 60 tareas: 30 recogidas y 30 entregas.

Incluso respetando la condición de recoger antes de entregar, el número de órdenes posibles es 60! / 2^30, una cantidad demasiado grande para explorar en un tiempo razonable.

Por esta razón se utilizan heurísticas y mejoras locales. No garantizan encontrar el óptimo global, pero permiten obtener rutas de buena calidad en un tiempo práctico.

### 6. Validación de la ruta

Después de construir y mejorar una ruta, el programa verifica que:

- Cada paquete se recoja exactamente una vez.
- Cada paquete se entregue exactamente una vez.
- Ningún paquete se entregue antes de ser recogido.
- Los recorridos entre ubicaciones sean posibles.
- La distancia total corresponda a la suma de los tramos.
## Salida completa del escenario principal

Resultado generado ejecutando el escenario principal desde D:

``text
Entrando en Plan()
Paquetes: 30
EJECUTANDO PLAN GREEDY E INSERTION
Distancia Greedy: 40,52 km
Distancia Insercion: 36,07 km
EJECUTANDO REINICIOS ALEATORIOS
Reinicio 1: 34,79 km
Reinicio 2: 33,68 km
Reinicio 3: 35,87 km
Reinicio 4: 33,43 km
Reinicio 5: 34,03 km
Reinicio 6: 32,53 km
Reinicio 7: 33,37 km
Reinicio 8: 33,73 km
Reinicio 9: 33,34 km
Reinicio 10: 34,54 km
Reinicio 11: 33,21 km
Reinicio 12: 36,33 km
Reinicio 13: 35,22 km
Reinicio 14: 34,21 km
Reinicio 15: 36,03 km
Reinicio 16: 34,73 km
Reinicio 17: 36,98 km
Reinicio 18: 34,31 km
Reinicio 19: 31,59 km
Reinicio 20: 34,13 km
Reinicio 21: 35,12 km
Reinicio 22: 33,92 km
Reinicio 23: 35,35 km
Reinicio 24: 34,82 km
Reinicio 25: 33,62 km
Reinicio 26: 32,24 km
Reinicio 27: 34,18 km
Reinicio 28: 36,45 km
Reinicio 29: 32,61 km
Reinicio 30: 33,56 km
Reinicio 31: 34,72 km
Reinicio 32: 32,50 km
Reinicio 33: 32,52 km
Reinicio 34: 32,69 km
Reinicio 35: 33,60 km
Reinicio 36: 31,27 km
Reinicio 37: 35,24 km
Reinicio 38: 33,13 km
Reinicio 39: 35,43 km
Reinicio 40: 35,42 km
Reinicio 41: 33,56 km
Reinicio 42: 35,07 km
Reinicio 43: 30,38 km
Reinicio 44: 34,32 km
Reinicio 45: 36,58 km
Reinicio 46: 34,80 km
Reinicio 47: 32,94 km
Reinicio 48: 38,31 km
Reinicio 49: 30,87 km
Reinicio 50: 32,64 km
Reinicio 51: 33,43 km
Reinicio 52: 34,83 km
Reinicio 53: 32,67 km
Reinicio 54: 34,94 km
Reinicio 55: 34,61 km
Reinicio 56: 32,33 km
Reinicio 57: 35,25 km
Reinicio 58: 31,59 km
Reinicio 59: 33,08 km
Reinicio 60: 31,21 km
Reinicio 61: 33,70 km
Reinicio 62: 35,39 km
Reinicio 63: 36,51 km
Reinicio 64: 33,86 km
Reinicio 65: 35,15 km
Reinicio 66: 34,31 km
Reinicio 67: 35,19 km
Reinicio 68: 34,97 km
Reinicio 69: 34,66 km
Reinicio 70: 31,59 km
Reinicio 71: 31,15 km
Reinicio 72: 35,52 km
Reinicio 73: 35,73 km
Reinicio 74: 34,89 km
Reinicio 75: 33,81 km
Reinicio 76: 33,43 km
Reinicio 77: 33,47 km
Reinicio 78: 33,43 km
Reinicio 79: 34,72 km
Reinicio 80: 37,28 km
Reinicio 81: 34,45 km
Reinicio 82: 34,96 km
Reinicio 83: 32,61 km
Reinicio 84: 33,55 km
Reinicio 85: 31,91 km
Reinicio 86: 35,20 km
Reinicio 87: 35,42 km
Reinicio 88: 35,49 km
Reinicio 89: 34,36 km
Reinicio 90: 34,62 km
Reinicio 91: 33,70 km
Reinicio 92: 35,51 km
Reinicio 93: 33,34 km
Reinicio 94: 33,59 km
Reinicio 95: 36,78 km
Reinicio 96: 30,87 km
Reinicio 97: 34,80 km
Reinicio 98: 33,53 km
Reinicio 99: 35,00 km
Reinicio 100: 36,09 km
Mejor distancia despues de reinicios: 30,38 km

Parada 1:
En D: sin desplazamiento.
  Recoger paquete 18.
  Camino: D

Parada 2:
Ir de D a AR (1110 m).
  Recoger paquete 20.
  Camino: D -> BT -> P -> CB -> AD -> AR

Parada 3:
Ir de AR a BB (150 m).
  Recoger paquete 16.
  Camino: AR -> BB

Parada 4:
Ir de BB a AJ (400 m).
  Recoger paquete 23.
  Recoger paquete 26.
  Camino: BB -> AJ

Parada 5:
Ir de AJ a BE (1660 m).
  Recoger paquete 7.
  Camino: AJ -> BB -> AR -> AD -> CB -> P -> BT -> BE

Parada 6:
Ir de BE a X (490 m).
  Recoger paquete 12.
  Camino: BE -> E -> X

Parada 7:
Ir de X a BU (560 m).
  Recoger paquete 8.
  Recoger paquete 28.
  Camino: X -> B -> BU

Parada 8:
Ir de BU a CN (430 m).
  Recoger paquete 2.
  Recoger paquete 3.
  Recoger paquete 1.
  Camino: BU -> CN

Parada 9:
Ir de CN a CS (1780 m).
  Recoger paquete 19.
  Camino: CN -> BU -> AM -> AK -> AX -> AQ -> BS -> CS

Parada 10:
Ir de CS a BS (310 m).
  Recoger paquete 25.
  Camino: CS -> BS

Parada 11:
Ir de BS a CR (520 m).
  Entregar paquete 20.
  Camino: BS -> CT -> CR

Parada 12:
Ir de CR a CH (680 m).
  Recoger paquete 10.
  Camino: CR -> BO -> CJ -> CH

Parada 13:
Ir de CH a CU (1050 m).
  Recoger paquete 15.
  Camino: CH -> CJ -> CV -> BX -> CU

Parada 14:
Ir de CU a CV (360 m).
  Entregar paquete 28.
  Camino: CU -> CV

Parada 15:
Ir de CV a M (700 m).
  Entregar paquete 1.
  Camino: CV -> W -> CM -> M

Parada 16:
Ir de M a AE (430 m).
  Entregar paquete 26.
  Camino: M -> CM -> AE

Parada 17:
Ir de AE a AP (370 m).
  Recoger paquete 5.
  Camino: AE -> AP

Parada 18:
Ir de AP a AM (390 m).
  Recoger paquete 30.
  Camino: AP -> AM

Parada 19:
Ir de AM a AX (270 m).
  Recoger paquete 17.
  Camino: AM -> AK -> AX

Parada 20:
Ir de AX a BV (630 m).
  Entregar paquete 19.
  Camino: AX -> F -> BV

Parada 21:
Ir de BV a AB (470 m).
  Recoger paquete 4.
  Camino: BV -> AB

Parada 22:
Ir de AB a BW (2360 m).
  Entregar paquete 30.
  Camino: AB -> BV -> F -> AX -> S -> K -> N -> BW

Parada 23:
Ir de BW a CL (270 m).
  Recoger paquete 14.
  Camino: BW -> CL

Parada 24:
Ir de CL a R (360 m).
  Recoger paquete 21.
  Recoger paquete 6.
  Camino: CL -> R

Parada 25:
Ir de R a BN (810 m).
  Entregar paquete 7.
  Camino: R -> CL -> BN

Parada 26:
Ir de BN a AI (1130 m).
  Recoger paquete 24.
  Camino: BN -> Y -> AV -> CG -> AI

Parada 27:
Ir de AI a AO (170 m).
  Entregar paquete 3.
  Recoger paquete 11.
  Camino: AI -> AO

Parada 28:
Ir de AO a BM (720 m).
  Entregar paquete 2.
  Entregar paquete 10.
  Camino: AO -> AI -> AS -> BM

Parada 29:
Ir de BM a BI (1750 m).
  Recoger paquete 13.
  Camino: BM -> AS -> BH -> BL -> CO -> BI

Parada 30:
Ir de BI a BY (990 m).
  Entregar paquete 16.
  Camino: BI -> CO -> AZ -> V -> O -> BY

Parada 31:
Ir de BY a Q (390 m).
  Recoger paquete 29.
  Recoger paquete 22.
  Camino: BY -> Q

Parada 32:
Ir de Q a AA (1450 m).
  Entregar paquete 12.
  Camino: Q -> BJ -> CI -> Z -> AA

Parada 33:
Ir de AA a AC (900 m).
  Entregar paquete 17.
  Camino: AA -> Z -> CI -> AC

Parada 34:
Ir de AC a CD (130 m).
  Entregar paquete 25.
  Camino: AC -> CD

Parada 35:
Ir de CD a BF (710 m).
  Entregar paquete 23.
  Entregar paquete 18.
  Camino: CD -> BA -> AG -> BF

Parada 36:
Ir de BF a AP (760 m).
  Entregar paquete 11.
  Entregar paquete 22.
  Camino: BF -> X -> B -> AP

Parada 37:
Ir de AP a AT (350 m).
  Entregar paquete 15.
  Entregar paquete 14.
  Recoger paquete 27.
  Camino: AP -> AT

Parada 38:
Ir de AT a AQ (200 m).
  Entregar paquete 6.
  Recoger paquete 9.
  Camino: AT -> AQ

Parada 39:
Ir de AQ a CT (350 m).
  Entregar paquete 4.
  Camino: AQ -> BS -> CT

Parada 40:
Ir de CT a T (1420 m).
  Entregar paquete 24.
  Entregar paquete 27.
  Camino: CT -> BK -> CV -> W -> CF -> T

Parada 41:
Ir de T a BE (440 m).
  Entregar paquete 21.
  Camino: T -> BE

Parada 42:
Ir de BE a CP (460 m).
  Entregar paquete 9.
  Camino: BE -> BT -> CP

Parada 43:
Ir de CP a AD (670 m).
  Entregar paquete 5.
  Entregar paquete 13.
  Camino: CP -> BT -> P -> CB -> AD

Parada 44:
Ir de AD a AR (280 m).
  Entregar paquete 8.
  Camino: AD -> AR

Parada 45:
Ir de AR a AJ (550 m).
  Entregar paquete 29.
  Camino: AR -> BB -> AJ

Todos los paquetes fueron recogidos y entregados.
Recorrido total: 30,38 km
```
