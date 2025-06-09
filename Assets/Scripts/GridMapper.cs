using UnityEngine;

public class GridMapper : MonoBehaviour
{
    // 1칸 = 0.095유닛 → 현실 거리로 환산 시 0.0565m (약 5.65cm)
    public float cellSize = 0.095f;

    // 왼쪽 아래 그리드 기준점이 (-65.5, -39.855)이므로, 이걸 offset으로 사용
    public Vector2 originOffset = new Vector2(-65.5f, -39.855f);

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - originOffset.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - originOffset.y) / cellSize); // Z축 → Y격자
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float x = gridPos.x * cellSize + originOffset.x + cellSize / 2f;
        float z = gridPos.y * cellSize + originOffset.y + cellSize / 2f;
        return new Vector3(x, 0f, z);
    }

    void Start()
    {
        Vector3 worldPos = new Vector3(-65.5f, 0f, -39.855f);
        Vector2Int grid = WorldToGrid(worldPos);
        Debug.Log($"world (-65.5, -39.855) → grid {grid}");

        Vector3 back = GridToWorld(grid);
        Debug.Log($"grid {grid} → world {back}");
    }
}
