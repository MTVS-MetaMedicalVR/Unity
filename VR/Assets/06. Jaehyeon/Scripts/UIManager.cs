using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;  // 싱글톤 인스턴스
    public List<Toggle> stepToggles;   // 절차 단계별 Toggle 리스트
    public ProcedureManager procedureManager;  // ProcedureManager와 연결

    private void Awake()
    {
        // 싱글톤 패턴을 적용해 인스턴스의 중복 생성을 방지
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeToggles();  // Toggle 초기화
    }

    // 모든 Toggle에 On Value Changed 이벤트 리스너 등록
    private void InitializeToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
        }
    }

    // Toggle의 상태가 변경될 때 호출되는 메서드
    private void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        if (isOn)  // Toggle이 켜졌을 때만 실행
        {
            string stepId = toggle.name.ToLower();  // Toggle 이름에서 단계 ID 추출
            Debug.Log($"'{stepId}' 단계가 활성화되었습니다.");

            // ProcedureManager에 단계 완료 알림
            procedureManager.CompleteStep(stepId);
        }
    }

    // 특정 단계의 Toggle 상태를 업데이트
    public void UpdateToggleState(string stepId, bool state)
    {
        var toggle = stepToggles.Find(t => t.name.ToLower() == stepId);
        if (toggle != null)
        {
            toggle.isOn = state;  // 주어진 상태로 Toggle 업데이트
            Debug.Log($"{stepId} 단계의 Toggle 상태가 업데이트되었습니다: {state}");
        }
        else
        {
            Debug.LogError($"'{stepId}'에 해당하는 Toggle을 찾을 수 없습니다.");
        }
    }

    // UI 초기화: 모든 단계의 Toggle 상태를 초기화 (옵션)
    public void ResetAllToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;  // 모든 Toggle 비활성화
        }
        Debug.Log("모든 Toggle이 초기화되었습니다.");
    }
}
