using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public ProcedureManager procedureManager;
    public Text stepDescriptionText;
    public List<GameObject> stepObjects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        procedureManager ??= FindObjectOfType<ProcedureManager>();
        if (procedureManager == null)
        {
            Debug.LogError("ProcedureManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    private void Start()
    {
        InitializeStepObjects();
        procedureManager.StartProcedure("hand_wash");
    }

    private void Update()
    {
        CenterUI();  // 카메라 위치에 따라 지속적으로 UI 중앙 정렬
    }

    private void InitializeStepObjects()
    {
        foreach (var stepObject in stepObjects)
        {
            stepObject.SetActive(false);
        }
    }

    public void UpdateStepDescription(string description)
    {
        if (stepDescriptionText != null)
        {
            stepDescriptionText.text = description;
        }
        else
        {
            Debug.LogError("단계 설명 텍스트가 없습니다.");
        }
    }

    public void SetStepObjectActive(string stepId, bool state)
    {
        var stepObject = stepObjects.Find(obj => obj.name.ToLower() == stepId);
        if (stepObject != null)
        {
            stepObject.SetActive(state);
        }
        else
        {
            Debug.LogError($"{stepId}에 해당하는 GameObject를 찾을 수 없습니다.");
        }
    }

    private void CenterUI()
    {
        Vector3 uiPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;
        transform.position = uiPosition;
        transform.LookAt(Camera.main.transform);
    }
}
