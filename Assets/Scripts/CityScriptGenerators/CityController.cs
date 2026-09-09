using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
    [Header("Agent Method")]
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

    [Header("Value Noise")]
    //[Header("Configuración de Value Noise")]
    [SerializeField] private int seed = 12345;
    [SerializeField] private int latticeSpacing = 5;
    [SerializeField] private InterpolationMode mode = InterpolationMode.Bicubic;



    [ContextMenu("Generate City")]

    private void Start()
    {
        RunProceduralGeneration();
    }

    public void RunProceduralGeneration()
    {
        visualizer.ClearAllTilemaps();

        var floorPositions = Walker.GenerateMap(Vector2Int.zero, ref Pc, iterations, limitesMinimos, limitesMaximos, minSteps, maxSteps, minRoomScale, maxRoomScale);

        //Agent method para general las calles

        // 1. Pintar carreteras centrales
        visualizer.PaintRoadsTiles(floorPositions);
        // 2. Pintar carreteras extra y guardar sus posiciones
        HashSet<Vector2Int> extraRoads = visualizer.PaintExtraRoad(floorPositions);
        // 3. Unir carreteras centrales y carreteras extra
        HashSet<Vector2Int> totalRoads = new HashSet<Vector2Int>(floorPositions);
        totalRoads.UnionWith(extraRoads);
        visualizer.PaintSidewalk(totalRoads);


        //Value Noise para generar edificios 
        // 3. Crear el conjunto total de casillas ocupadas por la infraestructura
        HashSet<Vector2Int> sidewalkPositions = visualizer.FindExtraRoadsInDirections(totalRoads);
        HashSet<Vector2Int> totalOccupiedSpace = new HashSet<Vector2Int>(totalRoads);
        totalOccupiedSpace.UnionWith(sidewalkPositions);

        // 4. Generar el mapa de Value Noise
        int resolution = (limitesMaximos.x - limitesMinimos.x) + 1;
        float[,] noiseMap = ValueNoise.GenerateValueNoiseMap(resolution, latticeSpacing, seed, mode);

        // 5. Pintar Edificios en los espacios disponibles usando el Value Noise
        visualizer.PaintBuildings(totalOccupiedSpace, noiseMap, limitesMinimos, resolution);
    }
}