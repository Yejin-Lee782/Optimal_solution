using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GridMap : MonoBehaviour
{
    public string csvFileName = "map_floor6.csv"; // Resources/gridmaps/map_floor6.csv
    public bool[,] grid;  // true: ��ֹ�, false: �̵� ����
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
            Debug.LogError("CSV ������ ã�� �� �����ϴ�: " + csvFileName);
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
                grid[x, height - y - 1] = cells[x] == "0"; //  0�̸� ��ֹ�
            }
        }

        Debug.Log($"CSV ��� ���� �ε� �Ϸ�: {width}x{height}");
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
