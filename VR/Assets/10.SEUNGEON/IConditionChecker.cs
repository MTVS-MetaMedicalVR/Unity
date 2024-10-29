// ProcedureSystem/Conditions/IConditionChecker.cs
public interface IConditionChecker
{
    bool CheckCondition();
    string GetDescription();
    void Reset();
}
