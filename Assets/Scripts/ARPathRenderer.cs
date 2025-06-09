using System.Collections.Generic;
using UnityEngine;

public class ARPathRenderer : MonoBehaviour
{
    public LocationDatabase locationDB;
    public Pathfinding pathfinder;
    public GridMap gridMap;
    public GameObject arrowPrefab;

    public void ShowPath(Vector2Int startGridRaw, string goalRoomId)
    {
        // offset 보정
        Vector2Int offset = locationDB.offset;
        Vector2Int startGrid = startGridRaw + offset;
        Vector2Int endGrid = locationDB.GetGridPos(goalRoomId); // 이미 offset 포함

        // 디버그 로그
        Debug.Log($"A* 호출: Start={startGrid}, End={endGrid}, GridSize=({gridMap.grid.GetLength(0)}, {gridMap.grid.GetLength(1)})");
        Debug.Log($"Start Walkable: {!gridMap.grid[startGrid.x, startGrid.y]}, Goal Walkable: {!gridMap.grid[endGrid.x, endGrid.y]}");

        //수정된 A* 호출
        List<Vector2Int> path = pathfinder.FindPath(startGrid, endGrid);

        if (path == null || path.Count < 2)
        {
            Debug.LogWarning("경로를 찾을 수 없습니다.");
            return;
        }

        // 경로 시각화
        foreach (Vector2Int gridPos in path)
        {
            Vector3 worldPos = gridMap.GridToWorld(gridPos.x, gridPos.y);
            Instantiate(arrowPrefab, worldPos + Vector3.up * 0.05f, Quaternion.identity);
        }
    }
}
