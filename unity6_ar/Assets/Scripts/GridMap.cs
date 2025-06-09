using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GridMap : MonoBehaviour
{
    public string csvFileName = "map_floor6.csv"; // Resources/gridmaps/map_floor6.csv
    public bool[,] grid;  // true: 장애물, false: 이동 가능
    public int width;
    public int height;

    public float cellSize = 1f;
    public Vector3 originPosition = Vector3.zero;

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

        string[] lines = csvData.text.Split('\n');
        height = lines.Length;
        width = lines[0].Split(',').Length;
        grid = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            string[] cells = lines[y].Trim().Split(',');
            for (int x = 0; x < width; x++)
            {
                grid[x, height - y - 1] = cells[x] == "0"; //  0이면 장애물
            }
        }

        Debug.Log($"CSV 기반 격자 로드 완료: {width}x{height}");
    }

    public Vector3 GridToWorld(int x, int y)
    {
        return originPosition + new Vector3(x * cellSize, 0, y * cellSize);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - originPosition;
        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.z / cellSize);
        return new Vector2Int(x, y);
    }
}
