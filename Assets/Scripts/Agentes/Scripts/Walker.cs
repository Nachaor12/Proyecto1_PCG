using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{
    [SerializeField] private int Pc = 5;
    [SerializeField] private int Room_W, Room_H;
    [SerializeField] private int Direction;
    private int Nc;

    [SerializeField] private int Iterations;
    [SerializeField] public GameObject prefab;

    [SerializeField] private float prefabSize = 0.5f;

    
    private Vector2Int currentGridPos = Vector2Int.zero;
    private HashSet<Vector2Int> spawnedGridPositions = new HashSet<Vector2Int>();

    void Start()
    {
        currentGridPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / prefabSize),
            Mathf.RoundToInt(transform.position.y / prefabSize)
        );

        GenerateMap();
    }

    void GenerateMap()
    {
        GeneratePrefabAtGrid(currentGridPos);

        while (Iterations > 0)
        {
            Nc = Random.Range(0, 100);

            if (Nc < Pc)
            {
                Direction = Random.Range(1, 5);
                int steps = Random.Range(3, 8);

                MoveAndDrawPath(Direction, steps);

                Room_W = Random.Range(3, 8);
                Room_H = Random.Range(3, 8);
                GenerateRoom(Room_W, Room_H);

                Pc = 0;
            }
            else
            {
                Pc += 5;
            }

            Iterations--;
        }
    }

    void MoveAndDrawPath(int dir, int steps)
    {
        Vector2Int gridDirection = Vector2Int.zero;

        switch (dir)
        {
            case 1: gridDirection = Vector2Int.up; break;
            case 2: gridDirection = Vector2Int.down; break;
            case 3: gridDirection = Vector2Int.right; break;
            case 4: gridDirection = Vector2Int.left; break;
        }

        for (int i = 0; i < steps; i++)
        {
            currentGridPos += gridDirection;
            GeneratePrefabAtGrid(currentGridPos);
        }

        transform.position = new Vector3(currentGridPos.x * prefabSize, currentGridPos.y * prefabSize, transform.position.z);
    }

    void GenerateRoom(int width, int height)
    {
        int startX = currentGridPos.x - (width / 2);
        int startY = currentGridPos.y - (height / 2);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int tileGridPos = new Vector2Int(startX + x, startY + y);
                GeneratePrefabAtGrid(tileGridPos);
            }
        }
    }

    private void GeneratePrefabAtGrid(Vector2Int gridPos)
    {
        if (!spawnedGridPositions.Contains(gridPos))
        {
            Vector3 worldPos = new Vector3(gridPos.x * prefabSize, gridPos.y * prefabSize, 0f);

            Instantiate(prefab, worldPos, Quaternion.identity);
            spawnedGridPositions.Add(gridPos);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(prefabSize, prefabSize, prefabSize));
    }
}
