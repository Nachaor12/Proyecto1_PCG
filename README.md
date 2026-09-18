# Generador de mapas para ciudad e interiores estilo RPG

Este proyecto presenta la implementación de varios algoritmos de generación procedural para generación automática de mapas basados en Tiles para videojuegos de vista top-down (por ejemplo, RPG's). Está hecho en Unity 2022

# Desarrolladores
Rodrigo Díaz
Félix Fuentes
Ignacio Alfaro

# Técnicas utilizadas

Utilizamos un Agente (Walker) para generar las calles de la ciudad y los pasillos de los interiores, las habitaciones generadas por el algoritmo corresponden a las intersecciones de las calles y las salas de los interiores respectivamente. Para el contexto de la ciudad de utiliza Value Noise, aprovechando su generación de alturas basado en grillas, ya que los Tiles también utilizan una grilla, detecta los espacios vacíos dejados por el Agente dentro de las dimensiones del mapa y los rellena con edificios de distintos tamaños, y terreno de pasto y agua. Finalmente se usa gramática secuencial para rellenar el mapa con distintos objetos/NPC, donde se les puede asignar un Tile especifico para los caracteres de la gramatica, y estos se posicionan en alguna coordenada que corresponda con el terreno generado por el agente (los objetos/NPC no se generan por sobre los edificios por ejemplo).

# Instrucciones

Puedes descargar el proyecto de Unity y editarlo a tu gusto, para lo que necesitaras instalar la versión de Unity correspondiente. Si quieres probar cómo es la generación y modificar los parámetros relevantes puedes acceder a la versión disponible en Itch Io en el siguiente link:

} https://incguy.itch.io/rpg-delivery-map-generator

Puedes moverte alrededor del mapa generado usando las flechas de dirección o WASD, para abrir el menú con los parametros presiona la tecla espaciadora.

# Parametros relevantes

- Densidad de Calles/Pasillos
Modifica la cantidad de calles o pasillos (dependiendo del contexto), en el caso de la ciudad el resto del espacio será ocupado por edificios.

- Tamaño del mapa
Modifica las dimensiones del mapa, desde el editor de Unity se pueden configurar dimensiones rectangulares, más estás no se generan correctamente. Se recomienda mantener las dimensiones cuadradas así cómo lo hace el menú implementado en la versión de Itch Io. Desde el menú puedes configurar el tamaño desde 10x10 hasta 100x100

- Grosor de las calles/pasillos
Modifica el tamaño de las intersecciones/salas, tiene un mínimo y máximo configurables por separado. A nivel de los algoritmos, esto modifica el tamaño de las habitaciones generadas por el agente (walker), no afecta al value noise o la gramatica.

- Largo de las calles/pasillos
Modifica el largo de las calles/pasillos, tiene un mínimo y máximo configurables por separado. Entre menor sea la distancia entre las habitaciones generadas por el agente será menor, cómo resultado también existiran más intersecciones/salas.

- Espaciado de Edificios.
Modifica la variedad en el tamaño y distancia entre los edificios generados en el contexto de ciudad. A mayor sea, más parecidos y juntos serán los edificios generados, a menor habrá más variedad de tamaños y podrán estar más separados.

- Contexto
Cambia el contexto de Ciudad e Interior.

- Interpolación
Cambia el tipo de interpolación del value noise que genera los edificios.

- Semilla
Cambia la semilla que ocupan los algoritmos, si algún resultado te gusta puedes volver a ingresar la semilla para recuperar esa generación. Esto funciona independientemente de los otros parametros, por lo que dos mapas con la misma semilla podrían ser distintos dependiendo de las otras configuraciones.
