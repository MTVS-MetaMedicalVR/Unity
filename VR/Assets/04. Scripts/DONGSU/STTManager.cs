using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class STTManager : MonoBehaviour
{
    [SerializeField] private string serverUrl = "http://192.168.137.1:8000/dental_stt";
    [SerializeField] private string serverBaseUrl = "http://192.168.137.1:8000";
    [SerializeField] private float volumeThreshold = 0.03f;
    [SerializeField] private float silenceDuration = 1.0f; // 침묵 시간 설정
    [SerializeField] private float maxRecordingTime = 10f; // 최대 녹음 시간 설정

    private AudioClip micClip;
    private bool isListening = false;
    private float silenceTimer = 0f; // 침묵 타이머
    private float recordingTimer = 0f; // 녹음 타이머

    // 이벤트 추가
    public static event System.Action<STTResponse> OnSTTResponse;

    void Start()
    {
        Debug.Log($"서버 URL: {serverUrl}");
        StartCoroutine(CheckServerConnection());
        StartCoroutine(RequestMicrophonePermission());
    }

    // 서버 연결 확인 함수 추가
    private IEnumerator CheckServerConnection()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverBaseUrl))
        {
            Debug.Log($"서버 연결 확인 중: {serverBaseUrl}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("서버 연결 성공!");
            }
            else
            {
                Debug.LogError($"서버 연결 실패: {request.error}");
                Debug.LogError($"서버 URL: {serverBaseUrl}");
            }
        }
    }

    private IEnumerator RequestMicrophonePermission()
    {
#if PLATFORM_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
        }
        yield return new WaitForSeconds(0.5f);
#endif

        SetupMicrophone();
        StartListening();
    }

    void SetupMicrophone()
    {
        string[] devices = Microphone.devices;
        foreach (string device in devices)
        {
            Debug.Log($"사용 가능한 마이크: {device}");
        }

        // Oculus 마이크 찾기
        string selectedDevice = null;
        foreach (string device in devices)
        {
            if (device.ToLower().Contains("oculus"))
            {
                selectedDevice = device;
                Debug.Log($"Oculus 마이크 발견: {device}");
                break;
            }
        }

        // 마이크 시작
        micClip = Microphone.Start(selectedDevice, true, (int)maxRecordingTime, 44100);
        if (micClip == null)
        {
            Debug.LogError("마이크 초기화 실패");
        }
        else
        {
            Debug.Log("마이크가 정상적으로 시작되었습니다.");
        }
    }

    void StartListening()
    {
        if (!isListening)
        {
            isListening = true;
            silenceTimer = 0f;
            recordingTimer = 0f;
            StartCoroutine(SendAudioToServer());
        }
    }

    private IEnumerator SendAudioToServer()
    {
        while (isListening)
        {
            float volume = AnalyzeVolume();

            if (volume > volumeThreshold)
            {
                Debug.Log($"음성 감지됨 (볼륨: {volume})");
                silenceTimer = 0f; // 침묵 타이머 초기화
            }
            else
            {
                silenceTimer += Time.deltaTime;
                Debug.Log($"침묵 중... (침묵 시간: {silenceTimer}s)");
            }

            recordingTimer += Time.deltaTime;

            // 침묵 시간이 초과되거나 최대 녹음 시간이 초과되면 전송
            if (silenceTimer > silenceDuration || recordingTimer > maxRecordingTime)
            {
                Debug.Log("음성이 종료되었다고 판단, 데이터 전송 시작...");
                yield return StartCoroutine(TransmitAudioData());

                // 타이머 초기화
                silenceTimer = 0f;
                recordingTimer = 0f;
            }

            yield return null; // 매 프레임 확인
        }
    }

    private float AnalyzeVolume()
    {
        if (micClip == null)
        {
            Debug.LogError("마이크 클립이 null입니다.");
            return 0f;
        }

        float[] samples = new float[1024];
        int micPosition = Microphone.GetPosition(null) - samples.Length + 1;

        if (micPosition < 0)
        {
            Debug.LogError("유효하지 않은 마이크 위치입니다.");
            return 0f;
        }

        micClip.GetData(samples, micPosition);

        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }

        float averageVolume = sum / samples.Length;
        Debug.Log($"현재 볼륨: {averageVolume}");

        return averageVolume;
    }

    private IEnumerator TransmitAudioData()
    {
        Debug.Log("오디오 데이터 전송 시작...");
        string filePath = Path.Combine(Application.persistentDataPath, "temp_audio.wav");
        SaveAudioToFile(micClip, filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", File.ReadAllBytes(filePath), "audio.wav", "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl, form))
        {
            Debug.Log($"서버로 전송 중... URL: {serverUrl}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"서버 응답 성공: {request.downloadHandler.text}");
                var response = JsonUtility.FromJson<STTResponse>(request.downloadHandler.text);
                OnSTTResponse?.Invoke(response);
            }
            else
            {
                Debug.LogError($"서버 통신 오류: {request.error}");
                Debug.LogError($"서버 URL: {serverUrl}");
            }
        }

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("임시 오디오 파일 삭제됨");
        }
    }

    void SaveAudioToFile(AudioClip clip, string path)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        byte[] wavFile = ConvertToWav(samples, clip.channels, clip.frequency);
        File.WriteAllBytes(path, wavFile);
    }

    byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * 2);
                writer.Write((short)(channels * 2));
                writer.Write((short)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (float sample in samples)
                {
                    writer.Write((short)(sample * 32767f));
                }

                return memoryStream.ToArray();
            }
        }
    }

    void OnDestroy()
    {
        isListening = false;
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
        }
    }
}

[System.Serializable]
public class STTResponse
{
    public string status;
    public string text;
    public string matched_command;
    public float similarity;
    public bool is_valid_command;
    public string action;
}
