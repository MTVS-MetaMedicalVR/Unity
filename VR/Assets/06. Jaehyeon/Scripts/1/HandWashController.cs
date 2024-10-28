using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class HandWashController : MonoBehaviour
{
    public Animator[] handAnimator;  // 손 애니메이터
    //public ParticleSystem foamParticle;  // 거품 파티클

    private bool isWashing = false;  // 손 씻기 상태 확인

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

            //handAnimator[].SetTrigger("Wash");  // 손 씻기 애니메이션 실행

            /*if (foamParticle != null)
            {
                foamParticle.gameObject.SetActive(true);  // 파티클 활성화
                foamParticle.Play();  // 파티클 재생
            }*/

            StartCoroutine(HandWashRoutine());  // 손 씻기 타이머 시작
        }
    }

    private IEnumerator HandWashRoutine()
    {
        Debug.Log("손 씻기를 시작합니다.");
        yield return new WaitForSeconds(30);  // 30초 대기
        CompleteHandWash();
    }

    private void CompleteHandWash()
    {
        Debug.Log("손 씻기 절차가 완료되었습니다.");

        /*if (foamParticle != null)
        {
            foamParticle.Stop();  // 파티클 정지
            foamParticle.gameObject.SetActive(false);  // 파티클 비활성화
        }*/

        FindObjectOfType<ProcedureM>().CompleteStep();  // 다음 절차로 이동
        isWashing = false;
    }
}
