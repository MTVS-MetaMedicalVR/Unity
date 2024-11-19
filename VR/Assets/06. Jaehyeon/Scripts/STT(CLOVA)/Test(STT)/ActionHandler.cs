using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    //public List<JawFollow> jawControllers = new List<JawFollow>();
    //public List<HeadFollow> headControllers = new List<HeadFollow>();
    public JawFollow jawController;  // 단일 JawFollow
    public HeadFollow headController;  // 단일 HeadFollow

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

    private void HandleSTTResponse(string response)
    {
        if (response.Contains("아 해보세요"))
        {
            HandleAction("OpenMouth");
        }
        else if (response.Contains("다 물어보세요"))
        {
            HandleAction("CloseMouth");
        }
        else if (response.Contains("좌측으로 돌려 보세요"))
        {
            HandleAction("TurnLeftHead");
        }
        else if (response.Contains("우측으로 돌려 보세요"))
        {
            HandleAction("TurnRightHead");
        }
        else
        {
            Debug.Log("인식된 텍스트: " + response + " (해당하는 동작 없음)");
        }
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
