using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MissionGenerator : MonoBehaviour
{
    public enum MissionContext
    {
        City,
        Restaurant
    }

    // ESTRUCTURA PARA OBJETIVOS EN EL MAPA
    [System.Serializable]
    public struct MissionObjective
    {
        public char symbol;
        public string description;
        public Vector2Int position;
    }

    [Header("Generation")]
    [SerializeField] public int seed = 12345;
    [Range(1, 10)][SerializeField] private int expansionSteps = 4;

    [Header("City Grammar")]
    [SerializeField] private string cityStartProduction = "STG";
    [SerializeField] private string cityTaskSymbol = "T";
    [SerializeField] private List<string> cityTaskProductions = new List<string>() { "CT", "ET", "RT", "KTL" };
    [SerializeField] private string cityTerminalProduction = "C";

    [Header("Restaurant Grammar")]
    [SerializeField] private string restStartProduction = "OMT";
    [SerializeField] private string restTaskSymbol = "T";
    [SerializeField] private List<string> restTaskProductions = new List<string>() { "MMT", "MHT", "DT" };
    [SerializeField] private string restTerminalProduction = "M";

    public int Seed => seed;

    /// <summary>
    /// Genera la cadena de misión o utilería basada en el contexto y la semilla.
    /// </summary>
    public string GenerateMissionString(MissionContext context)
    {
        // Seleccionar la gramática correcta según el contexto
        string startProduction = context == MissionContext.City ? cityStartProduction : restStartProduction;
        string taskSymbol = context == MissionContext.City ? cityTaskSymbol : restTaskSymbol;
        List<string> taskProductions = context == MissionContext.City ? cityTaskProductions : restTaskProductions;
        string terminalProduction = context == MissionContext.City ? cityTerminalProduction : restTerminalProduction;

        if (!ValidateGrammar(startProduction, taskSymbol, taskProductions, terminalProduction)) return string.Empty;

        string current = startProduction;
        System.Random rand = new System.Random(seed);

        // Proceso de expansión (Derivación)
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

        // Reemplazo terminal de los símbolos de tarea (T) restantes
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
    /// Convierte la cadena en una lista de objetivos con posiciones sobre las casillas disponibles.
    /// </summary>
    public List<MissionObjective> AssignObjectivesToPositions(string generatedString, HashSet<Vector2Int> validPositions)
    {
        List<MissionObjective> objectives = new List<MissionObjective>();
        List<Vector2Int> availableSpots = new List<Vector2Int>(validPositions);

        if (availableSpots.Count < generatedString.Length)
        {
            Debug.LogWarning("No hay suficientes posiciones para todos los objetivos/muebles.");
            return objectives;
        }

        System.Random rand = new System.Random(seed);

        foreach (char symbol in generatedString)
        {
            int randomIndex = rand.Next(availableSpots.Count);
            Vector2Int assignedSpot = availableSpots[randomIndex];
            availableSpots.RemoveAt(randomIndex); // Evitar solapamientos

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
            // Descripciones de Ciudad
            case 'S': return "Comienza la misión (Start)."; 
            case 'C': return "Busca el condimento."; 
            case 'E': return "Explora por nuevos clientes."; 
            case 'R': return "Recolecta el ingrediente."; 
            case 'K': return "Recibe una orden."; 
            case 'L': return "Llévale al cliente su comida."; 
            case 'G': return "Completa el objetivo (Goal)."; 
            
            // Descripciones de Restaurante
            case 'O': return "Área de Cocina/Horno.";
            case 'M': return "Mesa para clientes.";
            case 'H': return "Silla.";
            case 'D': return "Decoración interior (Planta, cuadro).";

            default: return "Objeto desconocido.";
        }
    }

    private bool ValidateGrammar(string startProd, string taskSym, List<string> taskProds, string terminalProd)
    {
        if (string.IsNullOrEmpty(startProd) || string.IsNullOrEmpty(taskSym) ||
            taskProds == null || taskProds.Count == 0 ||
            string.IsNullOrEmpty(terminalProd) || terminalProd.Contains(taskSym))
        {
            Debug.LogError("Configuración de gramática inválida para el contexto actual.", this);
            return false;
        }
        return true;
    }
}