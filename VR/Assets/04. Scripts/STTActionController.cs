using UnityEngine;

public class STTActionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private STTManager sttManager;    // STT 매니저
    [SerializeField] private JawFollow jawController;  // 턱 컨트롤러
    [SerializeField] private HeadFollow headController; // 목 컨트롤러

    // 다른 컨트롤러들도 필요하다면 여기에 추가
    // [SerializeField] private EyeController eyeController;

    private void OnEnable()
    {
        // STT 이벤트 구독
        STTManager.OnSTTResponse += HandleSTTResponse;
    }

    private void OnDisable()
    {
        // 구독 해제
        STTManager.OnSTTResponse -= HandleSTTResponse;
    }

    private void Start()
    {
        // 컴포넌트들이 할당되지 않았다면 자동으로 찾기
        if (sttManager == null)
            sttManager = FindObjectOfType<STTManager>();

        if (jawController == null)
            jawController = FindObjectOfType<JawFollow>();
        
        if (headController == null)
            headController = FindObjectOfType<HeadFollow>();
    }

    // STT 응답 처리
    private void HandleSTTResponse(STTResponse response)
    {
        if (!response.is_valid_command || string.IsNullOrEmpty(response.action))
            return;

        Debug.Log($"STT 액션 컨트롤러: {response.text} 명령 수신");

        // 액션에 따른 동작 처리
        switch (response.action)
        {
            case "OpenMouth":
                if (jawController != null)
                {
                    jawController.isOpen = true;
                    Debug.Log("입 벌리기 동작 실행");
                }
                break;

            case "CloseMouth":
                if (jawController != null)
                {
                    jawController.isOpen = false;
                    Debug.Log("입 닫는 동작 실행");
                }
                break;

            case "TurnRightHead":
                if (headController != null)
                {
                    headController.isRight = true;
                    headController.isLeft = false;
                    Debug.Log("오른쪽으로 고개 돌리기 실행");
                }
                break;

            case "TurnLeftHead":
                if (headController != null)
                {
                    headController.isLeft = true;
                    headController.isRight = false;
                    Debug.Log("왼쪽으로 고개 돌리기 실행");
                }
                break;

            // 다른 액션들도 여기에 추가
            // case "TurnHead":
            //     if (headController != null)
            //         headController.TurnHead();
            //     break;

            // case "CloseEyes":
            //     if (eyeController != null)
            //         eyeController.CloseEyes();
            //     break;

            default:
                Debug.Log($"정의되지 않은 액션: {response.action}");
                break;
        }
    }

    // 필요한 경우 수동으로 액션 실행
    public void ExecuteAction(string actionName)
    {
        HandleSTTResponse(new STTResponse
        {
            is_valid_command = true,
            action = actionName
        });
    }
}