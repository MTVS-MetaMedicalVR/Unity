using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class STTRequester : MonoBehaviour
{
    private const string API_URL = "https://clovaspeech-gw.ncloud.com/recog/v1/stt";
    private const string API_KEY = "93caee99af104496a286c6de690821d9"; // 발급받은 API 키 입력

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
            Debug.Log("STT 성공: " + request.downloadHandler.text);
            HandleResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("STT 요청 실패: " + request.error);
        }
    }

    private void HandleResponse(string jsonResponse)
    {
        // 받은 JSON 응답을 파싱하고 명령어에 따라 애니메이션 실행
        if (jsonResponse.Contains("아 해보세요"))
        {
            // 환자 오브젝트의 입 벌리기 애니메이션 호출
            Debug.Log("환자 입 벌리기 명령 실행");
            // 애니메이션 제어 로직 추가
        }
    }
}
