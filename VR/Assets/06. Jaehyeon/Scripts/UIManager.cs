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
    private bool isInitialized = false;

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (procedureManager == null)
        {
            procedureManager = FindObjectOfType<ProcedureManager>();
            if (procedureManager == null)
            {
                Debug.LogError("ProcedureManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            }
        }
    }

    private void Start()
    {
        cachedPosition = transform.position;  // �ʱ� ��ġ ĳ��
        InitializeToggles();  // ��� ��� �ʱ�ȭ
        CenterUI();  // UI�� ȭ�� �߾ӿ� ��ġ
        ShowProcedureUI(true);  // UI ǥ��
        procedureManager.StartProcedure("hand_wash");  // ù ��° ���� ����
    }

    // ��� ��� �ʱ�ȭ �� �̺�Ʈ ���
    private void InitializeToggles()
    {
        isInitialized = false;  // �ʱ�ȭ ����

        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;  // ��� ����� ��Ȱ��ȭ
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isInitialized && isOn)  // �ʱ�ȭ �Ϸ� �Ŀ��� �۵�
                    OnToggleActivated(toggle);
            });
        }

        isInitialized = true;  // �ʱ�ȭ �Ϸ�
    }

    // Toggle �� ���� �� ȣ��Ǵ� �޼���
    private void OnToggleActivated(Toggle toggle)
    {
        string stepId = toggle.name.ToLower();
        Debug.Log($"{stepId} �ܰ谡 Ȱ��ȭ�Ǿ����ϴ�.");

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

    private void Update()
    {
        transform.position = cachedPosition;  // �� �����Ӹ��� ��ġ ����
    }

    // UI�� ȭ�� �߾ӿ� ��ġ
    public void CenterUI()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);
        transform.position = Camera.main.ScreenToWorldPoint(screenCenter);
        Debug.Log("UI�� ȭ�� �߾ӿ� ��ġ�Ǿ����ϴ�.");
    }

    // Ư�� ��� ���� ������Ʈ
    public void UpdateToggleState(string stepId, bool state)
    {
        var toggle = stepToggles.Find(t => t.name.ToLower() == stepId);
        if (toggle != null)
        {
            toggle.isOn = state;
            toggle.gameObject.SetActive(state);  // UI ��� Ȱ��ȭ/��Ȱ��ȭ
            Debug.Log($"{stepId} �ܰ��� Toggle ���°� ������Ʈ�Ǿ����ϴ�: {state}");
        }
        else
        {
            Debug.LogError($"'{stepId}'�� �ش��ϴ� Toggle�� ã�� �� �����ϴ�.");
        }
    }

    // �ܰ� ������ UI�� ������Ʈ
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

    // ��� ��� �ʱ�ȭ
    public void ResetAllToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;
        }
        Debug.Log("��� ����� �ʱ�ȭ�Ǿ����ϴ�.");
    }
}
