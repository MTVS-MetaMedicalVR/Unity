// 5. ���� Procedure ������Ʈ Ŭ������
// ProcedureNormalObject.cs
using UnityEngine;

/// <summary>
/// ��ġ �� Ʈ���� ������
/// </summary>
public class ProcedureNormalObject : ProcedureObjectBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand") || isDone) return;
        CompleteInteraction();
    }
}
