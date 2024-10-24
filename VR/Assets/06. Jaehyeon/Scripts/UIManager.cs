using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public List<Toggle> stepToggles;
    public ProcedureManager procedureManager;
    public Text stepDescriptionText;  // �ܰ� ���� �ؽ�Ʈ UI

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeToggles();  // ��� Toggle �ʱ�ȭ
        CenterUI();           // UI�� ȭ�� �߾ӿ� ��ġ
        ShowProcedureUI(true); // UI ǥ��
        procedureManager.StartProcedure("hand_wash");  // ù ��° ���� ����
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
            Debug.Log($"{stepId} �ܰ谡 �Ϸ�Ǿ����ϴ�.");

            if (procedureManager != null && procedureManager.GetCurrentStepId() == stepId)
            {
                procedureManager.CompleteStep(stepId);  // �ش� �ܰ� �Ϸ�
            }
            else
            {
                Debug.LogError($"{stepId} �ܰ谡 ���� �ܰ�� ��ġ���� �ʽ��ϴ�.");
                toggle.isOn = false;  // ��ġ���� ������ �ٽ� ��Ȱ��ȭ
            }
        }
    }

    public void UpdateToggleState(string stepId, bool state)
    {
        var toggle = stepToggles.Find(t => t.name.ToLower() == stepId);
        if (toggle != null)
        {
            toggle.isOn = state;
            Debug.Log($"{stepId} �ܰ��� Toggle ���°� ������Ʈ�Ǿ����ϴ�: {state}");
        }
        else
        {
            Debug.LogError($"'{stepId}'�� �ش��ϴ� Toggle�� ã�� �� �����ϴ�.");
        }
    }


    public void CenterUI()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);
        transform.position = Camera.main.ScreenToWorldPoint(screenCenter);
        Debug.Log("UI�� ȭ�� �߾ӿ� ��ġ�Ǿ����ϴ�.");
    }

    public void UpdateStepDescription(string description)
    {
        if (stepDescriptionText != null)
        {
            stepDescriptionText.text = description;
            Debug.Log($"�ܰ� ������ ������Ʈ�Ǿ����ϴ�: {description}");
        }
        else
        {
            Debug.LogError("�ܰ� ���� �ؽ�Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void ShowProcedureUI(bool show)
    {
        gameObject.SetActive(show);
        Debug.Log($"Procedure UI�� {(show ? "ǥ��" : "����")}�Ǿ����ϴ�.");
    }

    public void ResetAllToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;
        }
        Debug.Log("��� Toggle�� �ʱ�ȭ�Ǿ����ϴ�.");
    }
}
