using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class STTClient : MonoBehaviour
{
    private string apiUrl = "http://192.168.204.27:8000/hoonjang_stt"; // Python ���� URL
    public static event System.Action<string> OnSTTResponseReceived; // STT ���� �̺�Ʈ

    public void SendAudioData(byte[] wavData)
    {
        // Base64 ���ڵ�
        string base64Audio = System.Convert.ToBase64String(wavData);

        // JSON ������ ����
        string jsonData = "{\"audio\":\"" + base64Audio + "\"}";

        // ��û ����
        StartCoroutine(SendRequest(jsonData));
    }

    private IEnumerator SendRequest(string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            // ���� ó��
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("����� �����͸� �����ϴ� �� ���� �߻�: " + request.error);
            }
            else
            {
                Debug.Log("���� ����: " + request.downloadHandler.text);
                // �̺�Ʈ�� ���� ���� ����
                OnSTTResponseReceived?.Invoke(request.downloadHandler.text);
            }
        }
    }
}
