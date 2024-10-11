using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class MultipleImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedObjects;

    //���� ó�� �Ҹ��� �Լ� == �������� ���� �ʱ�ȭ
    private void Awake()
    {
        //AR Session Orign�� Tracked Image Manager�� �޾ƿ´�. �׸��� �̰� trackedImageManager�� �����Ѵ�. 
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        spawnedObjects = new Dictionary<string, GameObject>();


        //��ϵ� Object�� ������ ���ÿ� ��üȭ�� ��Ų��. 
        foreach(GameObject obj in placeablePrefabs)
        {
            //INstantiate ==> static�� �� ��ü�� �ν���Ʈȭ ���ִ� ��
            GameObject newObject = Instantiate(obj);
            newObject.name = obj.name;
            //��Ȱ��ȭ �ؼ� ó������ �Ⱥ��̰� 
            newObject.SetActive(false);


            spawnedObjects.Add(newObject.name, newObject);
        }

    }

    //���� TrackedImageManager�� ����
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }


    void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateSpawnObject(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateSpawnObject(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            spawnedObjects[trackedImage.name].SetActive(false);
        }
    }


    void UpdateSpawnObject(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;

        spawnedObjects[referenceImageName].transform.position = trackedImage.transform.position;
        spawnedObjects[referenceImageName].transform.rotation = trackedImage.transform.rotation;

        spawnedObjects[referenceImageName].SetActive(true);

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"There are {trackedImageManager.trackables.count} Images being trafcked");

        foreach(var trackedImage in trackedImageManager.trackables)
        {
            Debug.Log($"Image : {trackedImage.referenceImage.name} is at " + $"{trackedImage.transform.position}");
        }
    }
}
