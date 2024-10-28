using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class HandWashController : MonoBehaviour
{
    public Animator[] handAnimator;  // �� �ִϸ�����
    //public ParticleSystem foamParticle;  // ��ǰ ��ƼŬ

    private bool isWashing = false;  // �� �ı� ���� Ȯ��

    public void StartHandWash()
    {
        if (!isWashing)
        {
            isWashing = true;
            foreach (var animator in handAnimator)
            {
                if (animator != null)
                {
                    animator.SetTrigger("Wash");
                }
            }

            //handAnimator[].SetTrigger("Wash");  // �� �ı� �ִϸ��̼� ����

            /*if (foamParticle != null)
            {
                foamParticle.gameObject.SetActive(true);  // ��ƼŬ Ȱ��ȭ
                foamParticle.Play();  // ��ƼŬ ���
            }*/

            StartCoroutine(HandWashRoutine());  // �� �ı� Ÿ�̸� ����
        }
    }

    private IEnumerator HandWashRoutine()
    {
        Debug.Log("�� �ı⸦ �����մϴ�.");
        yield return new WaitForSeconds(30);  // 30�� ���
        CompleteHandWash();
    }

    private void CompleteHandWash()
    {
        Debug.Log("�� �ı� ������ �Ϸ�Ǿ����ϴ�.");

        /*if (foamParticle != null)
        {
            foamParticle.Stop();  // ��ƼŬ ����
            foamParticle.gameObject.SetActive(false);  // ��ƼŬ ��Ȱ��ȭ
        }*/

        FindObjectOfType<ProcedureM>().CompleteStep();  // ���� ������ �̵�
        isWashing = false;
    }
}
