// ProcedureSystem/Conditions/ObjectStateConditionChecker.cs
public class ObjectStateConditionChecker : IConditionChecker
{
    private readonly IStatefulObject targetObject;
    private readonly string requiredState;
    private readonly float requiredProgress;
    private readonly string description;

    public ObjectStateConditionChecker(IStatefulObject obj, string state, float progress, string desc)
    {
        targetObject = obj;
        requiredState = state;
        requiredProgress = progress;
        description = desc;
    }

    public bool CheckCondition()
    {
        return targetObject != null &&
               targetObject.IsInState(requiredState) &&
               targetObject.StateProgress >= requiredProgress;
    }

    public string GetDescription() => description;
    public void Reset() { }
}