using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class AudioProcessor : MonoBehaviour
{
    public MicrophoneInput microphoneInput;
    private AudioClip recordedClip;
    private string apiUrl = "http://192.168.0.61:8000/hoonjang_stt"; // API 서버 URL을 설정하세요.

    void Start()
    {
        if (microphoneInput != null)
        {
            recordedClip = microphoneInput.GetRecordedClip();
            if (recordedClip != null)
            {
                ProcessAudioClip(recordedClip);
            }
            else
            {
                Debug.LogWarning("No audio clip found from MicrophoneInput.");
            }
        }
        else
        {
            Debug.LogError("MicrophoneInput is not assigned in the AudioProcessor.");
        }
    }

    void ProcessAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            Debug.Log("Processing recorded audio clip...");

            // AudioClip을 WAV 바이트 배열로 변환
            byte[] wavData = WavUtility.FromAudioClip(clip);

            if (wavData != null)
            {
                // WAV 데이터를 Base64 문자열로 변환
                string base64Audio = Convert.ToBase64String(wavData);
                Debug.Log("Base64 Encoded Audio Data: " + base64Audio);

                // Base64 데이터를 API로 전송
                StartCoroutine(SendAudioToAPI(base64Audio));
            }
            else
            {
                Debug.LogWarning("Failed to convert AudioClip to WAV data.");
            }
        }
        else
        {
            Debug.LogWarning("No audio clip found to process.");
        }
    }

    IEnumerator SendAudioToAPI(string base64Audio)
    {
        // JSON 데이터 생성
        string jsonData = "{\"audio\":\"" + base64Audio + "\"}";

        // UnityWebRequest 설정
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 전송
            yield return request.SendWebRequest();

            // 응답 확인
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending audio data: " + request.error);
            }
            else
            {
                Debug.Log("Server response: " + request.downloadHandler.text);
                // 응답 데이터를 처리하는 로직 추가 가능
            }
        }
    }
}
