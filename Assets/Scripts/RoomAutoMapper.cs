using System.Collections.Generic;
using UnityEngine;

public class RoomAutoMapper : MonoBehaviour
{
    public GridMap gridMap;
    public Transform roomAnchorParent; // "RoomAnchors" ������Ʈ
    public LocationDatabase locationDatabase;

    [ContextMenu("�ڵ����� roomEntries ä���")]
    public void AutoFillLocationDatabase()
    {
        if (gridMap == null || roomAnchorParent == null || locationDatabase == null)
        {
            Debug.LogError("�ʼ� ������ �����Ǿ����ϴ�.");
            return;
        }

        locationDatabase.roomEntries.Clear();

        foreach (Transform child in roomAnchorParent)
        {
            string roomId = child.name;
            Vector2Int gridPos = gridMap.WorldToGrid(child.position);

            locationDatabase.roomEntries.Add(new RoomEntry { roomId = roomId, gridPos = gridPos });
            Debug.Log($"{roomId}: {gridPos}");
        }

        Debug.Log("LocationDatabase.roomEntries �ڵ� �ϼ� �Ϸ�!");
    }
}
