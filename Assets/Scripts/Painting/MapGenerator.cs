using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap RoadsTilemap;
    [SerializeField] private Tilemap SidewalkTilemap;

    [Header("Tiles")]
    [SerializeField] private Tile roadHorizontalCenterTile;
    [SerializeField] private Tile roadHorizontalUpTile;
    [SerializeField] private Tile roadHorizontalDownTile;
    [SerializeField] private Tile roadVerticalRightTile;
    [SerializeField] private Tile roadVerticalLeftTile;
    [SerializeField] private Tile sidewalkTile;

    public void PaintRoadsTiles(IEnumerable<Vector2Int> roadsPosition)
    {
        ClearTilemaps();
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

        //HashSet<Vector2Int> TotalHorizontalRoads = new HashSet<Vector2Int>(roadsHorizontalUpPos);
        //TotalHorizontalRoads.UnionWith(roadsHorizontalDownPos);

        //HashSet<Vector2Int> TotalVerticalRoads = new HashSet<Vector2Int>(roadsVerticalRightPos);
        //TotalVerticalRoads.UnionWith(roadsVerticalLeftPos);

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

    public void ClearTilemaps()
    {
        RoadsTilemap.ClearAllTiles();
        SidewalkTilemap.ClearAllTiles();
    }
}
