using UnityEngine;

public class GridMapVisualizer : MonoBehaviour
{
    public GridMap gridMap; // ���� ���� ��������
    public GameObject cellPrefab; // ť�� �Ǵ� ���� �ڽ� ������
    public Transform cellParent; // ������ ���� ���� �θ� ������Ʈ
    public Material obstacleMaterial;
    public Material openMaterial;

    void Start()
    {
        for (int x = 0; x < gridMap.width; x++)
        {
            for (int y = 0; y < gridMap.height; y++)
            {
                Vector3 pos = gridMap.GridToWorld(x, y) + new Vector3(0, 0.01f, 0); // �ణ �� �ְ�
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, cellParent);
                cell.transform.localScale = Vector3.one * gridMap.cellSize * 0.95f; // ��ġ�� �ʰ� ��¦ �۰�

                // ��ֹ� ���ο� ���� ���� ����
                MeshRenderer mr = cell.GetComponent<MeshRenderer>();
                if (gridMap.grid[x, y])
                    mr.material = obstacleMaterial;
                else
                    mr.material = openMaterial;
            }
        }
    }
}
