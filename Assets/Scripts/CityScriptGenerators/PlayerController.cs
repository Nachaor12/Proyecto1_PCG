using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private HashSet<Vector2Int> validTiles;
    private Vector2Int currentGridPosition;

    ///Variables de movimiento
    [SerializeField] private float velocity = 5;
    private float MovePlayerAxisH = 1;
    private float MovePlayerAxisV = 0;

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

        


        SecondMovementHorizontal(MovePlayerAxisH);
        SecondMovementVertical(MovePlayerAxisV);
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

    private void SecondMovementHorizontal(float movementH)
    {
        //Variabales de detección de dirección
        movementH = Input.GetAxisRaw("Horizontal");

        //Aplica movimiento 
        Vector2 Movement = new Vector2(movementH * velocity * Time.deltaTime, 0);
        transform.Translate(Movement);
    }

    private void SecondMovementVertical(float movementV)
    {
        //Variabales de detección de dirección
        movementV = Input.GetAxisRaw("Vertical");

        //Aplica movimiento 
        Vector2 Movement = new Vector2(0, movementV * velocity * Time.deltaTime);
        transform.Translate(Movement);
    }
}