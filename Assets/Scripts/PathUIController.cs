using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PathUIController : MonoBehaviour
{
    public TMP_Dropdown goalDropdown;
    public Button pathButton;

    public ARPathRenderer pathRenderer;
    public LocationDatabase locationDB;

    private List<string> roomIds;

    //  QR에서 받아오는 시작 격자 좌표
    public Vector2Int currentStartGrid { get; private set; }

    //  QR 인식 후 NewIndoorNav.cs에서 이걸 호출함
    public void SetStartGrid(Vector2Int start)
    {
        currentStartGrid = start;
    }

    void Start()
    {
        // 임시 시작 좌표 지정 (예: S06-601 위치)
        if (currentStartGrid == Vector2Int.zero)
            currentStartGrid = locationDB.GetGridPos("S06-601");  // 또는 적절한 방 ID

        // Room ID 목록 불러오기
        roomIds = locationDB.GetAllRoomIds();

        goalDropdown.ClearOptions();
        goalDropdown.AddOptions(roomIds);

        pathButton.onClick.AddListener(OnPathButtonClicked);
    }

    void OnPathButtonClicked()
    {
        string goal = roomIds[goalDropdown.value];

        // currentStartGrid를 사용
        pathRenderer.ShowPath(currentStartGrid, goal);
    }
}
