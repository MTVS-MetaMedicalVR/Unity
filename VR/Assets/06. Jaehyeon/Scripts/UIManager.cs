using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스

public class UIManager : MonoBehaviour
{
    // UI 요소

    // 메인 메뉴 패널
    public GameObject mainMenuPanel;
    // 절차 안내 패널
    public GameObject procedurePanel;
    // 현재 절차 단계 안내 텍스트
    public TextMeshProUGUI stepText;
    // 피드백 텍스트
    public TextMeshProUGUI feedbackText;
    // 절차 완료 시 표시될 패널
    public GameObject completionPanel; 

    private void Start()
    {
        // 게임 시작 시 메인 메뉴를 표시
        ShowMainMenu(); 
    }

    // 메인 메뉴 표시
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        procedurePanel.SetActive(false);
        // 피드백 텍스트 초기화
        feedbackText.text = ""; 
        // 완료 패널 비활성화
        completionPanel.SetActive(false); 
    }

    // 절차 패널 표시
    public void ShowProcedurePanel()
    {
        mainMenuPanel.SetActive(false);
        procedurePanel.SetActive(true);
        // 피드백 초기화
        feedbackText.text = ""; 
    }

    // 절차 단계 텍스트 업데이트
    public void UpdateStepText(string stepDescription)
    {
        stepText.text = stepDescription;
    }

    // 피드백 표시
    public void ShowFeedback(string feedback)
    {
        // 피드백 텍스트 업데이트
        feedbackText.text = feedback;
        // 일정 시간 후에 피드백을 숨기기 위한 코루틴 실행
        StartCoroutine(HideFeedbackAfterDelay()); 
    }

    // 피드백을 일정 시간 후에 숨기기 위한 코루틴
    private IEnumerator HideFeedbackAfterDelay()
    {
        // 2초 후에 피드백 텍스트를 초기화
        yield return new WaitForSeconds(2.0f); 
        feedbackText.text = "";
    }

    // 절차 완료 처리
    public void ShowCompletion()
    {
        // 절차 패널 비활성화
        procedurePanel.SetActive(false);
        // 완료 패널 활성화
        completionPanel.SetActive(true); 
    }
}
