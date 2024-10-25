using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public ProcedureManager procedureManager;  
    public Text stepDescriptionText;  // �ܰ� ���� �ؽ�Ʈ
    public List<GameObject> stepObjects;  // ��� �ܰ躰 GameObject

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
                Debug.LogError("ProcedureManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

    private void Start()
    {
        InitializeStepObjects();  // ��� �ܰ� ��ü �ʱ�ȭ
        CenterUI();  // UI�� �߾ӿ� ��ġ
        procedureManager.StartProcedure("hand_wash");  // ���� ����
    }

    // �ܰ躰 GameObject �ʱ�ȭ
    private void InitializeStepObjects()
    {
        foreach (var stepObject in stepObjects)
        {
            Button button = stepObject.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnStepObjectClicked(stepObject));
            }
            else
            {
                Debug.LogError($"{stepObject.name}�� Button ������Ʈ�� �����ϴ�.");
            }

            stepObject.SetActive(false);  // ��� �ܰ� ��Ȱ��ȭ
        }
    }

    // �ܰ躰 GameObject Ŭ�� �� ȣ��Ǵ� �޼���
    private void OnStepObjectClicked(GameObject stepObject)
    {
        Debug.Log($"'{stepObject.name}'�� Ŭ���Ǿ����ϴ�.");  // Ŭ�� �̺�Ʈ Ȯ��
        string stepId = stepObject.name.ToLower();
        Debug.Log($"{stepId} �ܰ谡 �Ϸ�Ǿ����ϴ�.");

        if (procedureManager != null && procedureManager.GetCurrentStepId() == stepId)
        {
            procedureManager.CompleteStep(stepId);  // �ܰ� �Ϸ�
            stepObject.SetActive(false);  // �ܰ� ��Ȱ��ȭ
        }
        else
        {
            Debug.LogError($"{stepId} �ܰ谡 ���� �ܰ�� ��ġ���� �ʽ��ϴ�.");
        }
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
            Debug.LogError("�ܰ� ���� �ؽ�Ʈ�� �����ϴ�.");
        }
    }

    public void CenterUI()
    {
        Vector3 uiPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;
        transform.position = uiPosition;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        Debug.Log("UI�� ȭ�� �߾ӿ� ��ġ�Ǿ����ϴ�.");
    }

    // Ư�� �ܰ� GameObject Ȱ��ȭ/��Ȱ��ȭ
    public void SetStepObjectActive(string stepId, bool state)
    {
        var stepObject = stepObjects.Find(obj => obj.name.ToLower() == stepId);
        if (stepObject != null)
        {
            stepObject.SetActive(state);
            Debug.Log($"{stepId} �ܰ谡 {(state ? "Ȱ��ȭ" : "��Ȱ��ȭ")}�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError($"{stepId}�� �ش��ϴ� GameObject�� ã�� �� �����ϴ�.");
        }
    }
}
