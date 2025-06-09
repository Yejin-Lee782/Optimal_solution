using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using UnityEngine.UI;
using System.Linq;
using Unity.XR.CoreUtils;

public class NewIndoorNav : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin; // XR 오리진: 사용자의 기준점
    [SerializeField] private GameObject trakedImagePrefab;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Camera arCamera;

    private List<NavigationTarget> navigationTargets = new List<NavigationTarget>();
    private NavMeshSurface navMeshSurface;
    private NavMeshPath navMeshPath;
    private GameObject navigationBase;
    private IBarcodeReader barcodeReader;
    private Texture2D cameraTexture;

    private int frameCounter = 0;

    private void Start()
    {
        navMeshPath = new NavMeshPath();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        barcodeReader = new BarcodeReader { AutoRotate = true };

        cameraTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
    }

    private void Update()
    {
        if (navMeshPath != null && navigationTargets.Count > 0 && navMeshSurface != null)
        {
            // XR Origin에서 첫 번째 NavigationTarget까지의 경로 계산
            NavMesh.CalculatePath(xrOrigin.transform.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                Debug.Log("경로 코너 개수: " + navMeshPath.corners.Length);
                foreach (Vector3 corner in navMeshPath.corners)
                {
                    Debug.Log("Corner: " + corner);
                }
                line.positionCount = navMeshPath.corners.Length;
                line.SetPositions(navMeshPath.corners);
            }
            else
            {
                line.positionCount = 0;
            }
        }

        // 일정 주기로 QR 코드 스캔 실행
        if (frameCounter % 10 == 0)
        {
            StartCoroutine(ScanQRCodeCoroutine());
        }

        frameCounter++;
    }

    private IEnumerator ScanQRCodeCoroutine()
    {
        // 렌더링이 끝날 때까지 대기
        yield return new WaitForEndOfFrame();

        if (arCamera == null)
            yield break;

        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = arCamera.targetTexture;

        cameraTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        cameraTexture.Apply();

        RenderTexture.active = activeRenderTexture;

        try
        {
            Color32[] pixels = cameraTexture.GetPixels32();
            int width = cameraTexture.width;
            int height = cameraTexture.height;

            Result result = barcodeReader.Decode(pixels, width, height);

            if (result != null)
            {
                Debug.Log($"QR 코드 인식: {result.Text}");
                StartCoroutine(UpdateNavigationBasePosition(result.Text));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QR 코드 스캔 오류: {ex.Message}");
        }

        // 인식 주기 조정 (예: 0.2초)
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator UpdateNavigationBasePosition(string data)
    {
        // QR 코드 데이터 형식: x, y, z, x축 회전, y축 회전, z축 회전
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
            Vector3 init = Vector3.zero;

            if (navigationBase != null)
            {
                Destroy(navigationBase);
            }
            navigationBase = Instantiate(trakedImagePrefab);
            navigationBase.transform.SetPositionAndRotation(init, Quaternion.identity);

            yield return null; // navigationBase가 완전히 로드될 때까지 한 프레임 대기

            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();

            // InitialPose 컴포넌트를 가진 오브젝트 찾기
            InitialPose initialPose = navigationBase.GetComponentInChildren<InitialPose>();
            if (initialPose != null)
            {
                // InitialPose의 위치를 사용하되, 회전은 QR 코드의 회전 값을 적용합니다.
                MoveXROrigin(initialPose.transform.position, qrRotation);
                Debug.Log($"InitialPose 기준으로 XROrigin 이동: {initialPose.transform.position}, 회전: {qrRotation.eulerAngles}");
            }
            else
            {
                MoveXROrigin(qrPosition, qrRotation);
                Debug.Log($"InitialPose 없음. QR 코드 기준으로 XROrigin 이동: {qrPosition}, 회전: {qrRotation.eulerAngles}");
            }

            Debug.Log($"QR 코드 기반으로 NavigationBase 위치 재설정: {qrPosition}");

            if (navigationTargets.Count > 0)
            {
                NavMesh.CalculatePath(xrOrigin.transform.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

                if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    Debug.Log("네비게이션 경로 업데이트 완료");
                }
                else
                {
                    Debug.LogError("네비게이션 경로 설정 실패");
                }
            }
            else
            {
                Debug.LogError("Navigation Target이 설정되지 않음.");
            }
        }
        else
        {
            Debug.LogError("QR 코드 데이터가 올바른 형식이 아닙니다. 예: 10,0,-6,0,180,0");
        }
        yield return null;
    }

    private void MoveXROrigin(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (xrOrigin != null)
        {
            // 카메라의 현재 위치와 비교하여 오프셋을 계산하고, XR Origin의 위치와 회전을 업데이트합니다.
            Vector3 offset = targetPosition - xrOrigin.Camera.transform.position;
            xrOrigin.transform.position += offset;
            xrOrigin.transform.rotation = targetRotation;
            Debug.Log($"XROrigin 이동 완료: {xrOrigin.transform.position}, 회전: {xrOrigin.transform.rotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("XROrigin이 설정되지 않음.");
        }
    }
}
