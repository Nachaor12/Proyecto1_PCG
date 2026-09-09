using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Walker : MonoBehaviour
{
    [SerializeField] private int Pc = 5;
    [SerializeField] private int Room_W, Room_H;
    [SerializeField] private int Direction;
    
    private int Nc;

    [SerializeField] private int Iterations;

    // Se añaden minBounds y maxBounds para definir el área rectangular permitida
    public static HashSet<Vector2Int> GenerateMap(Vector2Int startPosition, ref int pc, int iterations, Vector2Int minBounds, 
        Vector2Int maxBounds, int minSteps, int maxSteps, int minRoomScale, int maxRoomScale)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int> { startPosition };
        Vector2Int currentGridPos = startPosition;

        int currentIterations = iterations;

        while (currentIterations > 0)
        {
            int nc = Random.Range(0, 100);

            if (nc < pc)
            {
                int direction = Random.Range(1, 5);
                int steps = Random.Range(minSteps, maxSteps);

                // Mueve al agente y genera el camino, respetando los límites
                currentGridPos = MoveAndDrawPath(currentGridPos, direction, steps, floorPositions, minBounds, maxBounds);

                // Dimensiones aleatorias de la habitación
                int roomW = Random.Range(minRoomScale, maxRoomScale);
                int roomH = Random.Range(minRoomScale, maxRoomScale);

                // Genera la habitación centrada en la posición actual, respetando los límites
                GenerateRoom(currentGridPos, roomW, roomH, floorPositions, minBounds, maxBounds);

                pc = 0; // Reinicia la probabilidad tras crear habitación
            }
            else
            {
                pc += 5; // Incrementa la probabilidad si no creó habitación
            }

            currentIterations--;
        }

        return floorPositions;
    }

    private static Vector2Int MoveAndDrawPath(Vector2Int startPos, int dir, int steps, HashSet<Vector2Int> floorPositions, Vector2Int minBounds, Vector2Int maxBounds)
    {
        Vector2Int gridDirection = Vector2Int.zero;

        switch (dir)
        {
            case 1: gridDirection = Vector2Int.up; break;
            case 2: gridDirection = Vector2Int.down; break;
            case 3: gridDirection = Vector2Int.right; break;
            case 4: gridDirection = Vector2Int.left; break;
        }

        Vector2Int current = startPos;
        for (int i = 0; i < steps; i++)
        {
            Vector2Int nextPos = current + gridDirection;

            // Verifica si la siguiente posición está dentro del rectángulo permitido
            if (nextPos.x >= minBounds.x && nextPos.x <= maxBounds.x &&
                nextPos.y >= minBounds.y && nextPos.y <= maxBounds.y)
            {
                current = nextPos;
                floorPositions.Add(current);
            }
            else
            {
                // Si choca con el límite, el agente deja de caminar en esta dirección
                break;
            }
        }

        return current;
    }

    private static void GenerateRoom(Vector2Int centerPos, int width, int height, HashSet<Vector2Int> floorPositions, Vector2Int minBounds, Vector2Int maxBounds)
    {
        int startX = centerPos.x - (width / 2);
        int startY = centerPos.y - (height / 2);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int tileGridPos = new Vector2Int(startX + x, startY + y);

                // Solo añade la baldosa si está dentro de los límites rectangulares
                if (tileGridPos.x >= minBounds.x && tileGridPos.x <= maxBounds.x &&
                    tileGridPos.y >= minBounds.y && tileGridPos.y <= maxBounds.y)
                {
                    floorPositions.Add(tileGridPos);
                }
            }
        }
    }
}