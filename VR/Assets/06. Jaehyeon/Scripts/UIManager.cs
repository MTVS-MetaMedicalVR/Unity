using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public List<Toggle> stepToggles;  // �� �ܰ迡 �ش��ϴ� ��� ����Ʈ
    public ProcedureManager procedureManager;  // ProcedureManager�� ����
    public Text stepDescriptionText;  // �ܰ� ���� �ؽ�Ʈ
    private Vector3 cachedPosition;

    private void Awake()
    {
        if (procedureManager == null)
        {
            procedureManager = FindObjectOfType<ProcedureManager>();
            if (procedureManager == null)
            {
                Debug.LogError("ProcedureManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            }
        }
        // �̱��� ���� ����
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        cachedPosition = new Vector3(0, 0, 0);
        InitializeToggles();  // ��� ��� �ʱ�ȭ
        CenterUI();           // UI�� ȭ�� �߾ӿ� ��ġ
        ShowProcedureUI(true);  // UI ǥ��
        procedureManager.StartProcedure("hand_wash");  // ù ��° ���� ����
    }

    // ��� ��� �ʱ�ȭ �� �̺�Ʈ ���
    private void InitializeToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;  // �ʱ�ȭ �� ��� Toggle�� ����
            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
        }
    }

    // Toggle �� ���� �� ȣ��Ǵ� �޼���
    private void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        if (isOn && procedureManager != null)
        {
            string stepId = toggle.name.ToLower();
            Debug.Log($"{stepId} �ܰ谡 Ȱ��ȭ�Ǿ����ϴ�.");

            if (procedureManager.GetCurrentStepId() == stepId)
            {
                procedureManager.CompleteStep(stepId);  // �ش� �ܰ� �Ϸ�
                toggle.isOn = false;  // �Ϸ� �� Toggle ��Ȱ��ȭ
            }
            else
            {
                Debug.LogError($"{stepId} �ܰ谡 ���� �ܰ�� ��ġ���� �ʽ��ϴ�.");
                toggle.isOn = false;  // ��ġ���� ������ �ٽ� ��Ȱ��ȭ
            }
        }
    }

    private void Update()
    {
        transform.position = cachedPosition;  // �� �����Ӹ��� �� ��ü ���� ��� ĳ�̵� �� ���
    }

    // UI�� ȭ�� �߾ӿ� ��ġ
    public void CenterUI()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);
        transform.position = Camera.main.ScreenToWorldPoint(screenCenter);
        Debug.Log("UI�� ȭ�� �߾ӿ� ��ġ�Ǿ����ϴ�.");
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


    // �ܰ� ���� ������Ʈ
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

    // UI ǥ��/����� �޼���
    public void ShowProcedureUI(bool show)
    {
        gameObject.SetActive(show);
        Debug.Log($"Procedure UI�� {(show ? "ǥ��" : "����")}�Ǿ����ϴ�.");
    }

    // ��� ����� �ʱ�ȭ�ϴ� �޼���
    public void ResetAllToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;  // ��� Toggle ��Ȱ��ȭ
        }
        Debug.Log("��� ����� �ʱ�ȭ�Ǿ����ϴ�.");
    }
}
