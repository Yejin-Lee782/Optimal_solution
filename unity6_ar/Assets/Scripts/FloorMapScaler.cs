using UnityEngine;

public class FloorMapScaler : MonoBehaviour
{
    public float realWidthMeters = 77.4f; // ���� ���� ���� ���� (����)
    private float aspect = 768f / 1261f;   // �̹��� ���� (���� / ����)

    void Start()
    {
        float realHeightMeters = realWidthMeters * aspect;

        // 1. ������ ����
        transform.localScale = new Vector3(realWidthMeters, 1f, realHeightMeters);

        // 2. �ٴڿ� ������
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // 3. ���ϴ� �������� ��ġ ���� (�߽� �̵�)
        transform.position = new Vector3(realWidthMeters / 2f, 0f, realHeightMeters / 2f);
    }
}
