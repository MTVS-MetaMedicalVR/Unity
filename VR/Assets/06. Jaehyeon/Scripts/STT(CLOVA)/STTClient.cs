using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class STTClient : MonoBehaviour
{
    private string apiUrl = "http://localhost:8000/hoonjang_stt"; // Python 서버의 URL

    public void SendAudioData(byte[] wavData)
    {
        StartCoroutine(SendRequest(wavData));
    }

    private IEnumerator SendRequest(byte[] wavData)
    {
        // Base64 인코딩
        string base64Audio = System.Convert.ToBase64String(wavData);

        // JSON 데이터 생성
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
                Debug.Log("서버 응답: " + request.downloadHandler.text);
                HandleSTTResponse(request.downloadHandler.text);
            }
        }
    }

    private void HandleSTTResponse(string response)
    {
        // Clova STT 응답을 처리하는 로직을 작성합니다.
        // 예시: "아, 해보세요"가 감지되었을 경우 환자 오브젝트의 애니메이션 실행
        if (response.Contains("아 해보세요"))
        {
            // 환자 애니메이션 실행 코드 추가
            Debug.Log("환자 입 벌리는 애니메이션 실행");
        }
    }
}
