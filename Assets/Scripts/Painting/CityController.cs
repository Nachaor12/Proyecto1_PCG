using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
    [SerializeField] private MapGenerator visualizer;
    [SerializeField] private int numberOfAgents = 1;
    [SerializeField] private int walkLength = 30;
    [SerializeField] private int Pc = 5;

    [ContextMenu("Generate City")]

    private void Start()
    {
        RunProceduralGeneration();
    }

    public void RunProceduralGeneration()
    {
        //var floorPositions = Walker.GenerateDungeon(Vector2Int.zero, numberOfAgents, walkLength);

        var floorPositions = Walker.GenerateMap(Vector2Int.zero, ref Pc, walkLength);

        visualizer.PaintRoadsTiles(floorPositions);
        visualizer.PaintWalls(floorPositions);
    }
}