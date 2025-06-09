using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using System.Linq;

public class NewIndoorNav2 : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private ARTrackedImageManager m_TrakedImageManager;
    [SerializeField] private GameObject trakedImagePrefab;
    [SerializeField] private LineRenderer line;

    private List<NavigationTarget> navigationTargets = new List<NavigationTarget>();
    private NavMeshSurface navMeshSurface;
    private NavMeshPath navMeshPath;

    private GameObject navigationBase;

    private void Start()
    {
        navMeshPath = new NavMeshPath();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // �ʱ� ��ġ ���� ���� (AR Ʈ��ŷ�� �������)
        SetFixedNavigationBase();
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
    }

    private void OnEnable() => m_TrakedImageManager.trackablesChanged.AddListener(OnChanged);

    private void OnDisable() => m_TrakedImageManager.trackablesChanged.RemoveListener(OnChanged);

    private void SetFixedNavigationBase()
    {
        if (navigationBase == null)
        {
            navigationBase = Instantiate(trakedImagePrefab);
        }

        navigationTargets.Clear();
        navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
        navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();

        // **�ʱ� ��ġ�� ������ (10, 0, -5)�� ����**
        Vector3 fixedPosition = new Vector3(10, 0, -5);
        Quaternion fixedRotation = Quaternion.Euler(0, 90, 0); // 90�� ȸ�� ���� (���ϴ� �������� ���� ����)

        navigationBase.transform.SetPositionAndRotation(fixedPosition, fixedRotation);
    }

    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            SetFixedNavigationBase();
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            SetFixedNavigationBase();
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // ��Ŀ�� ���ŵǾ��� �� �ʿ��� ���� �߰� ����
        }
    }
}