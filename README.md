# **IAV - Práctica 2 - El hilo de Ariadna - Navegación**

## ***- Autores***
- Sergio José Alfonso Rojas - sergialf@ucm.es 
- Daniel Illanes Morillas - dailla01@ucm.es
- Aaron Nauzet Moreno Sosa - aarmor01@ucm.es

## ***- Introducción Enunciado***
El objetivo de esta práctica reside en la simulación del mito clásico de **Teseo y el Minotauro**. Para ello, utilizaremos agentes inteligentes para el control de ambos personajes, además de definir un entorno que simula el laberinto donde ambos se enfrentan.

Este **laberinto** seguirá un *esquema de grafo*, que dividirá el mapa en baldosas/casillas, y dispondrá de caminos con *secciones de anchura variable*, es decir, siendo estos anchos o estrechos.

Con respecto a los agentes inteligentes, el primero, **Teseo**, tendrá dos formas de movimiento:

- Por defecto, Teseo será *controlado manualmente* por el jugador, siendo capaz de moverse utilizando las **flechas de teclado**.

- Cuando el jugador pulse la **barra espaciadora**, el control de Teseo dejará de ser manual, pasando a ser *automático* (hasta que se deje de pulsar el espacio). La ejecución de este comportamiento derivará en Teseo siguiendo el **hilo de Ariadna**, que aparecerá en pantalla como un hilo que muestra el *camino* que Teseo deberá seguir para salir lo antes posible del laberinto (y que desaparecerá tras volver al movimiento manual).

- Además, cabe destacar que, mediante la pulsación de la **tecla s**, el movimiento automático de Teseo se verá modificado por un *algoritmo de suavizado* para obtener un movimiento más realista y orgánico.

El otro agente inteligente presente en el prototipo, el **Minotauro**, empezará situado en el centro del mapa, moviéndose por este con un comportamiento de *merodeo* mientras no tenga a Teseo en su campo de visión.  En el momento en el que le detecte, el Minotauro procederá a **perseguir** a Teseo.


## ***- Punto de Partida***
El punto de partida del proyecto nos presenta un mapa generado por la clase **GraphGrid**, apoyada por las clases auxiliares **Graph**,**Edge** y **Vertex**. Este script se encargará de cargar el mapa mediante un archivo de texto, generando así un grafo de división de baldosas sobre el que aplicar los algoritmos de búsqueda de caminos. Además, permitirá *ajustar* tanto el tamaño de cada celda como los costes mínimos y máximos de las mismas, permitiendo así adaptarlas a los agentes en escena y a los algoritmos de búsqueda de caminos.

Por otro lado, disponemos de un objeto **TesterGraph**, con un script del mismo nombre. Este script se encargará de mostrar mediante el editor de escena de Unity el camino generado entre dos nodos del grafo por diferentes tipos de recorrido *(el hilo de Ariadna)*, tomando el input del ratón como referencia para seleccionar los nodos. En un principio se dispone de búsqueda en anchura y en profundidad, así como el esqueleto de los *algoritmos A** y de suavizado de caminos, que no se encuentran implementados desde el inicio y deberán completarse más adelante.

Además, se cuenta con una implementación de **montículos binarios** que se podrá utilizar en caso de necesitar colas de prioridad para los algoritmos de búsqueda de caminos.

