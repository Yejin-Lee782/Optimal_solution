using System.Collections.Generic;
using UnityEngine;

public class RoomAutoMapper : MonoBehaviour
{
    public GridMap gridMap;
    public Transform roomAnchorParent; // "RoomAnchors" 오브젝트
    public LocationDatabase locationDatabase;

    [ContextMenu("자동으로 roomEntries 채우기")]
    public void AutoFillLocationDatabase()
    {
        if (gridMap == null || roomAnchorParent == null || locationDatabase == null)
        {
            Debug.LogError("필수 참조가 누락되었습니다.");
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

        Debug.Log("LocationDatabase.roomEntries 자동 완성 완료!");
    }
}
