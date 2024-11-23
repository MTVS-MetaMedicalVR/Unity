using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OculusMirrorToScreen : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Video Player 컴포넌트
    public RenderTexture renderTexture; // Render Texture

    void Start()
    {
        // Oculus 미러링 URL
        videoPlayer.url = "https://www.oculus.com/casting"; // Oculus 미러링 주소
        videoPlayer.targetTexture = renderTexture; // Render Texture로 출력
        videoPlayer.Play(); // 비디오 플레이 시작
    }
}

