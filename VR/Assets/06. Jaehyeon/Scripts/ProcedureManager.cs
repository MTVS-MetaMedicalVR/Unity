using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProcedureManager : MonoBehaviour
{
    public UIManager uiManager;
    public List<ProcedureStep> steps;
    public int currentStepIndex = 0;

    void Start()
    {
        LoadProcedureFromJson();
        StartProcedure();
    }

    public void LoadProcedureFromJson()
    {
        //streamingAsset에서 우선 Json을 넣어뒀기에 해당 코드로 사용. 경로는 추후 문제가 생겼을 시 수정. 
        string filePath = Application.streamingAssetsPath + "/procedure.json"; 
        //string filePath = Application.persistentDataPath + "/procedure.json";  // Json 파일 경로
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ProcedureData procedureData = JsonConvert.DeserializeObject<ProcedureData>(json);
            steps = procedureData.procedure.steps;
        }
        else
        {
            Debug.LogError("절차 데이터를 찾을 수 없습니다.");
        }
    }

    //절차 시작
    public void StartProcedure()
    {
        currentStepIndex = 0;
        TriggerNextStep();
        uiManager.ShowProcedurePanel();
        UpdateStep();

    }

    // 현재 단계 업데이트(UI)
    private void UpdateStep()
    {
        if (currentStepIndex < steps.Count)
        {
            ProcedureStep currentStep = steps[currentStepIndex];
            uiManager.UpdateStepText(currentStep.description); // 단계 설명 업데이트
        }
        else
        {
            CompleteProcedure();
        }
    }

    // 절차 완료(UI)
    private void CompleteProcedure()
    {
        uiManager.UpdateStepText("절차 완료!");
    }

    // 다음 단계로 이동(UI)
    public void AdvanceStep()
    {
        currentStepIndex++;
        UpdateStep();
    }


    public void TriggerNextStep()
    {
        if (currentStepIndex < steps.Count)
        {
            ProcedureStep step = steps[currentStepIndex];
            ExecuteStep(step);  // 각 단계를 실행
        }
        else
        {
            Debug.Log("모든 절차가 완료되었습니다!");
        }
    }

    private void ExecuteStep(ProcedureStep step)
    {
        Debug.Log("현재 단계: " + step.description);

        switch (step.action)
        {
            case "MoveToSink":
                // MoveToSink 클래스 실행
                MoveToSink();
                break;
            case "TurnOnWater":
                // TurnOnWater 클래스 실행
                TurnOnWater();
                break;
            case "UseSoap":
                // UseSoap 클래스 실행
                UseSoap();
                break;
            case "HandWashing":
                // HandWashing 클래스 실행
                HandWashing(step.duration);
                break;
            case "DryHands":
                // DryHands 클래스 실행
                DryHands();
                break;
            case "TurnOffWater":
                // TurnOffWater 클래스 실행
                TurnOffWater();
                break;
            default:
                Debug.LogError("알 수 없는 단계입니다.");
                break;
        }

        currentStepIndex++;
    }

    private void MoveToSink()
    {
        // 이동 로직 처리
        Debug.Log("개수대로 이동");
    }

    private void TurnOnWater()
    {
        // 물 트기 로직 처리
        Debug.Log("물을 트세요");
    }

    private void UseSoap()
    {
        // 비누 사용 로직 처리
        Debug.Log("비누를 펌프하세요");
    }

    private void HandWashing(float duration)
    {
        // 손 씻기 타이머 로직
        Debug.Log("손을 " + duration + "초 동안 씻으세요.");
    }

    private void DryHands()
    {
        // 손 닦기 로직 처리
        Debug.Log("손을 티슈로 닦으세요.");
    }

    private void TurnOffWater()
    {
        // 물 끄기 로직 처리
        Debug.Log("물을 끄세요.");
    }
}

// Json 데이터 구조
public class ProcedureData
{
    public Procedure procedure;
}

public class Procedure
{
    public string id;
    public string name;
    public List<ProcedureStep> steps;
}

public class ProcedureStep
{
    public string id;
    public string description;
    public string action;
    public string target;
    public float requiredProximity;
    public float duration;
}
