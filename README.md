# **IAV - Navegación**

## Replica el formato de documentación habitual

Hay quien implementa el A* con una estructura de registro de nodo muy simple (el identificador del nodo y el coste f), sólo usa lista de apertura, se apoya en tener toda la información completa del grafo a mano (costes incluidos) y como estructura de datos auxiliar usa una cola de prioridad muy simple.

Según el pseudocódigo que plantea Millington, la estructura de registro de nodo es más rica (identificador del nodo, conexión con el nodo padre, coste g y coste f), se usa una lista de apertura y una lista de cierre, no se asume que toda la información del grafo esté disponible y la cola de prioridad se implementa con BinaryHeap.
