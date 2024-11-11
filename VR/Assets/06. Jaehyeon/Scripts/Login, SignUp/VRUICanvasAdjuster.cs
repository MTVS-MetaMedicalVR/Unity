using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRUICanvasAdjuster : MonoBehaviour
{
    public RectTransform targetUI;
    public Transform vrCamera;
    public Vector3 offset = new Vector3(0, 0, 2);
    public float uiScaleFactor = 0.1f;

    void Start()
    {
        if (targetUI == null || vrCamera == null)
        {
            Debug.LogWarning("Target UI or VR Camera is not assigned.");
            return;
        }
        AdjustUICanvas();
    }

    void AdjustUICanvas()
    {
        targetUI.position = vrCamera.position + vrCamera.forward * offset.z + vrCamera.right * offset.x + vrCamera.up * offset.y;
        targetUI.rotation = Quaternion.LookRotation(targetUI.position - vrCamera.position);
        targetUI.localScale = Vector3.one * uiScaleFactor;
    }
}
