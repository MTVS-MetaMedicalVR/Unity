//using UnityEngine;
//using UnityEngine.Networking;  // 웹 통신을 위한 네임스페이스
//using System.Collections;      // 코루틴 사용을 위한 네임스페이스
//using UnityEngine.Android;

//public class STTManager : MonoBehaviour
//{
//    [SerializeField] private string serverUrl = "http://192.168.0.2:8000";//

//    private bool isListening = false;  // 음성 인식 상태
//    private Coroutine sttCoroutine;    // STT 실행 코루틴

//    // STT 응답 이벤트 추가
//    public static event System.Action<STTResponse> OnSTTResponse;

//    private void Awake()
//    {
//        // 안드로이드 마이크 권한 요청
//#if PLATFORM_ANDROID
//        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
//        {
//            Permission.RequestUserPermission(Permission.Microphone);
//        }
//#endif
//    }
//    // 시작시 자동으로 음성 인식 시작
//    void Start()
//    {
//        SetupMicrophone(); // 마이크 설정 추가
//        StartListening();
//        Debug.Log("STT Manager가 실행되었습니다.");
//    }

//    private void SetupMicrophone()
//    {
//#if PLATFORM_ANDROID
//        // Oculus 마이크 디바이스 찾기
//        string[] devices = Microphone.devices;
//        foreach (string device in devices)
//        {
//            if (device.ToLower().Contains("oculus"))
//            {
//                Debug.Log($"Oculus 마이크 발견: {device}");
//                break;
//            }
//        }
//#endif
//    }

//    // 음성 인식 시작 함수
//    private void StartListening()
//    {
//        if (!isListening)
//        {
//            StartCoroutine(StartListeningRequest());
//        }
//    }

//    // 서버에 음성 인식 시작 요청
//    private IEnumerator StartListeningRequest()
//    {
//        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{serverUrl}/start_listening", ""))
//        {
//            yield return request.SendWebRequest();
//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                isListening = true;
//                Debug.Log("서버 연결 성공! 음성 인식을 시작합니다.");  // 서버 연결 성공 메시지
//                sttCoroutine = StartCoroutine(STTLoop());
//            }
//            else
//            {
//                Debug.LogError($"Error starting STT: {request.error}");
//                Debug.Log("서버 연결 실패! 서버 상태를 확인해주세요.");  // 서버 연결 실패 메시지
//            }
//        }
//    }

//    // 주기적으로 서버에 음성 인식 결과 요청
//    private IEnumerator STTLoop()
//    {
//        bool wasConnected = true;  // 이전 연결 상태 추적

//        while (isListening)
//        {
//            using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{serverUrl}/dental_stt", ""))
//            {
//                yield return request.SendWebRequest();
//                if (request.result == UnityWebRequest.Result.Success)
//                {
//                    wasConnected = true;  // 연결 상태 업데이트
//                    STTResponse response = JsonUtility.FromJson<STTResponse>(request.downloadHandler.text);

//                    if (response.is_valid_command && !string.IsNullOrEmpty(response.action))
//                    {
//                        Debug.Log($"인식된 명령: {response.text}");
//                        Debug.Log($"실행할 액션: {response.action}");
//                        // 이벤트 발생
//                        OnSTTResponse?.Invoke(response);
//                    }
//                }
//                else if (request.responseCode == 408)
//                {
//                    continue;
//                }
//                else
//                {
//                    if (wasConnected)  // 이전에 연결되어 있었다면
//                    {
//                        Debug.LogError("서버 연결이 끊어졌습니다! 서버 상태를 확인해주세요.");
//                        wasConnected = false;  // 연결 상태 업데이트
//                    }
//                    Debug.LogError($"Error in STT request: {request.error}");
//                }
//            }

//            // 연결이 끊어진 경우 잠시 대기
//            if (!wasConnected)
//            {
//                yield return new WaitForSeconds(5f);  // 5초 대기
//                Debug.Log("서버 재연결 시도 중...");
//            }
//        }
//    }

//    // 앱 종료시 정리
//    private void OnApplicationQuit()
//    {
//        isListening = false;
//        if (sttCoroutine != null)
//        {
//            StopCoroutine(sttCoroutine);
//        }
//    }
//}

//// 서버 응답을 받기 위한 데이터 구조
//[System.Serializable]
//public class STTResponse
//{
//    public string status;      // 응답 상태
//    public string text;        // 인식된 텍스트
//    public bool is_valid_command;  // 유효한 명령어 여부
//    public string action;      // 실행할 액션 이름
//}