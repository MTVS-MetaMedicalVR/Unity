using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public List<Toggle> stepToggles;
    public ProcedureManager procedureManager;
    public Text stepDescriptionText;  // 단계 설명 텍스트 UI

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeToggles();  // 모든 Toggle 초기화
        CenterUI();           // UI를 화면 중앙에 배치
        ShowProcedureUI(true); // UI 표시
        procedureManager.StartProcedure("hand_wash");  // 첫 번째 절차 시작
    }

    private void InitializeToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.onValueChanged.AddListener(delegate {
                OnToggleValueChanged(toggle, toggle.isOn);
            });
        }
    }


    private void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        if (isOn)
        {
            string stepId = toggle.name.ToLower();
            Debug.Log($"{stepId} 단계가 완료되었습니다.");

            if (procedureManager != null && procedureManager.GetCurrentStepId() == stepId)
            {
                procedureManager.CompleteStep(stepId);  // 해당 단계 완료
            }
            else
            {
                Debug.LogError($"{stepId} 단계가 현재 단계와 일치하지 않습니다.");
                toggle.isOn = false;  // 일치하지 않으면 다시 비활성화
            }
        }
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


    public void CenterUI()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);
        transform.position = Camera.main.ScreenToWorldPoint(screenCenter);
        Debug.Log("UI가 화면 중앙에 배치되었습니다.");
    }

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

    public void ShowProcedureUI(bool show)
    {
        gameObject.SetActive(show);
        Debug.Log($"Procedure UI가 {(show ? "표시" : "숨김")}되었습니다.");
    }

    public void ResetAllToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;
        }
        Debug.Log("모든 Toggle이 초기화되었습니다.");
    }
}
