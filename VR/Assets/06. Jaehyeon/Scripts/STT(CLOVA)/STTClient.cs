using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class STTClient : MonoBehaviour
{
    private string apiUrl = "http://192.168.35.9:9027/hoonjang_stt"; // Python ���� URL
    public static event System.Action<string, string> OnSTTResponseReceived; // STT ���� �̺�Ʈ (�νĵ� �ؽ�Ʈ�� �νķ�)

    public void SendAudioData(byte[] wavData, string expectedText)
    {
        // Base64 ���ڵ�
        string base64Audio = System.Convert.ToBase64String(wavData);

        // JSON ������ ���� (expected_text ����)
        string jsonData = "{\"audio\":\"" + base64Audio + "\", \"expected_text\":\"" + expectedText + "\"}";

        // ��û ����
        StartCoroutine(SendRequest(jsonData));
    }

    private void LogToFile(string message)
    {
        string path = Application.persistentDataPath + "/log.txt";
        System.IO.File.AppendAllText(path, message + "\n");
    }

    private IEnumerator SendRequest(string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("JSON ������ ����: " + jsonData);
            LogToFile("JSON ������ ����: " + jsonData);

            yield return request.SendWebRequest();

            // ���� ó��
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("����� �����͸� �����ϴ� �� ���� �߻�: " + request.error);
                LogToFile("���� �߻�: " + request.error);
            }
            else
            {
                Debug.Log("���� ����: " + request.downloadHandler.text);
                LogToFile("���� ����: " + request.downloadHandler.text);

                // ���� ���� �Ľ�
                try
                {
                    var response = JsonUtility.FromJson<STTResponse>(request.downloadHandler.text);
                    Debug.Log("�νĵ� �ؽ�Ʈ: " + response.hoonjang_stt);
                    Debug.Log("�νķ�: " + response.recognition_accuracy);
                    LogToFile("�νĵ� �ؽ�Ʈ: " + response.hoonjang_stt);
                    LogToFile("�νķ�: " + response.recognition_accuracy);

                    // �̺�Ʈ�� ���� ���� ����
                    OnSTTResponseReceived?.Invoke(response.hoonjang_stt, response.recognition_accuracy);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("���� ���� �Ľ� �� ���� �߻�: " + e.Message);
                    LogToFile("���� ���� �Ľ� �� ���� �߻�: " + e.Message);
                }
            }
        }
    }

    // ���� ���信 ���� Ŭ���� ����
    [System.Serializable]
    public class STTResponse
    {
        public string hoonjang_stt; // �νĵ� �ؽ�Ʈ

        public string recognition_accuracy; // �νķ�
    }
}
