using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap RoadsTilemap;
    [SerializeField] private Tilemap SidewalkTilemap;
    [SerializeField] private Tilemap BuildingsTilemap; 
    [SerializeField] private Tilemap MissionTilemap;

    [Header("Tiles de Calles")]
    [SerializeField] private Tile roadHorizontalCenterTile;
    [SerializeField] private Tile roadHorizontalUpTile;
    [SerializeField] private Tile roadHorizontalDownTile;
    [SerializeField] private Tile roadVerticalRightTile;
    [SerializeField] private Tile roadVerticalLeftTile;
    [SerializeField] private Tile sidewalkTile;

    [Header("Tiles de Edificios (Según Altura del Noise)")]
    [SerializeField] private Tile lowTerrainTile;
    [SerializeField] private Tile mediumTerrainTile;
    [SerializeField] private Tile highTerrainTile;

    [Header("Tiles de Estructura de Edificio")]
    [SerializeField] private Tile leftBorderBuilding;
    [SerializeField] private Tile rightBorderBuilding;
    [SerializeField] private Tile topBorderRoof;
    [SerializeField] private Tile downBorderRoof;
    [SerializeField] private Tile leftBorderRoof;
    [SerializeField] private Tile rightBorderRoof;
    [SerializeField] private Tile wallBuilding;
    [SerializeField] private Tile roofBuilding;
    [SerializeField] private Tile doorBuilding;
    [SerializeField] private Tile windowsBuilding;


    [Header("Tiles de Misión ")]
    [SerializeField] private Tile startTile;    // 'S'
    [SerializeField] private Tile cTile;        // 'C'
    [SerializeField] private Tile eTile;        // 'E'
    [SerializeField] private Tile rTile;        // 'R'
    [SerializeField] private Tile kTile;        // 'K'
    [SerializeField] private Tile lTile;        // 'L'
    [SerializeField] private Tile goalTile;     // 'G'

    [Header("Tiles de Restaurante")]
    [SerializeField] private Tile restaurantFloorTile;
    [SerializeField] private Tile restaurantWallTile;
    [SerializeField] private Tile restaurantTableTile;
    [SerializeField] private Tile restaurantKitchenTile;
    [SerializeField] private Tile restaurantChairTile;
    [SerializeField] private Tile restauranDecoTile;

    //Para los edificios
    public enum BuildingHeight
    {
        None,
        LowDensity,    // Noise < 0.4f
        MediumDensity, // Noise < 0.7f
        HighDensity    // Noise >= 0.7f
    }

    public enum BuildingType
    {
        House,
        Store, 
        Apartment,
        Office
    }

    public struct BuildingPlacementData
    {
        public Vector2Int position;
        public BuildingHeight height;
        public BuildingType type;
        public Vector2Int size;
        //public List<string> side;
        //public List<string> Color;
    }


    public void PaintRoadsTiles(IEnumerable<Vector2Int> roadsPosition)
    {
        //ClearTilemaps();
        foreach (var road in roadsPosition) 
        {
            Vector3Int tilePosition = new Vector3Int(road.x, road.y, 0);
            RoadsTilemap.SetTile(tilePosition, roadHorizontalCenterTile);
        }
    }

    public HashSet<Vector2Int> PaintExtraRoad(HashSet<Vector2Int> roadsPosition)
    {
        HashSet<Vector2Int> roadsHorizontalUpPos = FindPositionsInDirection(roadsPosition, Vector2Int.up);
        HashSet<Vector2Int> roadsHorizontalDownPos = FindPositionsInDirection(roadsPosition, Vector2Int.down);
        HashSet<Vector2Int> roadsVerticalRightPos = FindPositionsInDirection(roadsPosition, Vector2Int.right);
        HashSet<Vector2Int> roadsVerticalLeftPos = FindPositionsInDirection(roadsPosition, Vector2Int.left);

        foreach (var position in roadsHorizontalUpPos)
        {
            Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
            RoadsTilemap.SetTile(tilePosition, roadHorizontalUpTile);
        }

        foreach (var position in roadsHorizontalDownPos)
        {
            Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
            RoadsTilemap.SetTile(tilePosition, roadHorizontalDownTile);
        }

        foreach(var position in roadsVerticalRightPos)
        {
            Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
            RoadsTilemap.SetTile(tilePosition, roadVerticalRightTile);
        }

        foreach (var position in roadsVerticalLeftPos)
        {
            Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
            RoadsTilemap.SetTile(tilePosition, roadVerticalLeftTile);
        }

        HashSet<Vector2Int> TotalExtraRoads = new HashSet<Vector2Int>(roadsHorizontalUpPos);
        TotalExtraRoads.UnionWith(roadsHorizontalDownPos);
        TotalExtraRoads.UnionWith(roadsVerticalLeftPos);
        TotalExtraRoads.UnionWith(roadsVerticalRightPos);


        return TotalExtraRoads;
    }

    public void PaintSidewalk(HashSet<Vector2Int> roadsPosition)
    {
        HashSet<Vector2Int> sidewalkPositions = FindExtraRoadsInDirections(roadsPosition);

        foreach (var position in sidewalkPositions)
        {
            Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
            SidewalkTilemap.SetTile(tilePosition, sidewalkTile);
        }
    }
    private HashSet<Vector2Int> FindPositionsInDirection(HashSet<Vector2Int> basePositions, Vector2Int direction)
    {
        HashSet<Vector2Int> resultPositions = new HashSet<Vector2Int>();

        foreach (var position in basePositions)
        {
            Vector2Int neighbor = position + direction;
            // Si la casilla no pertenece al camino central, es una casilla válida para carretera extra
            if (!basePositions.Contains(neighbor))
            {
                resultPositions.Add(neighbor);
            }
        }

        return resultPositions;
    }

    public HashSet<Vector2Int> FindExtraRoadsInDirections(HashSet<Vector2Int> roadsPosition)
    {
        HashSet<Vector2Int> tilePosition = new HashSet<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                                    new Vector2Int(1,1), new Vector2Int(-1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1) };

        foreach (var position in roadsPosition)
        {
            foreach (var dir in directions)
            {
                Vector2Int neighbor = position + dir;
                if (!roadsPosition.Contains(neighbor))
                {
                    tilePosition.Add(neighbor);
                }
            }
        }

        return tilePosition;
    }

    public void PaintBuildings(HashSet<Vector2Int> occupiedPositions, float[,] noiseMap, Vector2Int boundsMin, Vector2Int boundsMax, int resolution)
    {
        // Recorremos todo el rango del mapa definido por los límites
        for (int x = boundsMin.x; x <= boundsMax.x; x++)
        {
            for (int y = boundsMin.y; y <= boundsMax.y; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);

                // Si la posición NO está ocupada por carretera ni acera, colocamos un edificio
                if (!occupiedPositions.Contains(currentPos))
                {
                    int noiseX = x - boundsMin.x;
                    int noiseY = y - boundsMin.y;

                    if (noiseX >= 0 && noiseX < resolution && noiseY >= 0 && noiseY < resolution)
                    {
                        float noiseValue = noiseMap[noiseY, noiseX];
                        Tile chosenTile = GetBuildingTileByNoise(noiseValue);

                        if (chosenTile != null)
                        {
                            Vector3Int tilePosition = new Vector3Int(x, y, 0);
                            BuildingsTilemap.SetTile(tilePosition, chosenTile);
                        }
                    }
                }
            }
        }
    }

    private Tile GetBuildingTileByNoise(float value)
    {
        if (value < 0.4f)
            return lowTerrainTile;
        else if (value < 0.7f)
            return mediumTerrainTile;
        else
            return highTerrainTile;
    }

    public void ClearAllTilemaps()
    {
        if (RoadsTilemap != null) RoadsTilemap.ClearAllTiles();
        if (SidewalkTilemap != null) SidewalkTilemap.ClearAllTiles();
        if (BuildingsTilemap != null) BuildingsTilemap.ClearAllTiles();
        if (MissionTilemap != null) MissionTilemap.ClearAllTiles();
    }

    // Método auxiliar que pinta las tiles de un edificio según su posición, tamaño y altura
    private void PaintSingleBuilding(BuildingPlacementData building)
    {
        // Elegimos la tile según la altura definida
        Tile tileToPaint = GetTileByHeight(building.height);

        if (tileToPaint == null) return;

        // Recorremos el área (size.x por size.y) a partir de la posición de origen
        for (int dx = 0; dx < building.size.x; dx++)
        {
            for (int dy = 0; dy < building.size.y; dy++)
            {
                Vector3Int tilePosition = new Vector3Int(
                    building.position.x + dx,
                    building.position.y + dy,
                    0
                );

                // Colocamos la tile en el Tilemap de edificios
                BuildingsTilemap.SetTile(tilePosition, tileToPaint);
            }
        }
    }

    // Mapea el enum BuildingHeight a las Tiles configuradas en el Inspector
    private Tile GetTileByHeight(BuildingHeight height)
    {
        switch (height)
        {
            case BuildingHeight.LowDensity: return lowTerrainTile;
            case BuildingHeight.MediumDensity: return mediumTerrainTile;
            case BuildingHeight.HighDensity: return highTerrainTile;
            default: return null;
        }
    }


    // Devuelve la densidad según el valor del Value Noise
    private BuildingHeight GetBuildingHeightByNoise(float noiseValue)
    {
        if (noiseValue < 0.2f) return BuildingHeight.None; // Espacio libre/parque
        if (noiseValue < 0.5f) return BuildingHeight.LowDensity;
        if (noiseValue < 0.8f) return BuildingHeight.MediumDensity;
        return BuildingHeight.HighDensity;
    }

    // Opcional: Define cuántas tiles mide la base según la altura
    /*private Vector2Int GetBuildingSizeByHeight(BuildingHeight height)
    {
        switch (height)
        {
            case BuildingHeight.HighDensity: return new Vector2Int(2, 2); // Rascacielos ocupan 2x2
            case BuildingHeight.MediumDensity: return new Vector2Int(2, 1); // Comercios/Oficinas 2x1
            case BuildingHeight.LowDensity: return new Vector2Int(1, 1); // Casas 1x1
            default: return new Vector2Int(1, 1);
        }
    }*/

    private Vector2Int GetBuildingSizeByHeight(BuildingHeight height)
    {
        switch (height)
        {
            case BuildingHeight.HighDensity: return new Vector2Int(6, 9);   // Rascacielos (Roof = 3 tiles)
            case BuildingHeight.MediumDensity: return new Vector2Int(4, 6); // Mediano (Roof = 2 tiles)
            case BuildingHeight.LowDensity: return new Vector2Int(3, 3);    // Pequeño (Roof = 1 tile)
            default: return new Vector2Int(3, 3);
        }
    }

    // Selecciona un tipo de edificio compatible con la densidad/altura
    private BuildingType GetRandomBuildingType(BuildingHeight height)
    {
        switch (height)
        {
            case BuildingHeight.LowDensity:
                return Random.value > 0.3f ? BuildingType.House : BuildingType.Store;

            case BuildingHeight.MediumDensity:
                return Random.value > 0.5f ? BuildingType.Store : BuildingType.Apartment;

            case BuildingHeight.HighDensity:
                return Random.value > 0.4f ? BuildingType.Office : BuildingType.Apartment;

            default:
                return BuildingType.House;
        }
    }

    // Revisa que ninguna celda del rectángulo esté ocupada o fuera del mapa
    private bool CanPlaceBuilding(Vector2Int origin, Vector2Int size, HashSet<Vector2Int> occupied, Vector2Int boundsMin, Vector2Int boundsMax)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dy = 0; dy < size.y; dy++)
            {
                Vector2Int checkPos = new Vector2Int(origin.x + dx, origin.y + dy);

                if (checkPos.x > boundsMax.x || checkPos.y > boundsMax.y) return false;
                if (occupied.Contains(checkPos)) return false;
            }
        }
        return true;
    }

    // Marca las celdas ocupadas por la base para no encimar otros edificios
    private void MarkAreaAsOccupied(Vector2Int origin, Vector2Int size, HashSet<Vector2Int> occupied)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dy = 0; dy < size.y; dy++)
            {
                occupied.Add(new Vector2Int(origin.x + dx, origin.y + dy));
            }
        }
    }

    // Para crear los edificios

    public void GenerateAndPaintBuildings(HashSet<Vector2Int> occupiedPositions, float[,] noiseMap, Vector2Int boundsMin, Vector2Int boundsMax, int resolution)
    {
        List<BuildingPlacementData> buildings = DefineBuildingSpace(occupiedPositions, noiseMap, boundsMin, boundsMax, resolution);
        MakeBuildings(buildings);
    }

    private void MakeBuildings(List<BuildingPlacementData> buildingList)
    {
        foreach (var building in buildingList)
        {
            PaintProceduralBuilding(building);
        }
    }

    private List<BuildingPlacementData> DefineBuildingSpace(HashSet<Vector2Int> occupiedPositions, float[,] noiseMap, Vector2Int boundsMin, Vector2Int boundsMax, int resolution)
    {
        List<BuildingPlacementData> buildingsList = new List<BuildingPlacementData>();
        string[] availableColors = { "Red", "Blue", "Green", "Yellow", "Gray" };
        string[] availableSides = { "North", "South", "East", "West" };

        for (int x = boundsMin.x; x <= boundsMax.x; x++)
        {
            for (int y = boundsMin.y; y <= boundsMax.y; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                // Si la celda de origen está ocupada, la saltamos

                if (occupiedPositions.Contains(currentPos)) continue;

                int noiseX = x - boundsMin.x;
                int noiseY = y - boundsMin.y;

                if (noiseX >= 0 && noiseX < resolution && noiseY >= 0 && noiseY < resolution)
                {
                    float noiseValue = noiseMap[noiseY, noiseX];

                    // Determinar Altura/Densidad por ruido

                    BuildingHeight height = GetBuildingHeightByNoise(noiseValue);
                    if (height == BuildingHeight.None) continue;

                    // Determinar Tamaño según la altura
                    Vector2Int buildingSize = GetBuildingSizeByHeight(height);

                    // Validar que todo el bloque (size.x * size.y) esté libre
                    if (CanPlaceBuilding(currentPos, buildingSize, occupiedPositions, boundsMin, boundsMax))
                    {
                        // Generar variables aleatorias
                        BuildingType randomType = GetRandomBuildingType(height);
                        string randomColor = availableColors[Random.Range(0, availableColors.Length)];
                        string randomSide = availableSides[Random.Range(0, availableSides.Length)];

                        // Crear la estructura de datos
                        BuildingPlacementData newBuilding = new BuildingPlacementData
                        {
                            position = currentPos,
                            height = height,
                            type = randomType,
                            size = buildingSize,
                            // Color = new List { randomColor },
                            // side = new List { randomSide }
                        };
                        buildingsList.Add(newBuilding);

                        // Bloquear todas las celdas de este edificio en occupiedPositions
                        MarkAreaAsOccupied(currentPos, buildingSize, occupiedPositions);
                    }
                }
            }
        }
        return buildingsList;
    }

    private void PaintProceduralBuilding(BuildingPlacementData building)
    {
        int width = building.size.x;
        int height = building.size.y;

        // El techo ocupa el tercio superior de la altura (mínimo 1 fila)
        int roofHeight = Mathf.Max(1, height / 3);
        int wallHeight = height - roofHeight;

        int doorX = width / 2; // Posición horizontal centrada para la puerta

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePos = new Vector3Int(building.position.x + x, building.position.y + y, 0);
                Tile tileToSet;

                bool isLeftEdge = (x == 0);
                bool isRightEdge = (x == width - 1);

                // Capa de techo (Filas superiores) 
                if (y >= wallHeight)
                {
                    bool isRoofBottom = (y == wallHeight);
                    bool isRoofTop = (y == height - 1);

                    if (isLeftEdge)
                        tileToSet = leftBorderRoof;
                    else if (isRightEdge)
                        tileToSet = rightBorderRoof;
                    else if (isRoofTop)
                        tileToSet = topBorderRoof;
                    else if (isRoofBottom)
                        tileToSet = downBorderRoof;
                    else
                        tileToSet = roofBuilding;
                }
                // pared (Filas inferiores) 
                else
                {
                    // Puerta de entrada centrada en la base
                    if (y == 0 && x == doorX)
                    {
                        tileToSet = doorBuilding;
                    }
                    else if (isLeftEdge)
                    {
                        tileToSet = leftBorderBuilding;
                    }
                    else if (isRightEdge)
                    {
                        tileToSet = rightBorderBuilding;
                    }
                    else
                    {
                        // Colocar ventanas intercaladas en el espacio interno de la pared
                        bool isWindowRow = (y % 2 == 1);
                        bool isWindowCol = (x % 2 == 1);

                        if (isWindowRow && isWindowCol && windowsBuilding != null)
                            tileToSet = windowsBuilding;
                        else
                            tileToSet = wallBuilding;
                    }
                }

                if (tileToSet != null)
                {
                    BuildingsTilemap.SetTile(tilePos, tileToSet);
                }
            }
        }
    }

    


    // Código de misiones

    public void PaintMissionObjectives(List<MissionGenerator.MissionObjective> objectives)
    {
        if (MissionTilemap == null) return;
        MissionTilemap.ClearAllTiles();

        foreach (var obj in objectives)
        {
            Tile chosenTile = GetMissionTileBySymbol(obj.symbol);

            if (chosenTile != null)
            {
                Vector3Int tilePos = new Vector3Int(obj.position.x, obj.position.y, 0);
                MissionTilemap.SetTile(tilePos, chosenTile);
            }
            else
            {
                Debug.LogWarning($"[MapGenerator] No hay una Tile asignada para el símbolo '{obj.symbol}'");
            }
        }
    }

    private Tile GetMissionTileBySymbol(char symbol)
    {
        switch (symbol)
        {
            // Objetivos de Ciudad
            case 'S': return startTile;
            case 'C': return cTile;
            case 'E': return eTile;
            case 'R': return rTile;
            case 'K': return kTile;
            case 'L': return lTile;
            case 'G': return goalTile;

            // Objetos de Restaurante 
            case 'M': return restaurantTableTile;   // M = Mesa
            case 'O': return restaurantKitchenTile; // O = Cocina 
            case 'H': return restaurantChairTile; // H = Silla 
            case 'D': return restauranDecoTile; // D = Decoracion

            default: return null;
        }
    }

    // Funciones para contexto de restaurante (interiores)

    public void PaintRestaurantFloor(HashSet<Vector2Int> floorPositions)
    {
        foreach (var pos in floorPositions)
        {
            Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);
            RoadsTilemap.SetTile(tilePosition, restaurantFloorTile); // Usamos RoadsTilemap para el suelo base
        }
    }

    public void PaintRestaurantWalls(HashSet<Vector2Int> wallPositions)
    {
        foreach (var pos in wallPositions)
        {
            Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);
            // Pintamos las paredes en el Tilemap de edificios para que colisionen correctamente
            BuildingsTilemap.SetTile(tilePosition, restaurantWallTile);
        }
    }

    public void GenerateAndPaintRestaurantProps(HashSet<Vector2Int> occupiedPositions, float[,] noiseMap, Vector2Int boundsMin, Vector2Int boundsMax, int resolution)
    {
        for (int x = boundsMin.x; x <= boundsMax.x; x++)
        {
            for (int y = boundsMin.y; y <= boundsMax.y; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);

                // Si no es un pasillo, colocamos paredes o muebles
                if (!occupiedPositions.Contains(currentPos))
                {
                    int noiseX = x - boundsMin.x;
                    int noiseY = y - boundsMin.y;

                    if (noiseX >= 0 && noiseX < resolution && noiseY >= 0 && noiseY < resolution)
                    {
                        float noiseValue = noiseMap[noiseY, noiseX];
                        Tile chosenTile = GetRestaurantPropByNoise(noiseValue);

                        if (chosenTile != null)
                        {
                            Vector3Int tilePosition = new Vector3Int(x, y, 0);
                            BuildingsTilemap.SetTile(tilePosition, chosenTile); // Usamos BuildingsTilemap para los obstáculos/mesas
                        }
                    }
                }
            }
        }
    }

    private Tile GetRestaurantPropByNoise(float noiseValue)
    {
        // Distribución basada en ruido para agrupar elementos lógicamente
        if (noiseValue < 0.35f)
            return restaurantTableTile; // Zonas de baja densidad (Mesas para clientes)
        else if (noiseValue < 0.65f)
            return restaurantWallTile;  // Zonas de densidad media (Paredes/Divisiones interiores)
        else
            return restaurantKitchenTile; // Zonas de alta densidad (Área de cocina/bar)
    }
}