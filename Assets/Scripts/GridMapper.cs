using UnityEngine;

public class GridMapper : MonoBehaviour
{
    // 1ĭ = 0.095���� �� ���� �Ÿ��� ȯ�� �� 0.0565m (�� 5.65cm)
    public float cellSize = 0.095f;

    // ���� �Ʒ� �׸��� �������� (-65.5, -39.855)�̹Ƿ�, �̰� offset���� ���
    public Vector2 originOffset = new Vector2(-65.5f, -39.855f);

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - originOffset.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - originOffset.y) / cellSize); // Z�� �� Y����
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
        Debug.Log($"world (-65.5, -39.855) �� grid {grid}");

        Vector3 back = GridToWorld(grid);
        Debug.Log($"grid {grid} �� world {back}");
    }
}
