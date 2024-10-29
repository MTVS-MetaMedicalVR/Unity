// ProcedureSystem/Core/Interfaces/IInteractable.cs
public interface IInteractable
{
    string ObjectId { get; }
    bool IsInUse { get; }
    float CurrentUseTime { get; }
    void StartUse();
    void StopUse();
    void ResetUseTime();
}
