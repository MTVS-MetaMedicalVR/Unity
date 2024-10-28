using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoapPumpController : MonoBehaviour
{
    public Animator pumpAnimator;  // 비누 펌프 애니메이터
    public ParticleSystem foamParticle;  // 거품 파티클
    private bool isPumped = false;  // 비누 펌프 상태 확인

    private void Start()
    {
        // 파티클 초기 비활성화
        foamParticle.gameObject.SetActive(false);
    }

    public void PumpSoap()
    {
        if (!isPumped)
        {
            pumpAnimator.SetTrigger("Pump");  // 펌프 애니메이션 실행
            foamParticle.gameObject.SetActive(true);  // 파티클 활성화
            foamParticle.Play();  // 파티클 재생
            isPumped = true;

            Debug.Log("비누를 펌프했습니다.");
            FindObjectOfType<ProcedureM>().CompleteStep();  // 다음 절차로 이동
        }
    }
}
