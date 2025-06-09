using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using System.Linq;

public class NewIndoorNavbackup: MonoBehaviour
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
    }

    private void Update()
    {
        if(navMeshPath != null&&navigationTargets.Count >0 && navMeshSurface !=null)
        {
            //navMeshSurface.BuildNavMesh();
            NavMesh.CalculatePath(player.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

            if(navMeshPath.status == NavMeshPathStatus.PathComplete)
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

    /*
        private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
        {
            foreach(var newImage in eventArgs.added)
            {
                navigationBase = GameObject.Instantiate(trakedImagePrefab);
                navigationTargets.Clear();
                navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
                navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();
            } 
            foreach(var updatedImage in eventArgs.updated)
            {
                navigationBase.transform.SetPositionAndRotation(updatedImage.pose.position, Quaternion.Euler(0, updatedImage.pose.rotation.eulerAngles.y, 0));
            }

            foreach(var removedImage in eventArgs.removed)
            {

            }
        }

        */

    /*
    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            navigationBase = GameObject.Instantiate(trakedImagePrefab);
            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Ư�� Pose�� ���� ���� (�׻� ���� ��ġ�� ����)
            Vector3 fixedPosition = new Vector3(10, 0, -5);
            Quaternion fixedRotation = Quaternion.Euler(0, updatedImage.pose.rotation.eulerAngles.y, 0); // (0, 0, 0)

            updatedImage.transform.SetPositionAndRotation(fixedPosition, fixedRotation);
            navigationBase.transform.SetPositionAndRotation(fixedPosition, fixedRotation);
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // ��Ŀ�� ���ŵǾ��� �� �ʿ��� ���� �߰� ����
        }
    }
   */ 
    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            navigationBase = GameObject.Instantiate(trakedImagePrefab);
            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Ư�� ��ġ ���� ����
            Vector3 fixedPosition = new Vector3(10, 0, -5);

            // ī�޶��� ȸ���� �ݿ��ϸ鼭�� 90������ ũ�� ����� �ʵ��� ����
            float cameraYRotation = updatedImage.pose.rotation.eulerAngles.y;
            float clampedRotationY = Mathf.Clamp(cameraYRotation, 75f, 105f); // 90�� �������� ��15�� ���

            Quaternion fixedRotation = Quaternion.Euler(0, clampedRotationY, 0); // ������ ȸ�� ����

            updatedImage.transform.SetPositionAndRotation(fixedPosition, fixedRotation);
            navigationBase.transform.SetPositionAndRotation(fixedPosition, fixedRotation);
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // ��Ŀ�� ���ŵǾ��� �� �ʿ��� ���� �߰� ����
        }
    }
    
}
