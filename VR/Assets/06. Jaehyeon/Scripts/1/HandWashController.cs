using System.Collections;
using UnityEngine;

public class HandWashController : MonoBehaviour
{
    public Animator[] handAnimators;
    public GameObject[] handObjects;  // 손 씻기 오브젝트를 참조할 배열 추가
    public bool IsWashing { get; private set; } = false;

    private void Start()
    {
        DisableAnimators();  // 시작할 때 모든 애니메이터와 손 오브젝트를 비활성화
        ResetAnimators();    // 트리거 초기화
    }

    public void EnableAnimators()
    {
        foreach (var animator in handAnimators)
        {
            animator.enabled = true;
        }

        // 손 오브젝트 활성화
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(true);
        }
    }

    public void DisableAnimators()
    {
        foreach (var animator in handAnimators)
        {
            animator.enabled = false;
        }

        // 손 오브젝트 비활성화
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(false);
        }
    }

    private void ResetAnimators()
    {
        // 모든 애니메이터의 트리거를 초기화
        foreach (var animator in handAnimators)
        {
            animator.ResetTrigger("Wash");
            animator.enabled = false;  // 시작 시 비활성화
        }
    }

    public void StartHandWash()
    {
        if (!IsWashing)
        {
            IsWashing = true;

            // 손 오브젝트 활성화
            foreach (var handObject in handObjects)
            {
                handObject.SetActive(true);
            }

            foreach (var animator in handAnimators)
            {
                animator.enabled = true;  // 애니메이터 활성화
                Debug.Log("손 씻기 애니메이션 재생");
                animator.SetTrigger("Wash");
            }

            StartCoroutine(HandWashRoutine());
        }
    }

    private IEnumerator HandWashRoutine()
    {
        Debug.Log("손 씻기를 시작합니다.");
        yield return new WaitForSeconds(10);  // 10초 동안 손 씻기
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

            // 손 오브젝트 비활성화
            foreach (var handObject in handObjects)
            {
                handObject.SetActive(false);
            }

            Debug.Log("손 씻기 중단.");
            IsWashing = false;
        }
    }

    private void CompleteHandWash()
    {
        Debug.Log("손 씻기 절차가 완료되었습니다.");
        IsWashing = false;

        foreach (var animator in handAnimators)
        {
            animator.ResetTrigger("Wash");
            animator.enabled = false;  // 애니메이터 비활성화
        }

        // 손 오브젝트 비활성화
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(false);
        }
    }
}
