
// ProcedureSystem/Conditions/ConditionCheckerFactory.cs
public class ConditionCheckerFactory
{
    private readonly IObjectService objectService;
    private readonly ProceduralObject proceduralObject;

    public ConditionCheckerFactory(IObjectService objectService, ProceduralObject proceduralObject)
    {
        this.objectService = objectService;
        this.proceduralObject = proceduralObject;
    }

    public IConditionChecker CreateChecker(ProcedureCondition condition)
    {
        switch (condition.conditionType)
        {
            case "ObjectState":
                var objectId = condition.parameters[0];
                var requiredState = condition.parameters[1];
                var requiredProgress = float.Parse(condition.parameters[2]);
                var statefulObject = objectService.GetStatefulObject(objectId);
                return new ObjectStateConditionChecker(statefulObject, requiredState, requiredProgress, condition.description);

            case "ObjectUsage":
                var targetObjectId = condition.parameters[0];
                var usageTime = float.Parse(condition.parameters[1]);
                var interactableObject = objectService.GetInteractable(targetObjectId);
                return new ObjectUsageConditionChecker(interactableObject, usageTime, condition.description);

            default:
                throw new System.ArgumentException($"Unknown condition type: {condition.conditionType}");
        }
    }
}