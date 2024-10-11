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

    //가장 처음 불리는 함수 == 변수들의 대한 초기화
    private void Awake()
    {
        //AR Session Orign에 Tracked Image Manager을 받아온다. 그리고 이걸 trackedImageManager에 저장한다. 
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        spawnedObjects = new Dictionary<string, GameObject>();


        //등록된 Object를 시작한 동시에 실체화를 시킨다. 
        foreach(GameObject obj in placeablePrefabs)
        {
            //INstantiate ==> static한 모델 자체를 인스턴트화 해주는 것
            GameObject newObject = Instantiate(obj);
            newObject.name = obj.name;
            //비활성화 해서 처음에는 안보이게 
            newObject.SetActive(false);


            spawnedObjects.Add(newObject.name, newObject);
        }

    }

    //위에 TrackedImageManager와 연결
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
