using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusMirrorToScreen : MonoBehaviour
{
    public RenderTexture renderTexture; // Render Texture

    void Start()
    {
        // Oculus 카메라의 출력 설정
        Camera oculusCamera = Camera.main; // Oculus의 메인 카메라 가져오기
        if (oculusCamera != null)
        {
            oculusCamera.targetTexture = renderTexture; // Render Texture에 출력
        }
        else
        {
            Debug.LogError("Oculus 메인 카메라를 찾을 수 없습니다. Oculus 기기가 연결되어 있는지 확인하세요.");
        }
    }

    void OnDisable()
    {
        // Render Texture 연결 해제
        Camera oculusCamera = Camera.main;
        if (oculusCamera != null)
        {
            oculusCamera.targetTexture = null;
        }
    }
}
