using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
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
    //[Header("Configuración de Value Noise")]
    [SerializeField] private int seed = 12345;
    [SerializeField] private int latticeSpacing = 5;
    [SerializeField] private InterpolationMode mode = InterpolationMode.Bicubic;

    [Header("Configuración de Edificios")]
    [SerializeField] private int buildingDepth = 3;

    [Header("Player Settings")]
    [SerializeField] private PlayerController playerPrefab;
    private PlayerController activePlayer;

    [ContextMenu("Generate City")]

    private void Start()
    {
        //SetIterations(999);
        RunProceduralGeneration();
    }

    public void RunProceduralGeneration()
    {
        visualizer.ClearAllTilemaps();

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
        //visualizer.GenerateAndPaintBuildings(totalOccupiedSpace, noiseMap, limitesMinimos, limitesMaximos, resolution);

        // Integrar la Gramática de Misiones sobre las Aceras
        if (missionGen != null)
        {
            string missionStr = missionGen.GenerateMissionString();
            var objectives = missionGen.AssignObjectivesToSidewalks(missionStr, sidewalkPositions);

            // Pinta cada tarea con su Tile específica según el switch
            visualizer.PaintMissionObjectives(objectives);
        }

        // Instanciar e inicializar al Jugador
        if (playerPrefab != null && totalOccupiedSpace.Count > 0)
        {
            // Tomamos una posición válida al azar (o la primera que haya) como punto de inicio
            var enumerator = totalOccupiedSpace.GetEnumerator();
            enumerator.MoveNext();
            Vector2Int startPos = enumerator.Current;

            // Si quieres que empiece exactamente en (0,0), puedes verificar si está en la lista:
            // if (totalOccupiedSpace.Contains(Vector2Int.zero)) startPos = Vector2Int.zero;

            if (activePlayer == null)
            {
                activePlayer = Instantiate(playerPrefab);
            }

            activePlayer.Initialize(startPos, totalOccupiedSpace);
        }

        Camera.main.GetComponent<CameraFollow>().target = activePlayer.transform;
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