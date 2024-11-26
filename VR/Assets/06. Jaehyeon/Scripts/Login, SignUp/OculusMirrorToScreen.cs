using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusMirrorToScreen : MonoBehaviour
{
    public RenderTexture renderTexture; // Render Texture

    void Start()
    {
        // Oculus의 CenterEyeAnchor에서 Render Texture로 미러링
        Transform centerEyeAnchor = transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        if (centerEyeAnchor != null)
        {
            Camera oculusCamera = centerEyeAnchor.GetComponent<Camera>();
            if (oculusCamera != null)
            {
                oculusCamera.targetTexture = renderTexture; // Render Texture에 출력
            }
            else
            {
                Debug.LogError("Oculus CenterEyeAnchor에 Camera가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("CenterEyeAnchor를 찾을 수 없습니다. OVRCameraRig 설정을 확인하세요.");
        }
    }

    void OnDisable()
    {
        // Render Texture 연결 해제
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

