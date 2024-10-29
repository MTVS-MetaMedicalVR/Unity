

// ProcedureSystem/Data/ProcedureStep.cs
[System.Serializable]
public class ProcedureStep
{
    public int id;
    public string title;
    public string description;
    public ProcedureCondition[] conditions;
    public string nextStepId;
    public ProcedureUIEvent uiEvent;
    public UnityEngine.Events.UnityEvent onStepStart;
    public UnityEngine.Events.UnityEvent onStepComplete;
}

// ProcedureSystem/Data/ProcedureCondition.cs
[System.Serializable]
public class ProcedureCondition
{
    public string id;
    public string conditionType;
    public string[] parameters;
    public float holdDuration;
    public string description;
}