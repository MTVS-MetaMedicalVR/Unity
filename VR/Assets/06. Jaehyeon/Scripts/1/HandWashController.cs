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
        Debug.Log("손 씻기를 시작합니다.");
        yield return new WaitForSeconds(30);
        CompleteHandWash();
    }

    private void CompleteHandWash()
    {
        Debug.Log("손 씻기 절차가 완료되었습니다.");
        isWashing = false;
    }
}
