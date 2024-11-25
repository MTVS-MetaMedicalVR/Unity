using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class STTVoiceRecorder : MonoBehaviour
{
    public AudioClip audioClip;
    private string serverUrl = "http://192.168.204.27:8000/hoonjang_stt";  // Python 서버 URL
    private ActionController actionController;

    void Start()
    {
        // ActionController 인스턴스 찾기
        actionController = FindObjectOfType<ActionController>();

        // 마이크 권한 요청 (필요한 경우)
        StartCoroutine(RequestMicrophonePermission());
    }

    IEnumerator RequestMicrophonePermission()
    {
        // PC에서는 별도의 마이크 권한 요청을 건너뜁니다.
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
        else
        {
            yield break;  // PC에서는 즉시 반환
        }
    }


    public void StartRecording()
    {
        // 사용 가능한 마이크 장치 목록 출력
        string[] devices = Microphone.devices;
        if (devices.Length > 0)
        {
            // 첫 번째 장치를 기본으로 사용하거나 원하는 장치를 선택
            string selectedDevice = devices[0]; // "Microphone Array"로 변경할 수 있음
            audioClip = Microphone.Start(selectedDevice, false, 5, 16000);
            Debug.Log($"음성 녹음 시작 - 선택된 장치: {selectedDevice}");
        }
        else
        {
            Debug.LogWarning("사용 가능한 마이크 장치가 없습니다.");
        }
    }


    public void StopRecordingAndSend()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            Debug.Log("음성 녹음 종료");

            // 녹음된 오디오 데이터를 byte[]로 변환하여 서버에 전송
            float[] samples = new float[audioClip.samples];
            audioClip.GetData(samples, 0);
            byte[] audioData = ConvertAudioClipToByteArray(samples);

            StartCoroutine(SendAudioToServer(audioData));
        }
    }

    private byte[] ConvertAudioClipToByteArray(float[] samples)
    {
        // PCM 16비트 오디오 데이터로 변환
        byte[] data = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = (short)(samples[i] * short.MaxValue);
            data[i * 2] = (byte)(sample & 0xFF);
            data[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }
        return data;
    }

    IEnumerator SendAudioToServer(byte[] audioData)
    {
        string base64Audio = System.Convert.ToBase64String(audioData);
        string jsonPayload = "{\"audio\": \"" + base64Audio + "\"}";

        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("서버로 데이터 전송 중...");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("서버 응답 성공: " + www.downloadHandler.text);
                HandleServerResponse(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("서버 요청 실패: " + www.error);
            }
        }
    }


    private void HandleServerResponse(string responseJson)
    {
        // JSON 응답을 파싱하고 행동에 따라 VR 모델 조작
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJson);
        if (responseData.status == "success" && responseData.action != null)
        {
            if (actionController != null)
            {
                actionController.ExecuteAction(responseData.action);
            }
            else
            {
                Debug.LogWarning("ActionController가 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.Log("유효한 명령어가 없습니다.");
        }
    }

    [System.Serializable]
    public class ResponseData
    {
        public string status;
        public string text;
        public string matched_command;
        public float similarity;
        public string action;
    }
}
