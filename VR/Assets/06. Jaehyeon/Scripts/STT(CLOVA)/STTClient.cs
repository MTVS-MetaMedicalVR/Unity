using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class STTClient : MonoBehaviour
{
    private string apiUrl = "http://192.168.0.61:8000/hoonjang_stt"; // Python 서버의 URL
    public List<JawFollow> jawControllers = new List<JawFollow>(); // JawFollow 스크립트 리스트
    public List<HeadFollow> headControllers = new List<HeadFollow>(); // HeadFollow 스크립트 리스트

    public void SendAudioData(byte[] wavData)
    {
        // Base64 인코딩
        string base64Audio = System.Convert.ToBase64String(wavData);

        // JSON 데이터 생성 (Base64 인코딩된 문자열을 포함)
        string jsonData = "{\"audio\":\"" + base64Audio + "\"}";

        // 요청 시작
        StartCoroutine(SendRequest(jsonData));
    }

    private IEnumerator SendRequest(string jsonData)
    {
        // UnityWebRequest 설정
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 전송
            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending audio data: " + request.error);
            }
            else
            {
                Debug.Log("Server response: " + request.downloadHandler.text);
                HandleSTTResponse(request.downloadHandler.text);
            }
        }
    }

    private void HandleSTTResponse(string response)
    {
        // Jaw and Head control logic based on STT response
        foreach (var jawController in jawControllers)
        {
            if (jawController != null)
            {
                if (response.Contains("아 해보세요"))
                {
                    jawController.isOpen = true; // Open jaw
                    Debug.Log("환자 입 벌리기 동작 실행");
                }
                else if (response.Contains("입을 닫아보세요"))
                {
                    jawController.isOpen = false; // Close jaw
                    Debug.Log("환자 입 닫기 동작 실행");
                }
                else
                {
                    Debug.Log("인식된 텍스트 (Jaw 관련): " + response + " (해당하는 동작 없음)");
                }
            }
            else
            {
                Debug.LogWarning("JawFollow 컨트롤러가 설정되지 않았습니다.");
            }
        }

        foreach (var headController in headControllers)
        {
            if (headController != null)
            {
                if (response.Contains("왼쪽으로 고개를 돌려보세요"))
                {
                    headController.isLeft = true;
                    headController.isRight = false;
                    Debug.Log("환자 왼쪽으로 고개 돌리기 동작 실행");
                }
                else if (response.Contains("오른쪽으로 고개를 돌려보세요"))
                {
                    headController.isLeft = false;
                    headController.isRight = true;
                    Debug.Log("환자 오른쪽으로 고개 돌리기 동작 실행");
                }
                else
                {
                    headController.isLeft = false;
                    headController.isRight = false; // Reset head position if no specific command
                    Debug.Log("인식된 텍스트 (Head 관련): " + response + " (해당하는 동작 없음)");
                }
            }
            else
            {
                Debug.LogWarning("HeadFollow 컨트롤러가 설정되지 않았습니다.");
            }
        }
    }
}
