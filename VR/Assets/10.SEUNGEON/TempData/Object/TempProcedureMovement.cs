// TempProcedureMovement.cs - �̵� ���ν���
using UnityEngine;

public class TempProcedureMovement : TempProcedureObjectBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        CompleteInteraction();
    }
}
