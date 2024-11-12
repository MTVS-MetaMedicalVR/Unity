using System.Collections;
using UnityEngine;

public class HandWashController : MonoBehaviour
{
    public Animator[] handAnimators;
    public bool IsWashing { get; private set; } = false;

    private bool isReadyToWash = false;  // �� �ı� �غ� ����

    public void EnableHandWash()
    {
        isReadyToWash = true;
        Debug.Log("�� �ı� �غ� �Ϸ�.");
    }

    private void Update()
    {
        // �� �ı� �غ� �Ǿ� ������ �� �ı� ����
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
                animator.enabled = true; //�ִϸ����� Ȱ��ȭ
                Debug.Log("�վı� �ִϸ��̼� ���");
                animator.SetTrigger("Wash");
            }

            StartCoroutine(HandWashRoutine());
        }
    }

    private IEnumerator HandWashRoutine()
    {
        Debug.Log("�� �ı⸦ �����մϴ�.");
        yield return new WaitForSeconds(15);  // 15�� ���� �� �ı�
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

            Debug.Log("�� �ı� �ߴ�.");
            IsWashing = false;
        }
    }

    private void CompleteHandWash()
    {
        Debug.Log("�� �ı� ������ �Ϸ�Ǿ����ϴ�.");
        IsWashing = false;
    }
}
