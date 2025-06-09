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
        // offset ����
        Vector2Int offset = locationDB.offset;
        Vector2Int startGrid = startGridRaw + offset;
        Vector2Int endGrid = locationDB.GetGridPos(goalRoomId); // �̹� offset ����

        // ����� �α�
        Debug.Log($"A* ȣ��: Start={startGrid}, End={endGrid}, GridSize=({gridMap.grid.GetLength(0)}, {gridMap.grid.GetLength(1)})");
        Debug.Log($"Start Walkable: {!gridMap.grid[startGrid.x, startGrid.y]}, Goal Walkable: {!gridMap.grid[endGrid.x, endGrid.y]}");

        //������ A* ȣ��
        List<Vector2Int> path = pathfinder.FindPath(startGrid, endGrid);

        if (path == null || path.Count < 2)
        {
            Debug.LogWarning("��θ� ã�� �� �����ϴ�.");
            return;
        }

        // ��� �ð�ȭ
        foreach (Vector2Int gridPos in path)
        {
            Vector3 worldPos = gridMap.GridToWorld(gridPos.x, gridPos.y);
            Instantiate(arrowPrefab, worldPos + Vector3.up * 0.05f, Quaternion.identity);
        }
    }
}
