using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;  // �̱��� �ν��Ͻ�
    public List<Toggle> stepToggles;            // �� �ܰ��� ��� ����Ʈ
    public Text stepDescriptionText;            // �ܰ� ���� �ؽ�Ʈ UI
    public List<string> stepIds;                // �ܰ躰 ID ����
    private int currentStepIndex = 0;           // ���� �ܰ� �ε���

    private void Awake()
    {
        // �̱��� ���� ����: �ν��Ͻ��� �ϳ��� �����ϰ� ����
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeToggles();     // ��� ��� �ʱ�ȭ �� �̺�Ʈ ����
        UpdateUI();              // UI ������Ʈ
    }

    // 1. ��� �ʱ�ȭ �� �̺�Ʈ ����
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

    // 2. �ܰ� �Ϸ� �� ȣ��Ǵ� �޼���
    private void OnStepCompleted(string stepId)
    {
        if (stepId.ToLower() == stepIds[currentStepIndex].ToLower())
        {
            Debug.Log($"{stepId} �ܰ谡 �Ϸ�Ǿ����ϴ�.");
            stepToggles[currentStepIndex].isOn = false; // ���� ��� ��Ȱ��ȭ
            currentStepIndex++;                         // ���� �ܰ�� �̵�

            if (currentStepIndex < stepIds.Count)
                UpdateUI();  // ���� �ܰ�� UI ������Ʈ
            else
                Debug.Log("��� ������ �Ϸ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError($"�߸��� �ܰ�: {stepId}�� ���� �ܰ�� ��ġ���� �ʽ��ϴ�.");
        }
    }

    // 3. UI ������Ʈ �޼��� (�ܰ� ���� �ؽ�Ʈ ������Ʈ)
    private void UpdateUI()
    {
        if (currentStepIndex < stepIds.Count)
        {
            string currentStepId = stepIds[currentStepIndex];
            stepDescriptionText.text = $"���� �ܰ�: {currentStepId}";
            Debug.Log($"UI�� ������Ʈ�Ǿ����ϴ�: {currentStepId}");
        }
    }
}
