using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusMirrorToScreen : MonoBehaviour
{
    public RenderTexture renderTexture; // Render Texture

    void Start()
    {
        // Oculus ī�޶��� ��� ����
        Camera oculusCamera = Camera.main; // Oculus�� ���� ī�޶� ��������
        if (oculusCamera != null)
        {
            oculusCamera.targetTexture = renderTexture; // Render Texture�� ���
        }
        else
        {
            Debug.LogError("Oculus ���� ī�޶� ã�� �� �����ϴ�. Oculus ��Ⱑ ����Ǿ� �ִ��� Ȯ���ϼ���.");
        }
    }

    void OnDisable()
    {
        // Render Texture ���� ����
        Camera oculusCamera = Camera.main;
        if (oculusCamera != null)
        {
            oculusCamera.targetTexture = null;
        }
    }
}