## ***- Diseño original de Implementación***
Para comenzar con la implementación, analizando los comportamientos requeridos por el **Minotauro**, queda claro que reutilizaremos los comportmientos de [Merodear](https://github.com/IAV22-G12/P1/blob/main/Assets/Scripts/Comportamientos/Merodear.cs) y [Llegada](https://github.com/IAV22-G12/P1/blob/main/Assets/Scripts/Comportamientos/Llegada.cs) usados en la práctica anterios.

Además, con respecto al tema de reutilización, reusaremos de la práctica 1 el [movimiento del flautista](https://github.com/IAV22-G12/P1/blob/main/Assets/Scripts/Comportamientos/ControlJugador.cs), para el movimiento manual de Teseo. También reusaremos modelos y animaciones (créditos en el último apartado).

Por su parte, Teseo (y, en consecuencia, el hilo de Ariadna) utilizará el **algoritmo A*** para encontrar el **camino óptimo** (permitiendo al usuario cambiar las heurísticas del algoritmo entre euclídea y Manhattan). El siguiente pseudocódigo mostrará la estructura principal de este algoritmo: *[pág. 219-222]*
```
function pathfindAStar(graph: Graph,
                            start: Node,
                            nd: Node, 
                            heuristic: Heuristic 
                            ) -> Connection[]: 
	    # This structure is used to keep track of the 
        # information we need for each node.

        class NodeRecord:
                node: Node
                connection: Connection
                costSoFar: float
                estimatedTotalCost: float

        # Initialize the record for the start node.
        startRecord = new NodeRecord()
        startRecord.node = start
        startRecord.connection = null
        startRecord.costSoFar = 0
        startRecord.estimatedTotalCost = heuristic.estimate(start)

        # Initialize the open and closed lists.
        open = new PathfindingList() 
        open += startRecord
        closed = new PathfindingList()

        # Iterate through processing each node.
        while length(open) > 0:
            # Find the smallest element in the open list (using the estimatedTotalCost).
            current = open.smallestElement()

            # If it is the goal node, then terminate.
            if current.node == goal:
                break

            # Otherwise get its outgoing connections.
            connections = graph.getConnections(current)

            # Loop through each connection in turn.
            for connection in connections:
                # Get the cost estimate for the end node.
                endNode = connection.getToNode()
                endNodeCost = current.costSoFar + connection.getCost()

                # If the node is closed we may have to skip, or remove it from the
                # closed list.
                if closed.contains(endNode):
                    # Here we find the record in the closed list corresponding to
                    # the endNode.
                    endNodeRecord = closed.find(endNode)

                    # If we didn’t find a shorter route, skip.
                    if endNodeRecord.costSoFar <= endNodeCost:
                        continue

                    # Otherwise remove it from the closed list.
                    closed -= endNodeRecord

                    # We can use the node’s old cost values to calculate its
                    # heuristic without calling the possibly expensive heuristic 
                    # function.
                    endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar

                # Skip if the node is open and we’ve not found a better route.
                else if open.contains(endNode): 
                    # Here we find the record in the open list corresponding to the
                    # endNode
                    endNodeRecord = open.find(endNode) 

                    # If our route is no better, then skip.
                    if endNodeRecord.costSoFar <= endNodeCost: 
                        continue

                    # Again, we can calculate its heuristic.
                    endNodeHeuristic = endNodeRecord.cost - endNodeRecord.costSoFar

                # Otherwise we know we’ve got an unvisited node, so make a
                # record for it.
                else:
                    endNodeRecord = new NodeRecord()
                    endNodeRecord.node = endNode 

                    # We’ll need to calculate the heuristic value using the function, 
                    # since we don’t have an existing record to use.
                    endNodeHeuristic = heuristic.estimate(endNode)

                # We’re here if we need to update the node. Update the cost, estimate
                # and connection.
                endNodeRecord.cost = endNodeCost
                endNodeRecord.connection = connection
                endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic

                # And add it to the open list.
                if not open.contains(endNode):
                    open += endNodeRecord

            # We’ve finished looking at the connections for the current node, so add it to
            # the closed list and remove it from the open list.
            open -= current
            closed += current

        # We’re here if we’ve either found the goal, or if we’ve no more nodes to search, find
        # which.
        if current.node != goal:
            # We’ve run out of nodes without finding the goal, so there’s no solution.
            return null
        else: 
            # Compile the list of connections in the path. 
            path = []

            # Work back along the path, accumulating connections. 
            while current.node != start:
                path += current.connection
                current = current.connection.getFromNode()

            # Reverse the path, and return it.
            return reverse(path) 
```

Además, se utilizará un **algoritmo de suavizado** de camino, para hacer el movimiento de Teseo más "óptimo" en cuanto a espacio desplazado (y que podrá ser activado o desactivado a decisión del usuario usando la tecla ‘S’). El pseudocódigo se este algoritmo es de esta manera: *[pág 255-256]*
```
function smoothPath(inputPath: Vector[]) -> Vector[]:
    # If the path is only two nodes long, then we can’t smooth it, so return.
    if len(inputPath) == 2:
        return inputPath 

    # Compile an output path.
    outputPath = [inputPath[0]]

    # Keep track of where we are in the input path. We start at 2, because we assume
    # two adjacent nodes will pass the ray cast.
    inputIndex: int = 2

    # Loop until we find the last item in the input.
    while inputIndex < len(inputPath) - 1:
        # Do the ray cast.
        fromPt = outputPath[len(outputPath) - 1] 
        toPt = inputPath[inputIndex]
        if not rayClear(fromPt, toPt):
            # The ray cast failed, add the last node that passed to the output list.
            outputPath += inputPath[inputIndex - 1]

        # Consider the next node.
        inputIndex ++

    # We’ve reached the end of the input path, add the end node to the output and return
    # it.
    outputPath += inputPath[len(inputPath) - 1]

    return outputPath
```

Además, se incluirá un generador de laberintos aleatorios: *[pág 706-708]*
```
function maze(level: Level, start: Location): 
    # A stack of locations we can branch from.
    locations = [start]
    level.startAt(start)

    while locations:
        current = locations.top()

    # Try to connect to a neighboring location.
    next = level.makeConnection(current)
    if next:
    # If successful, it will be our next iteration.
        locations.push(next)
    else:
        locations.pop()

	# GraphGrid generates map using level [ADDED PART]
	graphGrid.generateRandomMap(level)
```
```
class Level:
    function startAt(location: Location)
    function makeConnection(location: Location) -> Location
```
```
class Location:
    x: int
    y: int

class Connections:
    inMaze: bool = false
    directions: bool[4] = [false, false, false, false]

class GridLevel:
    # dx, dy, and index into the Connections.directions array.
    NEIGHBORS = [(1, 0, 0), (0, 1, 1), (0, -1, 2), (-1, 0, 3)]

    width: int
    height: int
    cells: Connections[width][height]

    function startAt(location: Location):
        cells[location.x][location.y].inMaze = true

    function canPlaceCorridor(x: int, y: int, dirn :int) -> bool:
        # Must be in-bounds and not already part of the maze.
        return 0 <= x < width and 0 <= y < height and not cells[x][y].inMaze

    function makeConnection(location: Location) -> Location:
    # Consider neighbors in a random order.
    neighbors = shuffle(NEIGHBORS)

    x = location.x
    y = location.y
    for (dx, dy, dirn) in neighbors:
        # Check if that location is valid.
        nx = x + dx
        ny = y + dy
        fromDirn = 3 - dirn
        if canPlaceCorridor(nx, ny, fromDirn):
            # Perform the connection.
            cells[x][y].directions[dirn] = true
            cells[nx][ny].inMaze = true
            cells[nx][ny].directions[fromDirn] = true
            return Location(nx, ny)

    # null of the neighbors were valid.
    return null
```

## ***- Diseño final de la implementación***

Tras la finalización de la práctica, se han cambiado bastantes ideas desde el planteamiento original de la implementación. En primer lugar, el pseudocódigo del algoritmo A* ha sido obtenido de Wikipedia, siendo así más fácil de adaptar en nuestro código. El pseudocódigo es el siguiente:

```
function A_Star(start, goal, h)
    // The set of discovered nodes that may need to be (re-)expanded.
    // Initially, only the start node is known.
    // This is usually implemented as a min-heap or priority queue rather than a hash-set.
    openSet := {start}

    // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
    // to n currently known.
    cameFrom := an empty map

    // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
    gScore := map with default value of Infinity
    gScore[start] := 0

    // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
    // how short a path from start to finish can be if it goes through n.
    fScore := map with default value of Infinity
    fScore[start] := h(start)

    while openSet is not empty
        // This operation can occur in O(Log(N)) time if openSet is a min-heap or a priority queue
        current := the node in openSet having the lowest fScore[] value
        if current = goal
            return reconstruct_path(cameFrom, current)

        openSet.Remove(current)
        for each neighbor of current
            // d(current,neighbor) is the weight of the edge from current to neighbor
            // tentative_gScore is the distance from start to the neighbor through current
            tentative_gScore := gScore[current] + d(current, neighbor)
            if tentative_gScore < gScore[neighbor]
                // This path to neighbor is better than any previous one. Record it!
                cameFrom[neighbor] := current
                gScore[neighbor] := tentative_gScore
                fScore[neighbor] := tentative_gScore + h(neighbor)
                if neighbor not in openSet
                    openSet.add(neighbor)

    // Open set is empty but goal was never reached
    return failure

```

Por otro lado, el minotauro también ha sufrido cambios en su comportamiento. El merodeo que habíamos planteado en un primer lugar no es un comportamiento óptimo para un escenario laberíntico. Por ello, hemos aplicado el algoritmo A* de manera que el minotauro avanza hasta un nodo aleatorio del mapa, de forma que merodee de manera más orgánica. Además, cuando el personaje entra en el rango de visión del minotauro, éste último se mueve directamente hacia la posición del mismo aplicando A*. El comportamiento está pensado de forma que el minotauro “carga” contra el jugador, es decir, va hacia la primera posición del jugador que ve, ignorando si el jugador se mueve o no durante ese tiempo. Una vez llega a dicha posición, vuelve a buscar en su rango de visión al jugador. En caso de no encontrarle, vuelve a merodear.

Por último, Teseo se comporta exactamente igual que lo que se planteó en el diseño original de la implementación. El movimiento por teclado es como el utilizado en la práctica anterior. Asimismo, el movimiento automático presionando “Espacio” sigue el algoritmo A*, generando una línea (hilo de Ariadna) entre los nodos para ver su recorrido. 

Como añadido adicional, se puede decidir la cantidad de minotauros que añadir a la escena mediante un panel dropdown en el menú principal, junto a otro panel que permite seleccionar tres mapas en base a su tamaño.
 
## ***- Métricas y opciones de adaptabilidad***

- [Vídeo testeo de métricas y pruebas de funcionamiento](https://youtu.be/hjXVPG05lHc)

En pos de facilitar el testeo del correcto funcionamiento de los agentes, hemos facilitado información que se muestre en pantalla tanto el número de ratas como el número de fps (anclados a un framerate de 30 o 60 fps). Asimismo, se proporcionan una serie de opciones para una mayor comodidad a la hora de hacer pruebas.

Las opciones de adaptabilidad son:

- Alternar framerate entre 30 y 60
- Hacer zoom con la cámara 
- Recargar la escena
- Cambiar la heurística

## ***- Referencias y créditos***

Los assets externos utilizados son de uso público

- “AI for Games”, Ian Millington

**Assets:**
- [kaykit medieval builder pack](https://kaylousberg.itch.io/kaykit-medieval-builder-pack)
- [kaykit dungeon](https://kaylousberg.itch.io/kaykit-dungeon)
- [kaykit animations](https://kaylousberg.itch.io/kaykit-animations)
- [kaykit skeleton pack](https://kaylousberg.itch.io/kaykit-skeletons)