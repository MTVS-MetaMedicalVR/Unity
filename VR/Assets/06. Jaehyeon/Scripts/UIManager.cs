using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public List<Toggle> stepToggles;  // 각 단계에 해당하는 토글 리스트
    public ProcedureManager procedureManager;  // ProcedureManager와 연결
    public Text stepDescriptionText;  // 단계 설명 텍스트
    private Vector3 cachedPosition;

    private void Awake()
    {
        if (procedureManager == null)
        {
            procedureManager = FindObjectOfType<ProcedureManager>();
            if (procedureManager == null)
            {
                Debug.LogError("ProcedureManager 인스턴스를 찾을 수 없습니다.");
            }
        }
        // 싱글톤 패턴 적용
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        cachedPosition = new Vector3(0, 0, 0);
        InitializeToggles();  // 모든 토글 초기화
        CenterUI();           // UI를 화면 중앙에 배치
        ShowProcedureUI(true);  // UI 표시
        procedureManager.StartProcedure("hand_wash");  // 첫 번째 절차 시작
    }

    // 모든 토글 초기화 및 이벤트 등록
    private void InitializeToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;  // 초기화 시 모든 Toggle을 꺼둠
            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
        }
    }

    // Toggle 값 변경 시 호출되는 메서드
    private void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        if (isOn && procedureManager != null)
        {
            string stepId = toggle.name.ToLower();
            Debug.Log($"{stepId} 단계가 활성화되었습니다.");

            if (procedureManager.GetCurrentStepId() == stepId)
            {
                procedureManager.CompleteStep(stepId);  // 해당 단계 완료
                toggle.isOn = false;  // 완료 후 Toggle 비활성화
            }
            else
            {
                Debug.LogError($"{stepId} 단계가 현재 단계와 일치하지 않습니다.");
                toggle.isOn = false;  // 일치하지 않으면 다시 비활성화
            }
        }
    }

    private void Update()
    {
        transform.position = cachedPosition;  // 매 프레임마다 새 객체 생성 대신 캐싱된 값 사용
    }

    // UI를 화면 중앙에 배치
    public void CenterUI()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);
        transform.position = Camera.main.ScreenToWorldPoint(screenCenter);
        Debug.Log("UI가 화면 중앙에 배치되었습니다.");
    }

    public void UpdateToggleState(string stepId, bool state)
    {
        var toggle = stepToggles.Find(t => t.name.ToLower() == stepId);
        if (toggle != null)
        {
            toggle.isOn = state;
            Debug.Log($"{stepId} 단계의 Toggle 상태가 업데이트되었습니다: {state}");
        }
        else
        {
            Debug.LogError($"'{stepId}'에 해당하는 Toggle을 찾을 수 없습니다.");
        }
    }


    // 단계 설명 업데이트
    public void UpdateStepDescription(string description)
    {
        if (stepDescriptionText != null)
        {
            stepDescriptionText.text = description;
            Debug.Log($"단계 설명이 업데이트되었습니다: {description}");
        }
        else
        {
            Debug.LogError("단계 설명 텍스트가 할당되지 않았습니다.");
        }
    }

    // UI 표시/숨기기 메서드
    public void ShowProcedureUI(bool show)
    {
        gameObject.SetActive(show);
        Debug.Log($"Procedure UI가 {(show ? "표시" : "숨김")}되었습니다.");
    }

    // 모든 토글을 초기화하는 메서드
    public void ResetAllToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;  // 모든 Toggle 비활성화
        }
        Debug.Log("모든 토글이 초기화되었습니다.");
    }
}
