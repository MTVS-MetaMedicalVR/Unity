using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class NewBehaviourScript : MonoBehaviour
{
   public ARRaycastManager raycastManager;
    public GameObject objecToPlace;
    public Camera arCamera;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();





    // Update is called once per frame
    void Update()
    {
        Ray ray = arCamera.ScreenPointToRay(Input.mousePosition); 

        if(Input.GetMouseButton(0))
        {
            if(raycastManager.Raycast(ray, hits, TrackableType.Planes) ) 
            {
                Pose hitPose = hits[0].pose;
                Instantiate(objecToPlace, hitPose.position, hitPose.rotation);
            
            }
        }

    }
}
