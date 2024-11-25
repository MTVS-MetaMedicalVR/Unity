using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusMirrorToScreen : MonoBehaviour
{
    public RenderTexture renderTexture; // Render Texture

    void Start()
    {
        // Oculus�� CenterEyeAnchor���� Render Texture�� �̷���
        Transform centerEyeAnchor = transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        if (centerEyeAnchor != null)
        {
            Camera oculusCamera = centerEyeAnchor.GetComponent<Camera>();
            if (oculusCamera != null)
            {
                oculusCamera.targetTexture = renderTexture; // Render Texture�� ���
            }
            else
            {
                Debug.LogError("Oculus CenterEyeAnchor�� Camera�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("CenterEyeAnchor�� ã�� �� �����ϴ�. OVRCameraRig ������ Ȯ���ϼ���.");
        }
    }

    void OnDisable()
    {
        // Render Texture ���� ����
        Transform centerEyeAnchor = transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        if (centerEyeAnchor != null)
        {
            Camera oculusCamera = centerEyeAnchor.GetComponent<Camera>();
            if (oculusCamera != null)
            {
                oculusCamera.targetTexture = null;
            }
        }
    }
}

