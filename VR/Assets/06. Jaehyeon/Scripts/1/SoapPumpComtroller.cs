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
        if (foamParticle != null)
        {
            foamParticle.Stop();
            foamParticle.gameObject.SetActive(false);
        }
    }

    public void PumpSoap()
    {
        if (!isPumped)
        {
            pumpAnimator.SetTrigger("Pump");
            if (foamParticle != null)
            {
                foamParticle.gameObject.SetActive(true);
                foamParticle.Play();
            }
            isPumped = true;

            Debug.Log("비누를 펌프했습니다.");
        }
    }
}
