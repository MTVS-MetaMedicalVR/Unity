using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class STTClient : MonoBehaviour
{
    private string apiUrl = "http://192.168.0.61:8000/hoonjang_stt"; // Python ������ URL
    public Animator patientAnimator; // ȯ�� �ִϸ�����

    public void SendAudioData(byte[] wavData)
    {
        StartCoroutine(SendRequest(wavData));
    }

    private IEnumerator SendRequest(byte[] wavData)
    {
        // Base64 ���ڵ�
        string base64Audio = System.Convert.ToBase64String(wavData);

        // JSON ������ ����
        string jsonData = "{\"audio\":\"" + base64Audio + "\"}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("���� ����: " + request.downloadHandler.text);
                HandleSTTResponse(request.downloadHandler.text);
            }
        }
    }

    private void HandleSTTResponse(string response)
    {
        // Clova STT ������ ó���ϴ� ������ �ۼ��մϴ�.
        // ����: "�� �غ�����"�� �����Ǿ��� ��� ȯ�� ������Ʈ�� �ִϸ��̼� ����
        if (response.Contains("�� �غ�����"))
        {
            // ȯ�� �ִϸ��̼� ���� �ڵ� �߰�
            if (patientAnimator != null)
            {
                patientAnimator.SetTrigger("OpenMouth");
                Debug.Log("ȯ�� �� ������ �ִϸ��̼� ����");
            }
            else
            {
                Debug.LogWarning("ȯ�� �ִϸ����Ͱ� �������� �ʾҽ��ϴ�.");
            }
        }
        else
        {
            Debug.Log("�νĵ� �ؽ�Ʈ: " + response);
        }
    }
}
