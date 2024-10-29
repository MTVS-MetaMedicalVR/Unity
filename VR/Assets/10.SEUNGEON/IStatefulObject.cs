// ProcedureSystem/Core/Interfaces/IStatefulObject.cs
public interface IStatefulObject
{
	string ObjectId { get; }
	string CurrentState { get; }
	float StateProgress { get; }
	void SetState(string newState);
	bool IsInState(string state);
}