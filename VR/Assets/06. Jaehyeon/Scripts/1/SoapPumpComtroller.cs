using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoapPumpController : MonoBehaviour
{
    public Animator pumpAnimator;  // 비누 펌프 애니메이터
    public ParticleSystem foamParticle;  // 거품 파티클
    public Transform handTransform;  // 손의 Transform (손 위치 감지)
    public float activationDistance = 0.1f;  // 비누 펌프 활성화 거리

    private bool isPumped = false;  // 비누 펌프 상태 확인

    private void Start()
    {
        // 파티클 초기 비활성화
        if (foamParticle != null)
        {
            foamParticle.Stop();
            foamParticle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // 손이 펌프 근처에 왔을 때 펌프 동작 실행
        if (IsHandNearPump() && !isPumped)
        {
            PumpSoap();
        }
    }

    private bool IsHandNearPump()
    {
        // 손과 비누 펌프 간의 거리를 계산
        float distance = Vector3.Distance(handTransform.position, transform.position);
        return distance < activationDistance;
    }

    public void PumpSoap()
    {
        if (!isPumped)
        {
            pumpAnimator.SetTrigger("Pump");  // 애니메이션 실행

            if (foamParticle != null)
            {
                foamParticle.gameObject.SetActive(true);  // 파티클 활성화
                foamParticle.Play();  // 파티클 재생
            }

            isPumped = true;  // 펌프 완료 상태 설정
            Debug.Log("비누를 펌프했습니다.");
        }
    }
}
