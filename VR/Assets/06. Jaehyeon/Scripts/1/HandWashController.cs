using System.Collections;
using UnityEngine;

public class HandWashController : MonoBehaviour
{
    public Animator[] handAnimators;
    public GameObject[] handObjects;  // �� �ı� ������Ʈ�� ������ �迭 �߰�
    public bool IsWashing { get; private set; } = false;

    private void Start()
    {
        DisableAnimators();  // ������ �� ��� �ִϸ����Ϳ� �� ������Ʈ�� ��Ȱ��ȭ
        ResetAnimators();    // Ʈ���� �ʱ�ȭ
    }

    public void EnableAnimators()
    {
        foreach (var animator in handAnimators)
        {
            animator.enabled = true;
        }

        // �� ������Ʈ Ȱ��ȭ
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

        // �� ������Ʈ ��Ȱ��ȭ
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(false);
        }
    }

    private void ResetAnimators()
    {
        // ��� �ִϸ������� Ʈ���Ÿ� �ʱ�ȭ
        foreach (var animator in handAnimators)
        {
            animator.ResetTrigger("Wash");
            animator.enabled = false;  // ���� �� ��Ȱ��ȭ
        }
    }

    public void StartHandWash()
    {
        if (!IsWashing)
        {
            IsWashing = true;

            // �� ������Ʈ Ȱ��ȭ
            foreach (var handObject in handObjects)
            {
                handObject.SetActive(true);
            }

            foreach (var animator in handAnimators)
            {
                animator.enabled = true;  // �ִϸ����� Ȱ��ȭ
                Debug.Log("�� �ı� �ִϸ��̼� ���");
                animator.SetTrigger("Wash");
            }

            StartCoroutine(HandWashRoutine());
        }
    }

    private IEnumerator HandWashRoutine()
    {
        Debug.Log("�� �ı⸦ �����մϴ�.");
        yield return new WaitForSeconds(10);  // 10�� ���� �� �ı�
        CompleteHandWash();
    }

    public void StopHandWash()
    {
        if (IsWashing)
        {
            foreach (var animator in handAnimators)
            {
                animator.ResetTrigger("Wash");
                animator.enabled = false;  // �ִϸ����� ��Ȱ��ȭ
            }

            // �� ������Ʈ ��Ȱ��ȭ
            foreach (var handObject in handObjects)
            {
                handObject.SetActive(false);
            }

            Debug.Log("�� �ı� �ߴ�.");
            IsWashing = false;
        }
    }

    private void CompleteHandWash()
    {
        Debug.Log("�� �ı� ������ �Ϸ�Ǿ����ϴ�.");
        IsWashing = false;

        foreach (var animator in handAnimators)
        {
            animator.ResetTrigger("Wash");
            animator.enabled = false;  // �ִϸ����� ��Ȱ��ȭ
        }

        // �� ������Ʈ ��Ȱ��ȭ
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(false);
        }
    }
}
