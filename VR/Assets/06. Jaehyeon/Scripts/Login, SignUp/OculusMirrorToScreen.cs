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
        videoPlayer.url = "https://www.oculus.com/casting"; // Oculus �̷��� �ּ�
        videoPlayer.targetTexture = renderTexture; // Render Texture�� ���
        videoPlayer.Play(); // ���� �÷��� ����
    }
}

