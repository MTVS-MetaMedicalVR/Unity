using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class ProcedureStep
{
    public string id;
    public string description;
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
    public MedicalProcedure currentProcedure;
    private int currentStepIndex;
    public UIManager uiManager;

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
    }

    public string GetCurrentStepId()
    {
        if (currentProcedure != null && currentStepIndex < currentProcedure.steps.Count)
        {
            return currentProcedure.steps[currentStepIndex].id;
        }
        return null;

    
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
            uiManager.ResetAllToggles();
            ExecuteCurrentStep();
        }
        else
        {
            Debug.LogError($"ID '{procedureId}'에 해당하는 절차를 찾을 수 없습니다.");
        }
    }

    public void ExecuteCurrentStep()
    {
        if (currentStepIndex < currentProcedure.steps.Count)
        {
            var step = currentProcedure.steps[currentStepIndex];
            Debug.Log($"현재 단계: {step.description}");

            // 단계 설명 업데이트 및 토글 활성화
            uiManager.UpdateStepDescription(step.description);
            uiManager.UpdateToggleState(step.id, true);
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
            Debug.Log($"'{stepId}' 단계가 완료되었습니다.");
            uiManager.UpdateToggleState(stepId, false);  // 해당 단계 비활성화

            currentStepIndex++;

            if (currentStepIndex < currentProcedure.steps.Count)
            {
                ExecuteCurrentStep();  // 다음 단계 실행
            }
            else
            {
                CompleteProcedure();
            }
        }
        else
        {
            Debug.LogError($"잘못된 단계: '{stepId}'가 현재 단계와 일치하지 않습니다.");
        }
    }

    private void CompleteProcedure()
    {
        Debug.Log($"{currentProcedure.name} 절차가 완료되었습니다.");
        uiManager.ResetAllToggles();
    }
}
