using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;  // 싱글톤 인스턴스
    public List<Toggle> stepToggles;            // 각 단계의 토글 리스트
    public Text stepDescriptionText;            // 단계 설명 텍스트 UI
    public List<string> stepIds;                // 단계별 ID 관리
    private int currentStepIndex = 0;           // 현재 단계 인덱스

    private void Awake()
    {
        // 싱글톤 패턴 적용: 인스턴스가 하나만 존재하게 만듦
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeToggles();     // 모든 토글 초기화 및 이벤트 연결
        UpdateUI();              // UI 업데이트
    }

    // 1. 토글 초기화 및 이벤트 설정
    private void InitializeToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnStepCompleted(toggle.name);
            });
        }
    }

    // 2. 단계 완료 시 호출되는 메서드
    private void OnStepCompleted(string stepId)
    {
        if (stepId.ToLower() == stepIds[currentStepIndex].ToLower())
        {
            Debug.Log($"{stepId} 단계가 완료되었습니다.");
            stepToggles[currentStepIndex].isOn = false; // 현재 토글 비활성화
            currentStepIndex++;                         // 다음 단계로 이동

            if (currentStepIndex < stepIds.Count)
                UpdateUI();  // 다음 단계로 UI 업데이트
            else
                Debug.Log("모든 절차가 완료되었습니다.");
        }
        else
        {
            Debug.LogError($"잘못된 단계: {stepId}가 현재 단계와 일치하지 않습니다.");
        }
    }

    // 3. UI 업데이트 메서드 (단계 설명 텍스트 업데이트)
    private void UpdateUI()
    {
        if (currentStepIndex < stepIds.Count)
        {
            string currentStepId = stepIds[currentStepIndex];
            stepDescriptionText.text = $"현재 단계: {currentStepId}";
            Debug.Log($"UI가 업데이트되었습니다: {currentStepId}");
        }
    }
}
