
// ProcedureSystem/Conditions/ObjectUsageConditionChecker.cs
using Oculus.Interaction;

public class ObjectUsageConditionChecker : IConditionChecker
{
    private readonly IInteractable targetObject;
    private readonly float requiredUsageTime;
    private readonly string description;

    public ObjectUsageConditionChecker(IInteractable obj, float usageTime, string desc)
    {
        targetObject = obj;
        requiredUsageTime = usageTime;
        description = desc;
    }

    public bool CheckCondition()
    {
        return targetObject != null &&
               targetObject.IsInUse &&
               targetObject.CurrentUseTime >= requiredUsageTime;
    }

    public string GetDescription() => description;

    public void Reset()
    {
        targetObject?.ResetUseTime();
    }
}