using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MissionGenerator : MonoBehaviour
{
    // ESTRUCTURA PARA OBJETIVOS EN EL MAPA
    [System.Serializable]
    public struct MissionObjective
    {
        public char symbol;
        public string description;
        public Vector2Int position; // Posición asignada en el Tilemap
    }

    [Header("Generation")]
    [SerializeField] private int seed = 12345;
    [Range(1, 10)][SerializeField] private int expansionSteps = 4;

    [Header("Grammar")]
    [SerializeField] private string startSymbol = "M";
    [SerializeField] private string startProduction = "STG";
    [SerializeField] private string taskSymbol = "T";
    [SerializeField] private List<string> taskProductions = new List<string>() { "CT", "ET", "RT", "KTL" };
    [SerializeField] private string terminalProduction = "C";

    public int Seed => seed;

    /// <summary>
    /// Genera la cadena de misión basada en las reglas y la semilla.
    /// </summary>
    public string GenerateMissionString()
    {
        if (!ValidateGrammar()) return string.Empty;

        string current = startProduction; // Comenzamos desde la primera derivación de M -> STG
        System.Random rand = new System.Random(seed);

        for (int i = 0; i < expansionSteps; i++)
        {
            StringBuilder mission = new StringBuilder();
            bool replaced = false;

            foreach (char symbol in current)
            {
                if (!replaced && symbol.ToString() == taskSymbol)
                {
                    int index = rand.Next(taskProductions.Count);
                    string newTask = taskProductions[index];
                    mission.Append(newTask);
                    replaced = true;
                }
                else
                {
                    mission.Append(symbol);
                }
            }
            current = mission.ToString();
            if (!replaced) break;
        }

        // Reemplazo terminal de T restantes
        StringBuilder terminalMission = new StringBuilder();
        foreach (char symbol in current)
        {
            if (symbol.ToString() == taskSymbol)
                terminalMission.Append(terminalProduction);
            else
                terminalMission.Append(symbol);
        }

        return terminalMission.ToString();
    }

    /// <summary>
    /// Convierte la cadena de misión en una lista de objetivos con posiciones sobre las aceras.
    /// </summary>
    public List<MissionObjective> AssignObjectivesToSidewalks(string missionString, HashSet<Vector2Int> sidewalkPositions)
    {
        List<MissionObjective> objectives = new List<MissionObjective>();
        List<Vector2Int> availableSpots = new List<Vector2Int>(sidewalkPositions);

        if (availableSpots.Count < missionString.Length)
        {
            Debug.LogWarning("No hay suficientes posiciones de acera para todos los objetivos de la misión.");
            return objectives;
        }

        System.Random rand = new System.Random(seed);

        foreach (char symbol in missionString)
        {
            // Selecciona una posición aleatoria de la acera y la remueve para evitar solapamientos
            int randomIndex = rand.Next(availableSpots.Count);
            Vector2Int assignedSpot = availableSpots[randomIndex];
            availableSpots.RemoveAt(randomIndex);

            objectives.Add(new MissionObjective
            {
                symbol = symbol,
                description = GetDescription(symbol),
                position = assignedSpot
            });
        }

        return objectives;
    }

    public string GetDescription(char symbol)
    {
        switch (symbol)
        {
            case 'S': return "Comienza la misión (Start).";
            case 'C': return "Busca el condimento.";
            case 'E': return "Explora por nuevos clientes.";
            case 'R': return "Recolecta el ingrediente.";
            case 'K': return "Recive una orden.";
            case 'L': return "Llevale al cliente su comida.";
            case 'G': return "Completa el objetivo (Goal).";
            default: return null;
        }
    }

    private bool ValidateGrammar()
    {
        if (string.IsNullOrEmpty(startSymbol) || string.IsNullOrEmpty(startProduction) ||
            string.IsNullOrEmpty(taskSymbol) || taskProductions == null || taskProductions.Count == 0 ||
            string.IsNullOrEmpty(terminalProduction) || terminalProduction.Contains(taskSymbol))
        {
            Debug.LogError("Configuración de gramática inválida.", this);
            return false;
        }
        return true;
    }
}