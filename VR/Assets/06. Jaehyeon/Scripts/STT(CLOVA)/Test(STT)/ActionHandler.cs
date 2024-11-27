using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public JawFollow jawController;  // 단일 JawFollow
    public HeadFollow headController;  // 단일 HeadFollow

    //private const float AccuracyThreshold = 0.8f; // 최소 인식률 (80%)
    // 명령어 리스트
    private readonly List<string> openMouthCommands = new List<string> { "아 해보세요", "아 해 보세요", "입 벌려 보세요", "아해 보세요", "해보세요" };
    private readonly List<string> closeMouthCommands = new List<string> { "다 물어보세요", "입 다물어 보세요", "다물어 보세요", "다물어보세요", "끝났습니다" };
    private readonly List<string> turnLeftCommands = new List<string> { "좌측으로 돌려 보세요", "왼쪽으로 돌려 보세요", "좌측으로" };
    private readonly List<string> turnRightCommands = new List<string> { "우측으로 돌려 보세요", "오른쪽으로 돌려 보세요", "우측으로" };

    private void OnEnable()
    {
        // STTClient의 이벤트 구독
        STTClient.OnSTTResponseReceived += HandleSTTResponse;
    }

    private void OnDisable()
    {
        // STTClient의 이벤트 구독 해제
        STTClient.OnSTTResponseReceived -= HandleSTTResponse;
    }

    private void HandleSTTResponse(string recognizedText, string recognitionAccuracy)
    {
        Debug.Log($"인식된 텍스트: {recognizedText} (정확도: {recognitionAccuracy})");

        if (IsCommandMatched(recognizedText, openMouthCommands))
        {
            HandleAction("OpenMouth");
        }
        else if (IsCommandMatched(recognizedText, closeMouthCommands))
        {
            HandleAction("CloseMouth");
        }
        else if (IsCommandMatched(recognizedText, turnLeftCommands))
        {
            HandleAction("TurnLeftHead");
        }
        else if (IsCommandMatched(recognizedText, turnRightCommands))
        {
            HandleAction("TurnRightHead");
        }
        else
        {
            Debug.Log("인식된 텍스트에 해당하는 동작이 없습니다.");
        }
    }

    // 주어진 응답이 명령어 리스트에 포함되어 있는지 확인하는 메서드
    private bool IsCommandMatched(string response, List<string> commands)
    {
        foreach (string command in commands)
        {
            if (response.Contains(command))
            {
                return true;
            }
        }
        return false;
    }

    public void HandleAction(string action)
    {
        switch (action)
        {
            case "OpenMouth":
                if (jawController != null)
                {
                    jawController.isOpen = true;
                    Debug.Log("환자 입 벌리기 동작 실행");
                }
                else
                {
                    Debug.LogWarning("JawFollow 컨트롤러가 설정되지 않았습니다.");
                }
                break;

            case "CloseMouth":
                if (jawController != null)
                {
                    jawController.isOpen = false;
                    Debug.Log("환자 입 닫기 동작 실행");
                }
                else
                {
                    Debug.LogWarning("JawFollow 컨트롤러가 설정되지 않았습니다.");
                }
                break;

            case "TurnLeftHead":
                if (headController != null)
                {
                    headController.isLeft = true;
                    headController.isRight = false;
                    Debug.Log("환자 왼쪽으로 고개 돌리기 동작 실행");
                }
                else
                {
                    Debug.LogWarning("HeadFollow 컨트롤러가 설정되지 않았습니다.");
                }
                break;

            case "TurnRightHead":
                if (headController != null)
                {
                    headController.isRight = true;
                    headController.isLeft = false;
                    Debug.Log("환자 오른쪽으로 고개 돌리기 동작 실행");
                }
                else
                {
                    Debug.LogWarning("HeadFollow 컨트롤러가 설정되지 않았습니다.");
                }
                break;

            default:
                Debug.Log("알 수 없는 동작: " + action);
                break;
        }
    }
}
