using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SyncSecondaryCamera : MonoBehaviour
{
    public Transform mainCamera;

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.position = mainCamera.position;
            transform.rotation = mainCamera.rotation;
        }
    }
}

