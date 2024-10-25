using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;  // UnityWebRequest 사용을 위해 필요

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
        public Interaction interaction;  // Interaction 사용
    }

    [System.Serializable]
    public class Interaction
    {
        public string type;      // 상호작용 유형 (예: grab, move)
        public string objectName; // 상호작용할 오브젝트 이름
        public string location;  // 상호작용 위치
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

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
                Debug.LogError("UIManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    private void Start()
    {
        StartCoroutine(LoadProcedures());
    }

    private IEnumerator LoadProcedures()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "procedure.json");
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            ProcedureData data = JsonConvert.DeserializeObject<ProcedureData>(json);
            allProcedures = data.procedures;
            StartProcedure(allProcedures[0].id);
        }
        else
        {
            Debug.LogError($"JSON 파일을 로드할 수 없습니다: {request.error}");
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
            Debug.LogError($"ID '{procedureId}'에 해당하는 절차를 찾을 수 없습니다.");
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
            currentStepIndex++;
            ExecuteCurrentStep();
        }
        else
        {
            Debug.LogError($"잘못된 단계: '{stepId}'가 현재 단계와 일치하지 않습니다.");
        }
    }

    private void CompleteProcedure()
    {
        Debug.Log($"{currentProcedure.name} 절차가 완료되었습니다.");
    }
}
