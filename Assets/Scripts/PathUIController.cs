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

    //  QR���� �޾ƿ��� ���� ���� ��ǥ
    public Vector2Int currentStartGrid { get; private set; }

    //  QR �ν� �� NewIndoorNav.cs���� �̰� ȣ����
    public void SetStartGrid(Vector2Int start)
    {
        currentStartGrid = start;
    }

    void Start()
    {
        // �ӽ� ���� ��ǥ ���� (��: S06-601 ��ġ)
        if (currentStartGrid == Vector2Int.zero)
            currentStartGrid = locationDB.GetGridPos("S06-601");  // �Ǵ� ������ �� ID

        // Room ID ��� �ҷ�����
        roomIds = locationDB.GetAllRoomIds();

        goalDropdown.ClearOptions();
        goalDropdown.AddOptions(roomIds);

        pathButton.onClick.AddListener(OnPathButtonClicked);
    }

    void OnPathButtonClicked()
    {
        string goal = roomIds[goalDropdown.value];

        // currentStartGrid�� ���
        pathRenderer.ShowPath(currentStartGrid, goal);
    }
}
