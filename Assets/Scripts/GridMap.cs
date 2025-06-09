using UnityEngine;
using System.IO;

public class GridMap : MonoBehaviour
{
    public string csvFileName = "floor_grid_map_black_as_obstacle";
    public bool[,] grid;
    public int width;
    public int height;
    public Quaternion rotation = Quaternion.identity;
    public float cellSize = 0.1f;
    public Vector3 originPosition = new Vector3(-20.5f, 0f, -44.544f);

    void Awake()
    {
        LoadCSVGrid();
    }

    void LoadCSVGrid()
    {
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        if (csvData == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + csvFileName);
            return;
        }

        string[] lines = csvData.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        height = lines.Length;
        width = lines[0].Trim().Split(',').Length;
        grid = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            string[] cells = lines[y].Trim().Split(',');
            if (cells.Length != width) continue;

            for (int x = 0; x < width; x++)
            {
                grid[x, height - y - 1] = cells[x] == "0";  // 장애물 처리
            }
        }
    }

    public Vector3 GridToWorld(int x, int y)
    {
        Vector3 localPos = new Vector3(x * cellSize, 0, y * cellSize);
        return originPosition + rotation * localPos;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 local = Quaternion.Inverse(rotation) * (worldPos - originPosition);
        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.z / cellSize);
        return new Vector2Int(x, y);
    }
}
