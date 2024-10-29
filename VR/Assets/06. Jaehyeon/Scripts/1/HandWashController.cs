using System.Collections;
using UnityEngine;

public class HandWashController : MonoBehaviour
{
    public Animator[] handAnimators;

    private bool isWashing = false;

    public void StartHandWash()
    {
        if (!isWashing)
        {
            isWashing = true;
            foreach (var animator in handAnimators)
            {
                animator.SetTrigger("Wash");
            }

            StartCoroutine(HandWashRoutine());
        }
    }

    private IEnumerator HandWashRoutine()
    {
        Debug.Log("�� �ı⸦ �����մϴ�.");
        yield return new WaitForSeconds(30);
        CompleteHandWash();
    }

    private void CompleteHandWash()
    {
        Debug.Log("�� �ı� ������ �Ϸ�Ǿ����ϴ�.");
        isWashing = false;
    }
}
