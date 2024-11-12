// 5. 각종 Procedure 오브젝트 클래스들
// ProcedureNormalObject.cs
using UnityEngine;

public class ProcedureNormalObject : ProcedureObjectBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand") || isDone) return;
        CompleteInteraction();
    }
}
