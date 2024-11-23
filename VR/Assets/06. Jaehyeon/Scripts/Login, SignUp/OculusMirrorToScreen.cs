using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OculusMirrorToScreen : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Video Player ������Ʈ
    public RenderTexture renderTexture; // Render Texture

    void Start()
    {
        // Oculus �̷��� URL
        videoPlayer.url = "rtsp://192.168.137.1:554/live"; // RTSP ��Ʈ�� �̷��� �ּ�
        videoPlayer.targetTexture = renderTexture; // Render Texture�� ���
        videoPlayer.Play(); // ���� �÷��� ����
    }
}

