// ProcedureMovement.cs
using UnityEngine;

public class ProcedureMovement : ProcedureObjectBase
{
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player") || isDone) return;
		CompleteInteraction();
	}
}