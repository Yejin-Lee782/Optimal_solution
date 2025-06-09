using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform gridParent;  // �ν����Ϳ��� GridMapRoot ����
    public Vector2Int offset = new Vector2Int(50, 50);  // ��ǥ ������
    public bool[,] grid;  // A*�� ����� ���� �迭

    void Start()
    {
        InitializeGridFromScene();  // grid ����
    }

    public void InitializeGridFromScene()
    {
        int width = 2000;
        int height = 1000;
        grid = new bool[width, height];

        foreach (Transform child in gridParent)
        {
            GameObject cell = child.gameObject;

            MeshRenderer renderer = cell.GetComponent<MeshRenderer>();
            if (renderer == null) continue;

            bool isWall = renderer.material.color == Color.red;

            Vector3 worldPos = cell.transform.position;
            Vector2Int rawGrid = WorldToGrid(worldPos);
            Vector2Int gridPos = rawGrid + offset;

            //  ����� �α� (�� ��ǥ�� ū�� Ȯ��)
            Debug.Log($"world {worldPos} �� rawGrid {rawGrid} �� gridPos {gridPos}");


            // ������ �ε��� üũ
            if (gridPos.x >= 0 && gridPos.x < grid.GetLength(0) &&
                gridPos.y >= 0 && gridPos.y < grid.GetLength(1))
            {
                grid[gridPos.x, gridPos.y] = isWall;
            }
            else
            {
                Debug.LogWarning($"���õ� ��: gridPos={gridPos} �� �迭 ������ ���");
            }

            grid[gridPos.x, gridPos.y] = isWall;
        }

        Debug.Log("grid �ʱ�ȭ �Ϸ� (�±� ����)");
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        if (grid == null)
        {
            Debug.LogError("A* grid �迭�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return null;
        }

        return AStarPathfinder.FindPath(grid, start, goal);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float cellSize = 0.095f;
        Vector2 originOffset = new Vector2(-65.5f, -39.855f);  // ���� ���� ���ϴ�

        int x = Mathf.FloorToInt((worldPos.x - originOffset.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - originOffset.y) / cellSize);

        return new Vector2Int(x, y);
    }
}
