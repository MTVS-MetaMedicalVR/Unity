using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class ProcedureManager : MonoBehaviour
{
    public static ProcedureManager Instance;
    public List<MedicalProcedure> allProcedures;
    public MedicalProcedure currentProcedure;
    private int currentStepIndex;
    public UIManager uiManager;
    public Transform Player;

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

    [System.Serializable]
    public class ProcedureStep
    {
        public string id;
        public string description;
        public string action;
        public string target;
        public float requiredProximity;
        public Interaction interaction;  // Interaction ���
    }

    [System.Serializable]
    public class Interaction
    {
        public string type;      // ��ȣ�ۿ� ���� (��: grab, move)
        public string objectName; // ��ȣ�ۿ��� ������Ʈ �̸�
        public string location;  // ��ȣ�ۿ� ��ġ
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        uiManager ??= FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

    private void Start()
    {
        StartCoroutine(LoadProcedures());
    }

    private IEnumerator LoadProcedures()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "procedure.json");
        UnityWebRequest request = UnityWebRequest.Get(path);  // using ���� ����
        yield return request.SendWebRequest();  // ��û�� ���� �� ���� ���

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            var data = JsonConvert.DeserializeObject<ProcedureData>(json);
            allProcedures = data.procedures;
            StartProcedure(allProcedures[0].id);
        }
        else
        {
            Debug.LogError($"JSON ������ �ε��� �� �����ϴ�: {request.error}");
        }

        request.Dispose();  // ��û ����
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

    public string GetCurrentStepId()
    {
        if (currentProcedure != null && currentStepIndex < currentProcedure.steps.Count)
            return currentProcedure.steps[currentStepIndex].id;
        return null;
    }

    public void ExecuteCurrentStep()
    {
        if (currentStepIndex < currentProcedure.steps.Count)
        {
            var step = currentProcedure.steps[currentStepIndex];
            uiManager.UpdateStepDescription(step.description);
            uiManager.SetStepObjectActive(step.id, true);
        }
        else
        {
            CompleteProcedure();
        }
    }

    public void CompleteStep(string stepId)
    {
        if (currentProcedure.steps[currentStepIndex].id == stepId)
        {
            uiManager.SetStepObjectActive(stepId, false);
            currentStepIndex++;  // �ε��� ������ ���� ����

            if (currentStepIndex < currentProcedure.steps.Count)
            {
                ExecuteCurrentStep();  // ���� �ܰ� ����
            }
            else
            {
                CompleteProcedure();  // ���� �Ϸ� ó��
            }
        }
        else
        {
            Debug.LogError($"�߸��� �ܰ�: '{stepId}'�� ���� �ܰ�� ��ġ���� �ʽ��ϴ�.");
        }
    }

    private void CompleteProcedure()
    {
        Debug.Log($"{currentProcedure.name} ������ �Ϸ�Ǿ����ϴ�.");
    }
}
