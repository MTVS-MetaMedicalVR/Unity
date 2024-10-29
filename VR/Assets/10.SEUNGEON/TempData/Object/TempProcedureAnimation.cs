
// TempProcedureAnimation.cs - 애니메이션 프로시저
using System.Collections;
using UnityEngine;

public class TempProcedureAnimation : TempProcedureObjectBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand") || isDone) return;

        if (animator && !string.IsNullOrEmpty(interactionConfig.animationTriggerName))
        {
            animator.SetTrigger(interactionConfig.animationTriggerName);
            StartCoroutine(WaitForAnimationComplete());
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