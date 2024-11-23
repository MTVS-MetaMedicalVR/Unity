
// HandWashObject.cs
using System.Collections;
using UnityEngine;

public class HandWashObject : ProcedureObjectBase
{
    [SerializeField] private Animator[] handAnimators;
    [SerializeField] private GameObject[] handObjects;

    public override void Initialize()
    {
        base.Initialize();
        foreach (var animator in handAnimators)
        {
            animator.enabled = false;
            animator.ResetTrigger("Wash");
        }
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(false);
        }
    }

    protected override void HandleInteraction()
    {
        if (isDone) return;

        foreach (var handObject in handObjects)
        {
            handObject.SetActive(true);
        }
        foreach (var animator in handAnimators)
        {
            animator.enabled = true;
            animator.SetTrigger("Wash");
        }
        StartCoroutine(WaitForAnimationComplete());
    }

    private IEnumerator WaitForAnimationComplete()
    {
        float maxDuration = 0f;
        foreach (var animator in handAnimators)
        {
            var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                maxDuration = Mathf.Max(maxDuration, clipInfo[0].clip.length);
            }
        }
        yield return new WaitForSeconds(maxDuration);
        CompleteInteraction();
    }

    protected override void CompleteInteraction()
    {
        foreach (var animator in handAnimators)
        {
            animator.enabled = false;
            animator.ResetTrigger("Wash");
        }
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(false);
        }
        base.CompleteInteraction();
    }
}

