using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
    public enum GenerationContext
    {
        City,
        Restaurant
    }

    [Header("Contexto de Generación")]
    [SerializeField] private GenerationContext currentContext = GenerationContext.City;

    [Header("Generators")]
    [SerializeField] private MapGenerator visualizer;
    [SerializeField] private MissionGenerator missionGen;

    [Header("Agent Method")]
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
    [SerializeField] private int seed = 12345;
    [SerializeField] private int latticeSpacing = 5;
    [SerializeField] private InterpolationMode mode = InterpolationMode.Bicubic;

    [Header("Configuración de Edificios / Entorno")]
    [SerializeField] private int buildingDepth = 3;

    [Header("Player Settings")]
    [SerializeField] private PlayerController playerPrefab;
    private PlayerController activePlayer;

    [ContextMenu("Generate Environment")]

    private void Start()
    {
        //SetIterations(999);
        RunProceduralGeneration();
    }

    public void RunProceduralGeneration()
    {
        visualizer.ClearAllTilemaps();

        if (currentContext == GenerationContext.City)
        {
            GenerateCity();
        }
        else if (currentContext == GenerationContext.Restaurant)
        {
            GenerateRestaurant();
        }
    }

    public void GenerateCity()
    {
        // Generar caminos
        var floorPositions = Walker.GenerateMap(
            Vector2Int.zero, ref Pc, iterations,
            limitesMinimos, limitesMaximos,
            minSteps, maxSteps, minRoomScale, maxRoomScale
        );

        // Pintar vías y aceras
        visualizer.PaintRoadsTiles(floorPositions);
        HashSet<Vector2Int> extraRoads = visualizer.PaintExtraRoad(floorPositions);

        HashSet<Vector2Int> totalRoads = new HashSet<Vector2Int>(floorPositions);
        totalRoads.UnionWith(extraRoads);

        visualizer.PaintSidewalk(totalRoads);

        // Registrar todo el espacio ocupado por calles y aceras
        HashSet<Vector2Int> sidewalkPositions = visualizer.FindExtraRoadsInDirections(totalRoads);
        HashSet<Vector2Int> totalOccupiedSpace = new HashSet<Vector2Int>(totalRoads);
        totalOccupiedSpace.UnionWith(sidewalkPositions);

        // Calcular Value Noise
        int resolution = (limitesMaximos.x - limitesMinimos.x) + 1;
        float[,] noiseMap = ValueNoise.GenerateValueNoiseMap(resolution, latticeSpacing, seed, mode);

        // Rellenar TODO el mapa sobrante con edificios usando los límites
        visualizer.PaintBuildings(totalOccupiedSpace, noiseMap, limitesMinimos, limitesMaximos, resolution);

        // Nuevo visualizer para dibujar edificios, quitar el de arriba 
        visualizer.GenerateAndPaintBuildings(totalOccupiedSpace, noiseMap, limitesMinimos, limitesMaximos, resolution);

        // Integrar la Gramática de Misiones sobre las Aceras
        if (missionGen != null)
        {
            string missionStr = missionGen.GenerateMissionString(MissionGenerator.MissionContext.City);
            var objectives = missionGen.AssignObjectivesToPositions(missionStr, sidewalkPositions);

            // Pinta cada tarea con su Tile específica según el switch
            visualizer.PaintMissionObjectives(objectives);
        }

        SpawnPlayer(totalOccupiedSpace);
    }

    private void GenerateRestaurant()
    {
        //Generar el suelo (equivalente a las calles principales)
        var floorPositions = Walker.GenerateMap(
            Vector2Int.zero, ref Pc, iterations,
            limitesMinimos, limitesMaximos,
            minSteps, maxSteps, minRoomScale, maxRoomScale
        );

        // Generar las paredes bordeando el suelo (equivalente a cómo buscabas aceras)
        HashSet<Vector2Int> wallPositions = visualizer.FindExtraRoadsInDirections(floorPositions);

        // Generar paredes en el perímetro absoluto para asegurar 
        // que el interior quede totalmente encerrado en los límites máximos de la grilla.
        for (int x = limitesMinimos.x - 1; x <= limitesMaximos.x + 1; x++)
        {
            wallPositions.Add(new Vector2Int(x, limitesMinimos.y - 1));
            wallPositions.Add(new Vector2Int(x, limitesMaximos.y + 1));
        }
        for (int y = limitesMinimos.y - 1; y <= limitesMaximos.y + 1; y++)
        {
            wallPositions.Add(new Vector2Int(limitesMinimos.x - 1, y));
            wallPositions.Add(new Vector2Int(limitesMaximos.x + 1, y));
        }

        // Aseguramos que ninguna pared reemplace un área de suelo transitable
        wallPositions.ExceptWith(floorPositions);

        //Pintar Suelos y Paredes
        visualizer.PaintRestaurantFloor(floorPositions);
        visualizer.PaintRestaurantWalls(wallPositions);

        // Distribuir Mesas y Cocinas usando el Mission Generator sobre el suelo
        if (missionGen != null)
        {
            string restaurantPropsString = missionGen.GenerateMissionString(MissionGenerator.MissionContext.Restaurant);
            var propObjectives = missionGen.AssignObjectivesToPositions(restaurantPropsString, floorPositions);
            visualizer.PaintMissionObjectives(propObjectives);
        }

        // Instanciar al jugador en una casilla válida del suelo
        SpawnPlayer(floorPositions);
    }

    private void SpawnPlayer(HashSet<Vector2Int> validSpace)
    {
        if (playerPrefab != null && validSpace.Count > 0)
        {
            var enumerator = validSpace.GetEnumerator();
            enumerator.MoveNext();
            Vector2Int startPos = enumerator.Current;

            if (activePlayer == null)
            {
                activePlayer = Instantiate(playerPrefab);
            }

            activePlayer.Initialize(startPos, validSpace);

            if (Camera.main.GetComponent<CameraFollow>() != null)
                Camera.main.GetComponent<CameraFollow>().target = activePlayer.transform;
        }
    }

    // UI
    public void SetIterations(float value) { iterations = (int)value; }
    public void SetMinSteps(float value) { minSteps = (int)value; }
    public void SetMaxSteps(float value) { maxSteps = (int)value; }
    public void SetSeed(string value)
    {
        if (int.TryParse(value, out int parsedSeed))
        {
            seed = parsedSeed;
        }
    }
}