// 5. 각종 Procedure 오브젝트 클래스들
// ProcedureNormalObject.cs
using UnityEngine;

/// <summary>
/// 배치 및 트리거 감지형
/// </summary>
public class ProcedureNormalObject : ProcedureObjectBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand") || isDone) return;
        CompleteInteraction();
    }
}
