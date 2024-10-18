using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;  // UnityWebRequest 사용을 위해 필요

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
            Destroy(gameObject);  // 중복 방지
    }

    private void Start()
    {
        StartCoroutine(LoadProcedures());  // 코루틴으로 JSON 파일 로드
    }

    // **Android와 모든 플랫폼에서 JSON 파일을 읽는 코루틴**
    private IEnumerator LoadProcedures()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "procedures.json");

        UnityWebRequest request = UnityWebRequest.Get(path);  // JSON 파일 요청 생성
        yield return request.SendWebRequest();  // 요청이 완료될 때까지 대기

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;  // JSON 내용 가져오기
            ProcedureData data = JsonConvert.DeserializeObject<ProcedureData>(json);
            allProcedures = data.procedures;
            Debug.Log("절차가 성공적으로 로드되었습니다.");
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

    private void ExecuteCurrentStep()
    {
        if (currentStepIndex < currentProcedure.steps.Count)
        {
            var step = currentProcedure.steps[currentStepIndex];
            Debug.Log($"현재 단계: {step.description}");
            uiManager.UpdateToggleState(step.id, true);  // UI 단계 표시
        }
        else
        {
            Debug.Log("모든 절차가 완료되었습니다.");
            CompleteProcedure();
        }
    }

    public void CompleteStep(string stepId)
    {
        if (currentProcedure.steps[currentStepIndex].id == stepId)
        {
            Debug.Log($"'{stepId}' 단계가 완료되었습니다.");
            uiManager.UpdateToggleState(stepId, false);  // UI 단계 비활성화
            currentStepIndex++;
            ExecuteCurrentStep();  // 다음 단계 실행
        }
        else
        {
            Debug.LogError($"잘못된 단계: '{stepId}'가 현재 단계와 일치하지 않습니다.");
        }
    }

    private void CompleteProcedure()
    {
        Debug.Log($"{currentProcedure.name} 절차가 완료되었습니다.");
        uiManager.ResetAllToggles();  // 모든 UI 초기화
    }
}
