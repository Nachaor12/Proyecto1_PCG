using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    //[Header("Tilemap")]
    [SerializeField] private Tilemap RoadsTilemap;
    [SerializeField] private Tilemap WallTilemap;

    [SerializeField] private Tile roadTile;
    [SerializeField] private Tile wallTile;

    public void PaintRoadsTiles(IEnumerable<Vector2Int> roadsPosition)
    {
        ClearTilemaps();
        foreach (var road in roadsPosition) 
        {
            Vector3Int tilePosition = new Vector3Int(road.x, road.y, 0);
            RoadsTilemap.SetTile(tilePosition, roadTile);
        }
    }


    public void PaintWalls(HashSet<Vector2Int> roadsPosition)
    {
        HashSet<Vector2Int> sidewalkPositions = FindWallsInDirections(roadsPosition);

        foreach (var position in sidewalkPositions)
        {
            Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
            WallTilemap.SetTile(tilePosition, wallTile);
        }
    }

    public HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> roadsPosition)
    {
        HashSet<Vector2Int> wallPosition = new HashSet<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                                    new Vector2Int(1,1), new Vector2Int(-1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1) };

        foreach (var position in roadsPosition)
        {
            foreach (var dir in directions)
            {
                Vector2Int neighbor = position + dir;
                if (!roadsPosition.Contains(neighbor))
                {
                    wallPosition.Add(neighbor);
                }
            }
        }

        return wallPosition;
    }


    public void ClearTilemaps()
    {
        RoadsTilemap.ClearAllTiles();
        WallTilemap.ClearAllTiles();
    }

}
