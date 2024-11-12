using System.Collections;
using UnityEngine;

public class HandWashController : MonoBehaviour
{
    public Animator[] handAnimators;
    public bool IsWashing { get; private set; } = false;

    public void StartHandWash()
    {
        if (!IsWashing)
        {
            IsWashing = true;
            foreach (var animator in handAnimators)
            {
                animator.enabled = true;  // 애니메이터 활성화
                Debug.Log("손씻기 애니메이션 재생");
                animator.SetTrigger("Wash");
            }

            StartCoroutine(HandWashRoutine());
        }
    }

    private IEnumerator HandWashRoutine()
    {
        Debug.Log("손 씻기를 시작합니다.");
        yield return new WaitForSeconds(15);  // 15초 동안 손 씻기
        CompleteHandWash();
    }

    public void StopHandWash()
    {
        if (IsWashing)
        {
            foreach (var animator in handAnimators)
            {
                animator.ResetTrigger("Wash");
                animator.enabled = false;  // 애니메이터 비활성화
            }

            Debug.Log("손 씻기 중단.");
            IsWashing = false;
        }
    }

    private void CompleteHandWash()
    {
        Debug.Log("손 씻기 절차가 완료되었습니다.");
        IsWashing = false;
    }
}
