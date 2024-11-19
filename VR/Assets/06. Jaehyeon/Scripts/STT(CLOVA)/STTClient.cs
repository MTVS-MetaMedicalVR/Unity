using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class STTClient : MonoBehaviour
{
    private string apiUrl = "http://192.168.204.27:8000/hoonjang_stt"; // Python 서버 URL
    public static event System.Action<string> OnSTTResponseReceived; // STT 응답 이벤트

    public void SendAudioData(byte[] wavData)
    {
        // Base64 인코딩
        string base64Audio = System.Convert.ToBase64String(wavData);

        // JSON 데이터 생성
        string jsonData = "{\"audio\":\"" + base64Audio + "\"}";

        // 요청 전송
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

            // 응답 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("오디오 데이터를 전송하는 중 오류 발생: " + request.error);
            }
            else
            {
                Debug.Log("서버 응답: " + request.downloadHandler.text);
                // 이벤트를 통해 응답 전달
                OnSTTResponseReceived?.Invoke(request.downloadHandler.text);
            }
        }
    }
}
