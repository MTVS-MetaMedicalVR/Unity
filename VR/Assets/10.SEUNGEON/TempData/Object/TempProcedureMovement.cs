// TempProcedureMovement.cs - 이동 프로시저
using UnityEngine;

public class TempProcedureMovement : TempProcedureObjectBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        CompleteInteraction();
    }
}
