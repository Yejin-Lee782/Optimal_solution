using UnityEngine;

public class FloorMapScaler : MonoBehaviour
{
    public float realWidthMeters = 77.4f; // 도면 실제 가로 길이 (미터)
    private float aspect = 768f / 1261f;   // 이미지 비율 (세로 / 가로)

    void Start()
    {
        float realHeightMeters = realWidthMeters * aspect;

        // 1. 스케일 조정
        transform.localScale = new Vector3(realWidthMeters, 1f, realHeightMeters);

        // 2. 바닥에 눕히기
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // 3. 좌하단 기준으로 위치 조정 (중심 이동)
        transform.position = new Vector3(realWidthMeters / 2f, 0f, realHeightMeters / 2f);
    }
}
