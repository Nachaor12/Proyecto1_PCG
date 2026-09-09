using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
    [SerializeField] private MapGenerator visualizer;
    [SerializeField] private int numberOfAgents = 1;
    [SerializeField] private int iterations = 30;
    [SerializeField] private int Pc = 5;
    [SerializeField] private int minSteps;
    [SerializeField] private int maxSteps;
    [SerializeField] private int minRoomScale;
    [SerializeField] private int maxRoomScale;

    [SerializeField] private Vector2Int limitesMinimos = new Vector2Int(-25, -25);
    [SerializeField] private Vector2Int limitesMaximos = new Vector2Int(25, 25);

    [ContextMenu("Generate City")]

    private void Start()
    {
        RunProceduralGeneration();
    }

    public void RunProceduralGeneration()
    {
        //var floorPositions = Walker.GenerateDungeon(Vector2Int.zero, numberOfAgents, walkLength);

        var floorPositions = Walker.GenerateMap(Vector2Int.zero, ref Pc, iterations, limitesMinimos, limitesMaximos, minSteps, maxSteps, minRoomScale, maxRoomScale);

        //visualizer.PaintRoadsTiles(floorPositions);
        //visualizer.PaintExtraRoad(floorPositions);
        //visualizer.PaintSidewalk(floorPositions);


        // 1. Pintar carreteras centrales
        visualizer.PaintRoadsTiles(floorPositions);

        // 2. Pintar carreteras extra y guardar sus posiciones
        HashSet<Vector2Int> extraRoads = visualizer.PaintExtraRoad(floorPositions);

        // 3. Unir carreteras centrales y carreteras extra
        HashSet<Vector2Int> totalRoads = new HashSet<Vector2Int>(floorPositions);
        totalRoads.UnionWith(extraRoads);

        // 4. Pintar aceras alrededor del bloque total de carreteras
        visualizer.PaintSidewalk(totalRoads);
    }
}