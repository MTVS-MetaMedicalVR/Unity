using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;  // �̱��� �ν��Ͻ�
    public List<Toggle> stepToggles;   // ���� �ܰ躰 Toggle ����Ʈ
    public ProcedureManager procedureManager;  // ProcedureManager�� ����

    private void Awake()
    {
        // �̱��� ������ ������ �ν��Ͻ��� �ߺ� ������ ����
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeToggles();  // Toggle �ʱ�ȭ
    }

    // ��� Toggle�� On Value Changed �̺�Ʈ ������ ���
    private void InitializeToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
        }
    }

    // Toggle�� ���°� ����� �� ȣ��Ǵ� �޼���
    private void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        if (isOn)  // Toggle�� ������ ���� ����
        {
            string stepId = toggle.name.ToLower();  // Toggle �̸����� �ܰ� ID ����
            Debug.Log($"'{stepId}' �ܰ谡 Ȱ��ȭ�Ǿ����ϴ�.");

            // ProcedureManager�� �ܰ� �Ϸ� �˸�
            procedureManager.CompleteStep(stepId);
        }
    }

    // Ư�� �ܰ��� Toggle ���¸� ������Ʈ
    public void UpdateToggleState(string stepId, bool state)
    {
        var toggle = stepToggles.Find(t => t.name.ToLower() == stepId);
        if (toggle != null)
        {
            toggle.isOn = state;  // �־��� ���·� Toggle ������Ʈ
            Debug.Log($"{stepId} �ܰ��� Toggle ���°� ������Ʈ�Ǿ����ϴ�: {state}");
        }
        else
        {
            Debug.LogError($"'{stepId}'�� �ش��ϴ� Toggle�� ã�� �� �����ϴ�.");
        }
    }

    // UI �ʱ�ȭ: ��� �ܰ��� Toggle ���¸� �ʱ�ȭ (�ɼ�)
    public void ResetAllToggles()
    {
        foreach (var toggle in stepToggles)
        {
            toggle.isOn = false;  // ��� Toggle ��Ȱ��ȭ
        }
        Debug.Log("��� Toggle�� �ʱ�ȭ�Ǿ����ϴ�.");
    }
}
