using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private HashSet<Vector2Int> validTiles;
    private Vector2Int currentGridPosition;

    // Inicializa el jugador con su posición inicial y las rutas permitidas
    public void Initialize(Vector2Int startPos, HashSet<Vector2Int> walkableTiles)
    {
        validTiles = walkableTiles;
        currentGridPosition = startPos;
        UpdateVisualPosition();
    }

    private void Update()
    {
        // Si el juego está en pausa, no procesar movimiento
        if (MenuUI.isPaused) return;

        // Movimiento por la grilla usando las flechas o WASD
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) TryMove(Vector2Int.up);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) TryMove(Vector2Int.down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) TryMove(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) TryMove(Vector2Int.right);
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int targetPos = currentGridPosition + direction;

        // Validar si la casilla destino existe en las calles o aceras
        if (validTiles != null && validTiles.Contains(targetPos))
        {
            currentGridPosition = targetPos;
            UpdateVisualPosition();
        }
    }

    private void UpdateVisualPosition()
    {
        // Actualiza el Transform del GameObject para alinearse con la grilla de Unity
        transform.position = new Vector3(currentGridPosition.x + 0.5f, 
            currentGridPosition.y + 0.5f, 0f);
    }
}