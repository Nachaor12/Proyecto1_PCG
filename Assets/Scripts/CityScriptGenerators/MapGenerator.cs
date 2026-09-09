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
    [SerializeField] private Tile lowBuildingTile;
    [SerializeField] private Tile mediumBuildingTile;
    [SerializeField] private Tile highBuildingTile;


    [Header("Tiles de Misión ")]
    [SerializeField] private Tile startTile;    // 'S'
    [SerializeField] private Tile cTile;   // 'C'
    [SerializeField] private Tile eTile;  // 'E'
    [SerializeField] private Tile rTile; // 'R'
    [SerializeField] private Tile kTile;      // 'K'
    [SerializeField] private Tile lTile;     // 'L'
    [SerializeField] private Tile goalTile;     // 'G'


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

    /*public void PaintBuildings(HashSet<Vector2Int> occupiedPositions, float[,] noiseMap, Vector2Int boundsMin, int resolution, int buildingDepth)
    {
        // 1. Encuentra las posiciones vacías expandiéndose varias casillas hacia afuera
        HashSet<Vector2Int> buildingSpots = FindAvailableBuildingSpots(occupiedPositions, buildingDepth);

        // 2. Asigna la baldosa correspondiente según el Value Noise para cada casilla del bloque
        foreach (var pos in buildingSpots)
        {
            int noiseX = pos.x - boundsMin.x;
            int noiseY = pos.y - boundsMin.y;

            if (noiseX >= 0 && noiseX < resolution && noiseY >= 0 && noiseY < resolution)
            {
                float noiseValue = noiseMap[noiseY, noiseX];
                Tile chosenTile = GetBuildingTileByNoise(noiseValue);

                if (chosenTile != null)
                {
                    Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);
                    BuildingsTilemap.SetTile(tilePosition, chosenTile);
                }
            }
        }
    }*/

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
            return lowBuildingTile;
        else if (value < 0.7f)
            return mediumBuildingTile;
        else
            return highBuildingTile;
    }

    public void ClearAllTilemaps()
    {
        if (RoadsTilemap != null) RoadsTilemap.ClearAllTiles();
        if (SidewalkTilemap != null) SidewalkTilemap.ClearAllTiles();
        if (BuildingsTilemap != null) BuildingsTilemap.ClearAllTiles();
        if (MissionTilemap != null) MissionTilemap.ClearAllTiles();
    }

    // Selecciona la baldosa según el rango de altura devuelto por Value Noise
    /*public void ClearTilemaps()
    {
        RoadsTilemap.ClearAllTiles();
        SidewalkTilemap.ClearAllTiles();
    }*/


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
            case 'S': return startTile;
            case 'C': return cTile;
            case 'E': return eTile;
            case 'R': return rTile;
            case 'K': return kTile;
            case 'L': return lTile;
            case 'G': return goalTile;
            default: return null;
        }
    }



}
