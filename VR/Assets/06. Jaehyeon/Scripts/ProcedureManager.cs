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
        //streamingAsset���� �켱 Json�� �־�ױ⿡ �ش� �ڵ�� ���. ��δ� ���� ������ ������ �� ����. 
        string filePath = Application.streamingAssetsPath + "/procedure.json"; 
        //string filePath = Application.persistentDataPath + "/procedure.json";  // Json ���� ���
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ProcedureData procedureData = JsonConvert.DeserializeObject<ProcedureData>(json);
            steps = procedureData.procedure.steps;
        }
        else
        {
            Debug.LogError("���� �����͸� ã�� �� �����ϴ�.");
        }
    }

    //���� ����
    public void StartProcedure()
    {
        currentStepIndex = 0;
        TriggerNextStep();
        uiManager.ShowProcedurePanel();
        UpdateStep();

    }

    // ���� �ܰ� ������Ʈ(UI)
    private void UpdateStep()
    {
        if (currentStepIndex < steps.Count)
        {
            ProcedureStep currentStep = steps[currentStepIndex];
            uiManager.UpdateStepText(currentStep.description); // �ܰ� ���� ������Ʈ
        }
        else
        {
            CompleteProcedure();
        }
    }

    // ���� �Ϸ�(UI)
    private void CompleteProcedure()
    {
        uiManager.UpdateStepText("���� �Ϸ�!");
    }

    // ���� �ܰ�� �̵�(UI)
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
            ExecuteStep(step);  // �� �ܰ踦 ����
        }
        else
        {
            Debug.Log("��� ������ �Ϸ�Ǿ����ϴ�!");
        }
    }

    private void ExecuteStep(ProcedureStep step)
    {
        Debug.Log("���� �ܰ�: " + step.description);

        switch (step.action)
        {
            case "MoveToSink":
                // MoveToSink Ŭ���� ����
                MoveToSink();
                break;
            case "TurnOnWater":
                // TurnOnWater Ŭ���� ����
                TurnOnWater();
                break;
            case "UseSoap":
                // UseSoap Ŭ���� ����
                UseSoap();
                break;
            case "HandWashing":
                // HandWashing Ŭ���� ����
                HandWashing(step.duration);
                break;
            case "DryHands":
                // DryHands Ŭ���� ����
                DryHands();
                break;
            case "TurnOffWater":
                // TurnOffWater Ŭ���� ����
                TurnOffWater();
                break;
            default:
                Debug.LogError("�� �� ���� �ܰ��Դϴ�.");
                break;
        }

        currentStepIndex++;
    }

    private void MoveToSink()
    {
        // �̵� ���� ó��
        Debug.Log("������� �̵�");
    }

    private void TurnOnWater()
    {
        // �� Ʈ�� ���� ó��
        Debug.Log("���� Ʈ����");
    }

    private void UseSoap()
    {
        // �� ��� ���� ó��
        Debug.Log("�񴩸� �����ϼ���");
    }

    private void HandWashing(float duration)
    {
        // �� �ı� Ÿ�̸� ����
        Debug.Log("���� " + duration + "�� ���� ��������.");
    }

    private void DryHands()
    {
        // �� �۱� ���� ó��
        Debug.Log("���� Ƽ���� ��������.");
    }

    private void TurnOffWater()
    {
        // �� ���� ���� ó��
        Debug.Log("���� ������.");
    }
}

// Json ������ ����
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
