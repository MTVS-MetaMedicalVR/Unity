// TempProcedureShared.cs - 공유 프로시저 (수도꼭지 등)
using System.Collections;
using UnityEngine;

public class TempProcedureShared : TempProcedureObjectBase
{
    [SerializeField]
    private string[] validProcedureIds; // 이 오브젝트가 반응할 수 있는 프로시저 ID 목록

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand")) return;

        string currentProcedureId = TempProcedureManager.Instance.GetCurrentProcedureId();

        if (System.Array.Exists(validProcedureIds, id => id == currentProcedureId))
        {
            if (animator && !string.IsNullOrEmpty(interactionConfig.animationTriggerName))
            {
                animator.SetTrigger(interactionConfig.animationTriggerName);
                StartCoroutine(WaitForAnimationComplete());
            }
            else
            {
                CompleteInteraction();
            }
        }
    }

    private IEnumerator WaitForAnimationComplete()
    {
        yield return new WaitForSeconds(0.1f);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        CompleteInteraction();
    }
}