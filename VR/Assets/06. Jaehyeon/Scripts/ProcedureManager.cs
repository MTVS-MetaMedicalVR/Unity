using System;
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
    private int currentStepIndex = 0;
    public UIManager uiManager;
    private bool isProcedureComplete = false;  // 절차 완료 여부
    private Vector3 cachedPosition;
    public Transform player;  // 플레이어의 Transform 객체


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
        cachedPosition = new Vector3(0, 0, 0);
        StartCoroutine(LoadProcedures());  // 절차 로드
    }

    private IEnumerator LoadProcedures()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "procedure.json");
        UnityWebRequest request = UnityWebRequest.Get(path);
        WaitForSeconds wait = new WaitForSeconds(1f);  // 객체 재사용
        yield return request.SendWebRequest();

        while(true)
        {
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

            yield return wait;
        }
        
    }

    private void Update()
    {
        transform.position = cachedPosition;  // 매 프레임마다 새 객체 생성 대신 캐싱된 값 사용
    }

    public void StartProcedure(string procedureId)
    {
        if (isProcedureComplete)
        {
            Debug.Log("절차가 이미 완료되었습니다. 다시 시작할 수 없습니다.");
            return;
        }

        currentProcedure = allProcedures.Find(p => p.id == procedureId);

        if (currentProcedure != null)
        {
            currentStepIndex = 0;
            uiManager.ResetAllToggles();
            ExecuteCurrentStep();
        }
        else
        {
            Debug.LogError($"'{procedureId}'에 해당하는 절차를 찾을 수 없습니다.");
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

    public void ExecuteCurrentStep()
    {
        if (isProcedureComplete) return;

        if (currentStepIndex < currentProcedure.steps.Count)
        {
            var step = currentProcedure.steps[currentStepIndex];
            Debug.Log($"현재 단계: {step.description}");

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
        if (isProcedureComplete) return;  // 절차 완료 시 무시

        if (currentProcedure.steps[currentStepIndex].id == stepId)
        {
            Debug.Log($"'{stepId}' 단계가 완료되었습니다.");
            uiManager.UpdateToggleState(stepId, false);
            currentStepIndex++;

            if (currentStepIndex < currentProcedure.steps.Count)
            {
                ExecuteCurrentStep();
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
        isProcedureComplete = true;  // 절차 완료 상태 설정
    }
}
