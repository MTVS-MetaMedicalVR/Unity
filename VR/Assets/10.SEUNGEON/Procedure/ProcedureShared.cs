// ProcedureShared.cs
using System.Collections;
using UnityEngine;

public class ProcedureShared : ProcedureObjectBase
{
    [SerializeField]
    private string[] validProcedureIds;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand") || isDone) return;

        string currentProcedureId = InGameProcedureManager.Instance.GetCurrentProcedureId();
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