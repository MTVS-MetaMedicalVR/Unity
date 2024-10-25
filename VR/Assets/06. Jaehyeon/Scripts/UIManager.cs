using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public ProcedureManager procedureManager;
    public Text stepDescriptionText;  // 단계 설명 텍스트
    public List<GameObject> stepObjects;  // 모든 단계별 GameObject

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (procedureManager == null)
        {
            procedureManager = FindObjectOfType<ProcedureManager>();
            if (procedureManager == null)
                Debug.LogError("ProcedureManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    private void Start()
    {
        InitializeStepObjects();  // 단계 개체 초기화
        CenterUI();               // UI를 화면 중앙에 배치
        procedureManager.StartProcedure("hand_wash");  // 절차 시작
    }

    // 단계별 GameObject 초기화
    private void InitializeStepObjects()
    {
        foreach (var stepObject in stepObjects)
        {
            Button button = stepObject.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnStepObjectClicked(stepObject));
                Debug.Log($"{stepObject.name} 버튼이 초기화되었습니다.");
            }
            else
            {
                Debug.LogWarning($"{stepObject.name}에 Button 컴포넌트가 없습니다.");
            }

            stepObject.SetActive(false);  // 초기에는 모든 단계 비활성화
        }
    }

    // 단계별 GameObject 클릭 시 호출되는 메서드
    private void OnStepObjectClicked(GameObject stepObject)
    {
        string stepId = stepObject.name.ToLower();
        Debug.Log($"{stepId} 단계가 완료되었습니다.");

        if (procedureManager != null && procedureManager.GetCurrentStepId() == stepId)
        {
            procedureManager.CompleteStep(stepId);  // 해당 단계 완료 처리
            stepObject.SetActive(false);  // 단계 비활성화
        }
        else
        {
            Debug.LogError($"{stepId} 단계가 현재 단계와 일치하지 않습니다.");
        }
    }

    // UI를 플레이어 시야 중앙에 배치
    public void CenterUI()
    {
        Vector3 uiPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;
        transform.position = uiPosition;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        Debug.Log("UI가 화면 중앙에 배치되었습니다.");
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
            Debug.LogError("단계 설명 텍스트가 없습니다.");
        }
    }

    // 특정 단계 GameObject 활성화/비활성화
    public void SetStepObjectActive(string stepId, bool state)
    {
        var stepObject = stepObjects.Find(obj => obj.name.ToLower() == stepId);
        if (stepObject != null)
        {
            stepObject.SetActive(state);
            Debug.Log($"{stepId} 단계가 {(state ? "활성화" : "비활성화")}되었습니다.");
        }
        else
        {
            Debug.LogError($"{stepId}에 해당하는 GameObject를 찾을 수 없습니다.");
        }
    }
}
