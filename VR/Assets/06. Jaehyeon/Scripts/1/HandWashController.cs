using System.Collections;
using UnityEngine;

public class HandWashController : MonoBehaviour
{
    public Animator[] handAnimators;
    public bool IsWashing { get; private set; } = false;

    private bool isReadyToWash = false;  // 손 씻기 준비 상태

    public void EnableHandWash()
    {
        isReadyToWash = true;
        Debug.Log("손 씻기 준비 완료.");
    }

    private void Update()
    {
        // 손 씻기 준비가 되어 있으면 손 씻기 시작
        if (isReadyToWash && !IsWashing)
        {
            StartHandWash();
        }
    }

    public void StartHandWash()
    {
        if (!IsWashing)
        {
            IsWashing = true;
            foreach (var animator in handAnimators)
            {
                animator.enabled = true; //애니메이터 활성화
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
