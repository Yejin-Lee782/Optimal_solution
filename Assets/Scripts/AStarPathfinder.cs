using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    private static readonly Vector2Int[] Directions = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1)
    };

    public static List<Vector2Int> FindPath(bool[,] grid, Vector2Int start, Vector2Int goal)
    {
        int width = grid.GetLength(0), height = grid.GetLength(1);

        Debug.Log($"A* Ω√¿€: Start={start}, Goal={goal}, GridSize=({width},{height})");
        Debug.Log($"Start Walkable: {!grid[start.x, start.y]}, Goal Walkable: {!grid[goal.x, goal.y]}");


        var frontier = new PriorityQueue<Vector2Int>();
        frontier.Enqueue(start, 0);

        var cameFrom = new Dictionary<Vector2Int, Vector2Int?>();
        var costSoFar = new Dictionary<Vector2Int, int>();

        cameFrom[start] = null;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            if (current == goal) break;

            foreach (var dir in Directions)
            {
                Vector2Int next = current + dir;
                if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                {
                    Debug.LogWarning($"Out of bounds: {next}");
                    continue;
                }

                if (grid[next.x, next.y])
                {
                    Debug.LogWarning($"Blocked (wall): {next}");
                    continue;
                }

                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null;

        var path = new List<Vector2Int>();
        Vector2Int? currentNode = goal;
        while (currentNode != null)
        {
            path.Add(currentNode.Value);
            currentNode = cameFrom[currentNode.Value];
        }
        path.Reverse();
        return path;
    }

    private static int Heuristic(Vector2Int a, Vector2Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
}
