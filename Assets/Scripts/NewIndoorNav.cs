using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using UnityEngine.UI;
using System.Linq;
using Unity.XR.CoreUtils;

public class NewIndoorNav : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private GameObject trackedImagePrefab;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Camera arCamera;

    [SerializeField] private GridMap gridMap;                     // 격자 맵 참조
    [SerializeField] private LocationDatabase locationDatabase;   // 위치 DB 참조
    [SerializeField] private string goalRoomId;                   // 목적지 ID
    [SerializeField] private PathUIController pathUIController; // 연결 필요

    private GameObject navigationBase;
    private IBarcodeReader barcodeReader;
    private Texture2D cameraTexture;
    private int frameCounter = 0;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        barcodeReader = new BarcodeReader { AutoRotate = true };
        cameraTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
    }

    private void Update()
    {
        // 매 10 프레임마다 QR 스캔 시도
        if (frameCounter % 10 == 0)
        {
            StartCoroutine(ScanQRCodeCoroutine());
        }
        frameCounter++;
    }
    private IEnumerator DelayedQRCodeScan()
    {
        yield return new WaitForSeconds(0.1f);  // 내부 업데이트가 끝난 뒤 실행
        yield return StartCoroutine(ScanQRCodeCoroutine());
    }

    private IEnumerator ScanQRCodeCoroutine()
    {
        yield return new WaitForEndOfFrame();

        if (arCamera == null)
            yield break;

        RenderTexture.active = arCamera.targetTexture;
        cameraTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        cameraTexture.Apply();
        RenderTexture.active = null;

        try
        {
            Color32[] pixels = cameraTexture.GetPixels32();
            int width = cameraTexture.width;
            int height = cameraTexture.height;

            Result result = barcodeReader.Decode(pixels, width, height);
            if (result != null)
            {
                Debug.Log($"QR 인식 성공: {result.Text}");
                StartCoroutine(UpdateNavigationBasePosition(result.Text));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QR 처리 오류: {ex.Message}");
        }

        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator UpdateNavigationBasePosition(string data)
    {
        // 예시 포맷: "10,0,-6,0,180,0"
        string[] dataValues = data.Split(',');

        if (dataValues.Length == 6 &&
            float.TryParse(dataValues[0], out float x) &&
            float.TryParse(dataValues[1], out float y) &&
            float.TryParse(dataValues[2], out float z) &&
            float.TryParse(dataValues[3], out float rotX) &&
            float.TryParse(dataValues[4], out float rotY) &&
            float.TryParse(dataValues[5], out float rotZ))
        {
            Vector3 qrPosition = new Vector3(x, y, z);
            Quaternion qrRotation = Quaternion.Euler(rotX, rotY, rotZ);

            if (navigationBase != null)
                Destroy(navigationBase);

            navigationBase = Instantiate(trackedImagePrefab);
            navigationBase.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            yield return null;

            // XR 위치 초기화
            MoveXROrigin(qrPosition, qrRotation);
            Debug.Log($"XR Origin 초기 위치 설정: {qrPosition}");

     
            // 격자 맵 좌표 계산
            Vector2Int startGrid = gridMap.WorldToGrid(qrPosition);
            Vector2Int goalGrid = locationDatabase.GetGridPos(goalRoomId);
            pathUIController.SetStartGrid(startGrid);

            Debug.Log($"QR World Position: {qrPosition}");
            Debug.Log($"Start Grid: {startGrid}");
            Debug.Log($"Goal Grid for {goalRoomId}: {goalGrid}");
            // A* 경로 탐색
            List<Vector2Int> path = AStarPathfinder.FindPath(gridMap.grid, startGrid, goalGrid);

            if (path != null && path.Count > 1)
            {
                Debug.Log($"경로 탐색 성공: {path.Count} 지점");

                Vector3[] worldPath = path.Select(p => gridMap.GridToWorld(p.x, p.y)).ToArray();
                line.positionCount = worldPath.Length;
                line.SetPositions(worldPath);
            }
            else
            {
                Debug.LogWarning("유효한 경로를 찾지 못했습니다.");
                line.positionCount = 0;
            }
        }
        else
        {
            Debug.LogError("QR 코드 데이터 형식 오류. 예: 10,0,-6,0,180,0");
        }

        yield return null;
    }

    private void MoveXROrigin(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (xrOrigin != null)
        {
            Vector3 offset = targetPosition - xrOrigin.Camera.transform.position;
            xrOrigin.transform.position += offset;
            xrOrigin.transform.rotation = targetRotation;

            Debug.Log($"XR Origin 이동 완료: {xrOrigin.transform.position}");
        }
        else
        {
            Debug.LogError("XR Origin이 설정되지 않았습니다.");
        }
    }
}
