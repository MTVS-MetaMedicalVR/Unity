using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class STTRequester : MonoBehaviour
{
    private const string API_URL = "https://clovaspeech-gw.ncloud.com/recog/v1/stt";
    private const string API_KEY = "93caee99af104496a286c6de690821d9"; // �߱޹��� API Ű �Է�

    public IEnumerator SendAudioFile(string filePath)
    {
        byte[] audioData = System.IO.File.ReadAllBytes(filePath);
        UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
        request.uploadHandler = new UploadHandlerRaw(audioData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/octet-stream");
        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", "YOUR_CLIENT_ID");
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", API_KEY);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("STT ����: " + request.downloadHandler.text);
            HandleResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("STT ��û ����: " + request.error);
        }
    }

    private void HandleResponse(string jsonResponse)
    {
        // ���� JSON ������ �Ľ��ϰ� ��ɾ ���� �ִϸ��̼� ����
        if (jsonResponse.Contains("�� �غ�����"))
        {
            // ȯ�� ������Ʈ�� �� ������ �ִϸ��̼� ȣ��
            Debug.Log("ȯ�� �� ������ ��� ����");
            // �ִϸ��̼� ���� ���� �߰�
        }
    }
}
