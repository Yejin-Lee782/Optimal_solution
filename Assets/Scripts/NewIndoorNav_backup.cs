using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using ZXing;
using UnityEngine.UI;
using System.Linq;

public class NewIndoorNavBackup : MonoBehaviour
{
    [SerializeField] private Transform player;
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
            NavMesh.CalculatePath(player.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                line.positionCount = navMeshPath.corners.Length;
                line.SetPositions(navMeshPath.corners);
            }
            else
            {
                line.positionCount = 0;
            }
        }

        if (frameCounter % 15 == 0)
        {
            ScanQRCode();
        }

        frameCounter++;
    }

    private void ScanQRCode()
    {
        if (arCamera == null)
            return;

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
                UpdateNavigationBasePosition(result.Text);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QR �ڵ� ��ĵ ����: {ex.Message}");
        }
    }

    private void UpdateNavigationBasePosition(string data)
    {
        string[] positionData = data.Split(',');

        if (positionData.Length == 3 &&
            float.TryParse(positionData[0], out float x) &&
            float.TryParse(positionData[1], out float y) &&
            float.TryParse(positionData[2], out float z))
        {
            Vector3 qrPosition = new Vector3(x, y, z);
            Vector3 init = Vector3.zero;

            if (navigationBase != null)
            {
                Destroy(navigationBase);
            }
            navigationBase = Instantiate(trakedImagePrefab);
            navigationBase.transform.SetPositionAndRotation(init, Quaternion.identity);

            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();

            // **InitialPose ������Ʈ�� ���� ������Ʈ ã��**
            InitialPose initialPose = navigationBase.GetComponentInChildren<InitialPose>();
            if (initialPose != null)
            {
                player.transform.SetPositionAndRotation(initialPose.transform.position, Quaternion.identity);
                Debug.Log($"InitialPose �������� Player ��ġ ����: {player.position}");
            }
            else
            {
                player.transform.SetPositionAndRotation(qrPosition, Quaternion.identity);
                Debug.Log($"InitialPose ����. QR �ڵ� ��ġ �������� Player ��ġ ����: {qrPosition}");
            }

            Debug.Log($"QR �ڵ� ������� NavigationBase ��ġ �缳��: {qrPosition}");

            if (navigationTargets.Count > 0)
            {
                NavMesh.CalculatePath(player.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

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
            Debug.LogError("QR �ڵ� �����Ͱ� �ùٸ� ��ǥ ������ �ƴմϴ�. ��: 10,0,-6");
        }
    }
}