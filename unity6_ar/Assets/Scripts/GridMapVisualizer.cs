using UnityEngine;

public class GridMapVisualizer : MonoBehaviour
{
    public GridMap gridMap; // 격자 정보 가져오기
    public GameObject cellPrefab; // 큐브 또는 투명 박스 프리팹
    public Transform cellParent; // 생성된 셀을 담을 부모 오브젝트
    public Material obstacleMaterial;
    public Material openMaterial;

    void Start()
    {
        for (int x = 0; x < gridMap.width; x++)
        {
            for (int y = 0; y < gridMap.height; y++)
            {
                Vector3 pos = gridMap.GridToWorld(x, y) + new Vector3(0, 0.01f, 0); // 약간 떠 있게
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, cellParent);
                cell.transform.localScale = Vector3.one * gridMap.cellSize * 0.95f; // 겹치지 않게 살짝 작게

                // 장애물 여부에 따라 색상 지정
                MeshRenderer mr = cell.GetComponent<MeshRenderer>();
                if (gridMap.grid[x, y])
                    mr.material = obstacleMaterial;
                else
                    mr.material = openMaterial;
            }
        }
    }
}
