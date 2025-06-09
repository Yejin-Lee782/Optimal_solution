using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform gridParent;  // 인스펙터에서 GridMapRoot 지정
    public Vector2Int offset = new Vector2Int(50, 50);  // 좌표 보정용
    public bool[,] grid;  // A*가 사용할 격자 배열

    void Start()
    {
        InitializeGridFromScene();  // grid 생성
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

            //  디버깅 로그 (왜 좌표가 큰지 확인)
            Debug.Log($"world {worldPos} → rawGrid {rawGrid} → gridPos {gridPos}");


            // 안전한 인덱스 체크
            if (gridPos.x >= 0 && gridPos.x < grid.GetLength(0) &&
                gridPos.y >= 0 && gridPos.y < grid.GetLength(1))
            {
                grid[gridPos.x, gridPos.y] = isWall;
            }
            else
            {
                Debug.LogWarning($"무시된 셀: gridPos={gridPos} → 배열 범위를 벗어남");
            }

            grid[gridPos.x, gridPos.y] = isWall;
        }

        Debug.Log("grid 초기화 완료 (태그 없이)");
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        if (grid == null)
        {
            Debug.LogError("A* grid 배열이 초기화되지 않았습니다.");
            return null;
        }

        return AStarPathfinder.FindPath(grid, start, goal);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float cellSize = 0.095f;
        Vector2 originOffset = new Vector2(-65.5f, -39.855f);  // 도면 기준 좌하단

        int x = Mathf.FloorToInt((worldPos.x - originOffset.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - originOffset.y) / cellSize);

        return new Vector2Int(x, y);
    }
}
