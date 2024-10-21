using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;  // UnityWebRequest ����� ���� �ʿ�

[System.Serializable]
public class ProcedureStep
{
    public string id;
    public string description;
    public string action;
    public string target;
    public float requiredProximity;
    public Interaction interaction;
}

[System.Serializable]
public class Interaction
{
    public string type;
    public string objectName;
    public string location;
}

[System.Serializable]
public class MedicalProcedure
{
    public string id;
    public string name;
    public List<ProcedureStep> steps;
}

[System.Serializable]
public class ProcedureData
{
    public List<MedicalProcedure> procedures;
}

public class ProcedureManager : MonoBehaviour
{
    public static ProcedureManager Instance;
    public List<MedicalProcedure> allProcedures;
    private MedicalProcedure currentProcedure;
    private int currentStepIndex;
    public UIManager uiManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);  // �ߺ� ����
    }

    private void Start()
    {
        StartCoroutine(LoadProcedures());  // �ڷ�ƾ���� JSON ���� �ε�
    }

    // **Android�� ��� �÷������� JSON ������ �д� �ڷ�ƾ**
    private IEnumerator LoadProcedures()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "procedures.json");

        UnityWebRequest request = UnityWebRequest.Get(path);  // JSON ���� ��û ����
        yield return request.SendWebRequest();  // ��û�� �Ϸ�� ������ ���

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;  // JSON ���� ��������
            ProcedureData data = JsonConvert.DeserializeObject<ProcedureData>(json);
            allProcedures = data.procedures;
            Debug.Log("������ ���������� �ε�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError($"JSON ������ �ε��� �� �����ϴ�: {request.error}");
        }
    }

    public void StartProcedure(string procedureId)
    {
        currentProcedure = allProcedures.Find(p => p.id == procedureId);
        if (currentProcedure != null)
        {
            currentStepIndex = 0;
            ExecuteCurrentStep();
        }
        else
        {
            Debug.LogError($"ID '{procedureId}'�� �ش��ϴ� ������ ã�� �� �����ϴ�.");
        }
    }

    private void ExecuteCurrentStep()
    {
        if (currentStepIndex < currentProcedure.steps.Count)
        {
            var step = currentProcedure.steps[currentStepIndex];
            Debug.Log($"���� �ܰ�: {step.description}");
            uiManager.UpdateToggleState(step.id, true);  // UI �ܰ� ǥ��
        }
        else
        {
            Debug.Log("��� ������ �Ϸ�Ǿ����ϴ�.");
            CompleteProcedure();
        }
    }

    public void CompleteStep(string stepId)
    {
        if (currentProcedure.steps[currentStepIndex].id == stepId)
        {
            Debug.Log($"'{stepId}' �ܰ谡 �Ϸ�Ǿ����ϴ�.");
            uiManager.UpdateToggleState(stepId, false);  // UI �ܰ� ��Ȱ��ȭ
            currentStepIndex++;
            ExecuteCurrentStep();  // ���� �ܰ� ����
        }
        else
        {
            Debug.LogError($"�߸��� �ܰ�: '{stepId}'�� ���� �ܰ�� ��ġ���� �ʽ��ϴ�.");
        }
    }

    private void CompleteProcedure()
    {
        Debug.Log($"{currentProcedure.name} ������ �Ϸ�Ǿ����ϴ�.");
        uiManager.ResetAllToggles();  // ��� UI �ʱ�ȭ
    }
}
