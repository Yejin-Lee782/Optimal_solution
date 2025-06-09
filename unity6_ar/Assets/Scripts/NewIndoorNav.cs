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
    [SerializeField] private XROrigin xrOrigin; // XR ������: ������� ������
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
            // XR Origin���� ù ��° NavigationTarget������ ��� ���
            NavMesh.CalculatePath(xrOrigin.transform.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                Debug.Log("��� �ڳ� ����: " + navMeshPath.corners.Length);
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

        // ���� �ֱ�� QR �ڵ� ��ĵ ����
        if (frameCounter % 10 == 0)
        {
            StartCoroutine(ScanQRCodeCoroutine());
        }

        frameCounter++;
    }

    private IEnumerator ScanQRCodeCoroutine()
    {
        // �������� ���� ������ ���
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
                Debug.Log($"QR �ڵ� �ν�: {result.Text}");
                StartCoroutine(UpdateNavigationBasePosition(result.Text));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QR �ڵ� ��ĵ ����: {ex.Message}");
        }

        // �ν� �ֱ� ���� (��: 0.2��)
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator UpdateNavigationBasePosition(string data)
    {
        // QR �ڵ� ������ ����: x, y, z, x�� ȸ��, y�� ȸ��, z�� ȸ��
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

            yield return null; // navigationBase�� ������ �ε�� ������ �� ������ ���

            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();

            // InitialPose ������Ʈ�� ���� ������Ʈ ã��
            InitialPose initialPose = navigationBase.GetComponentInChildren<InitialPose>();
            if (initialPose != null)
            {
                // InitialPose�� ��ġ�� ����ϵ�, ȸ���� QR �ڵ��� ȸ�� ���� �����մϴ�.
                MoveXROrigin(initialPose.transform.position, qrRotation);
                Debug.Log($"InitialPose �������� XROrigin �̵�: {initialPose.transform.position}, ȸ��: {qrRotation.eulerAngles}");
            }
            else
            {
                MoveXROrigin(qrPosition, qrRotation);
                Debug.Log($"InitialPose ����. QR �ڵ� �������� XROrigin �̵�: {qrPosition}, ȸ��: {qrRotation.eulerAngles}");
            }

            Debug.Log($"QR �ڵ� ������� NavigationBase ��ġ �缳��: {qrPosition}");

            if (navigationTargets.Count > 0)
            {
                NavMesh.CalculatePath(xrOrigin.transform.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

                if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    Debug.Log("�׺���̼� ��� ������Ʈ �Ϸ�");
                }
                else
                {
                    Debug.LogError("�׺���̼� ��� ���� ����");
                }
            }
            else
            {
                Debug.LogError("Navigation Target�� �������� ����.");
            }
        }
        else
        {
            Debug.LogError("QR �ڵ� �����Ͱ� �ùٸ� ������ �ƴմϴ�. ��: 10,0,-6,0,180,0");
        }
        yield return null;
    }

    private void MoveXROrigin(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (xrOrigin != null)
        {
            // ī�޶��� ���� ��ġ�� ���Ͽ� �������� ����ϰ�, XR Origin�� ��ġ�� ȸ���� ������Ʈ�մϴ�.
            Vector3 offset = targetPosition - xrOrigin.Camera.transform.position;
            xrOrigin.transform.position += offset;
            xrOrigin.transform.rotation = targetRotation;
            Debug.Log($"XROrigin �̵� �Ϸ�: {xrOrigin.transform.position}, ȸ��: {xrOrigin.transform.rotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("XROrigin�� �������� ����.");
        }
    }
}
