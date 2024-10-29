// ProcedureSystem/Core/Events/ProcedureEvents.cs
public class ProcedureEvents
{
	public class StepStarted
	{
		public ProcedureStep Step { get; }
		public StepStarted(ProcedureStep step) => Step = step;
	}

	public class StepCompleted
	{
		public ProcedureStep Step { get; }
		public float CompletionTime { get; }
		public StepCompleted(ProcedureStep step, float time)
		{
			Step = step;
			CompletionTime = time;
		}
	}

	public class ProcedureCompleted
	{
		public float TotalTime { get; }
		public ProcedureCompleted(float time) => TotalTime = time;
	}
}