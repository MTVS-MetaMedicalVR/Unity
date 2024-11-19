using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class STTClient : MonoBehaviour
{
    private string apiUrl = "http://192.168.204.27:8000/hoonjang_stt"; // Python ���� URL
    public List<JawFollow> jawControllers = new List<JawFollow>(); // JawFollow ��ũ��Ʈ ����Ʈ
    public List<HeadFollow> headControllers = new List<HeadFollow>(); // HeadFollow ��ũ��Ʈ ����Ʈ

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
                HandleSTTResponse(request.downloadHandler.text);
            }
        }
    }

    private void HandleSTTResponse(string response)
    {
        // Ư�� ��ɾ ���� Jaw�� Head ���� ����
        foreach (var jawController in jawControllers)
        {
            if (jawController != null)
            {
                if (response.Contains("�� �غ�����"))
                {
                    jawController.isOpen = true; // �� ������
                    Debug.Log("ȯ�� �� ������ ���� ����");
                }
                else if (response.Contains("���� �ݾƺ�����"))
                {
                    jawController.isOpen = false; // �� �ݱ�
                    Debug.Log("ȯ�� �� �ݱ� ���� ����");
                }
                else
                {
                    Debug.Log("�νĵ� �ؽ�Ʈ (Jaw ����): " + response + " (�ش��ϴ� ���� ����)");
                }
            }
            else
            {
                Debug.LogWarning("JawFollow ��Ʈ�ѷ��� �������� �ʾҽ��ϴ�.");
            }
        }

        foreach (var headController in headControllers)
        {
            if (headController != null)
            {
                if (response.Contains("�������� ���� ����������"))
                {
                    headController.isLeft = true;
                    headController.isRight = false;
                    Debug.Log("ȯ�� �������� �� ������ ���� ����");
                }
                else if (response.Contains("���������� ���� ����������"))
                {
                    headController.isLeft = false;
                    headController.isRight = true;
                    Debug.Log("ȯ�� ���������� �� ������ ���� ����");
                }
                else
                {
                    headController.isLeft = false;
                    headController.isRight = false; // �⺻ ���·� ����
                    Debug.Log("�νĵ� �ؽ�Ʈ (Head ����): " + response + " (�ش��ϴ� ���� ����)");
                }
            }
            else
            {
                Debug.LogWarning("HeadFollow ��Ʈ�ѷ��� �������� �ʾҽ��ϴ�.");
            }
        }
    }
}
