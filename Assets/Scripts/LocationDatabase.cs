using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomEntry
{
    public string roomId;
    public Vector2Int gridPos;
}

public class LocationDatabase : MonoBehaviour
{
    public List<RoomEntry> roomEntries = new List<RoomEntry>();

    private Dictionary<string, Vector2Int> roomToGrid;

    // 음수 좌표 보정을 위한 offset
    public Vector2Int offset = new Vector2Int(50, 50);  // 모든 좌표를 양수로 맞춤

    void Awake()
    {
        InitializeRooms();

        roomToGrid = new Dictionary<string, Vector2Int>();
        foreach (RoomEntry entry in roomEntries)
        {
            if (!roomToGrid.ContainsKey(entry.roomId))
                roomToGrid.Add(entry.roomId, entry.gridPos);
        }
    }

    /// <summary>
    /// 보정된 격자 위치 반환
    /// </summary>
    public Vector2Int GetGridPos(string roomId)
    {
        if (roomToGrid.ContainsKey(roomId))
        {
            Vector2Int rawPos = roomToGrid[roomId];
            return rawPos + offset; // 🔄 offset 보정
        }
        else
        {
            Debug.LogWarning($"Room ID not found: {roomId}");
            return Vector2Int.zero;
        }
    }
    public Vector2Int GetRawGridPos(string roomId)  //  방 배치용 원본 좌표
    {
        if (roomToGrid.ContainsKey(roomId))
            return roomToGrid[roomId];
        else
        {
            Debug.LogWarning($"Room ID not found: {roomId}");
            return Vector2Int.zero;
        }
    }
    void InitializeRooms()
    {
        roomEntries = new List<RoomEntry>
        {
            //실습실 및 교육 공간
            new RoomEntry { roomId = "S06-601", gridPos = new Vector2Int(41, -9) },
            new RoomEntry { roomId = "S06-602", gridPos = new Vector2Int(28, -9) },
            new RoomEntry { roomId = "S06-603", gridPos = new Vector2Int(17, -9) },
            new RoomEntry { roomId = "S06-604", gridPos = new Vector2Int(-2, -9) },
            new RoomEntry { roomId = "S06-611", gridPos = new Vector2Int(-27, 16) },
            new RoomEntry { roomId = "S06-633", gridPos = new Vector2Int(44, 0) },
            new RoomEntry { roomId = "S06-632", gridPos = new Vector2Int(38, 0) },
            new RoomEntry { roomId = "S06-631", gridPos = new Vector2Int(34, 0) },
            new RoomEntry { roomId = "S06-607", gridPos = new Vector2Int(-47, -25) },
            new RoomEntry { roomId = "S06-606", gridPos = new Vector2Int(-35, -22) },
            new RoomEntry { roomId = "S06-605", gridPos = new Vector2Int(-26, -12) },
            new RoomEntry { roomId = "S06-608", gridPos = new Vector2Int(-42, -14) },
            new RoomEntry { roomId = "S06-609", gridPos = new Vector2Int(-34, -5) },

            //교수 연구실
            new RoomEntry { roomId = "S06-614", gridPos = new Vector2Int(-16, 34) },
            new RoomEntry { roomId = "S06-615", gridPos = new Vector2Int(-16, 30) },
            new RoomEntry { roomId = "S06-616", gridPos = new Vector2Int(-16, 26) },
            new RoomEntry { roomId = "S06-617", gridPos = new Vector2Int(-16, 22) },
            new RoomEntry { roomId = "S06-618", gridPos = new Vector2Int(-16, 18) },
            new RoomEntry { roomId = "S06-619", gridPos = new Vector2Int(-15, 14) },
            new RoomEntry { roomId = "S06-620", gridPos = new Vector2Int(-16, 10) },
            new RoomEntry { roomId = "S06-621", gridPos = new Vector2Int(-16, 6) },
            new RoomEntry { roomId = "S06-626", gridPos = new Vector2Int(10, 0) },
            new RoomEntry { roomId = "S06-627", gridPos = new Vector2Int(14, 0) },
            new RoomEntry { roomId = "S06-630", gridPos = new Vector2Int(26, 0) },

            //공용 공간 및 기타
            new RoomEntry { roomId = "S06-612", gridPos = new Vector2Int(-26, 22) },
            new RoomEntry { roomId = "S06-613", gridPos = new Vector2Int(-26, 27) },
            new RoomEntry { roomId = "S06-622", gridPos = new Vector2Int(-7, 0) },
            new RoomEntry { roomId = "S06-623", gridPos = new Vector2Int(-2, 0) },
            new RoomEntry { roomId = "S06-624", gridPos = new Vector2Int(2, 0) },
            new RoomEntry { roomId = "S06-625", gridPos = new Vector2Int(6, 0) },
            new RoomEntry { roomId = "S06-628", gridPos = new Vector2Int(14, 0) },
            new RoomEntry { roomId = "S06-629", gridPos = new Vector2Int(18, 0) },

            //학생용 공간
            new RoomEntry { roomId = "S06-634", gridPos = new Vector2Int(2, 55) },
            new RoomEntry { roomId = "S06-635", gridPos = new Vector2Int(1, 55) },
            new RoomEntry { roomId = "S06-636", gridPos = new Vector2Int(0, 55) },
        };
    }
    public List<string> GetAllRoomIds()
    {
        List<string> ids = new List<string>();
        foreach (var entry in roomEntries)
        {
            ids.Add(entry.roomId);
        }
        return ids;
    }
}
