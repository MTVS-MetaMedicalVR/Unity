
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

        // 손 닿으면 애니메이션 실행
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(true);
        }
        foreach (var animator in handAnimators)
        {
            animator.enabled = true;
            animator.SetTrigger("Wash");
        }

        // 애니메이션 종료 후 작업 완료 호출
        StartCoroutine(WaitForAnimationComplete());
    }

    private IEnumerator WaitForAnimationComplete()
    {
        float maxDuration = 0f;

        // 각 애니메이션의 길이를 확인하여 가장 긴 애니메이션 시간 추출
        foreach (var animator in handAnimators)
        {
            var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                maxDuration = Mathf.Max(maxDuration, clipInfo[0].clip.length);
            }
        }

        // 애니메이션 시간만큼 대기
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
