using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class STTClient : MonoBehaviour
{
    private string apiUrl = "http://192.168.35.9:9027/hoonjang_stt"; // Python 서버 URL
    public static event System.Action<string, string> OnSTTResponseReceived; // STT 응답 이벤트 (인식된 텍스트와 인식률)

    public void SendAudioData(byte[] wavData, string expectedText)
    {
        // Base64 인코딩
        string base64Audio = System.Convert.ToBase64String(wavData);

        // JSON 데이터 생성 (expected_text 포함)
        string jsonData = "{\"audio\":\"" + base64Audio + "\", \"expected_text\":\"" + expectedText + "\"}";

        // 요청 전송
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

            Debug.Log("JSON 데이터 전송: " + jsonData);
            LogToFile("JSON 데이터 전송: " + jsonData);

            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("오디오 데이터를 전송하는 중 오류 발생: " + request.error);
                LogToFile("오류 발생: " + request.error);
            }
            else
            {
                Debug.Log("서버 응답: " + request.downloadHandler.text);
                LogToFile("서버 응답: " + request.downloadHandler.text);

                // 서버 응답 파싱
                try
                {
                    var response = JsonUtility.FromJson<STTResponse>(request.downloadHandler.text);
                    Debug.Log("인식된 텍스트: " + response.hoonjang_stt);
                    Debug.Log("인식률: " + response.recognition_accuracy);
                    LogToFile("인식된 텍스트: " + response.hoonjang_stt);
                    LogToFile("인식률: " + response.recognition_accuracy);

                    // 이벤트를 통해 응답 전달
                    OnSTTResponseReceived?.Invoke(response.hoonjang_stt, response.recognition_accuracy);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("서버 응답 파싱 중 오류 발생: " + e.Message);
                    LogToFile("서버 응답 파싱 중 오류 발생: " + e.Message);
                }
            }
        }
    }

    // 서버 응답에 대한 클래스 정의
    [System.Serializable]
    public class STTResponse
    {
        public string hoonjang_stt; // 인식된 텍스트

        public string recognition_accuracy; // 인식률
    }
}
