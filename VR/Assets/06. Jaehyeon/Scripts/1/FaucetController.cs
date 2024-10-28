using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // 물 파티클
    private Animator animator;  // 애니메이터
    private bool isWaterRunning = false;  // 물 상태 확인

    public Transform handTransform;  // 사용자의 손 Transform
    public Collider faucetHandleCollider;  // 수도꼭지의 Collider

    private void Start()
    {
        animator = GetComponent<Animator>();

        // 초기 파티클 비활성화
        waterParticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 손이 수도꼭지와 충돌했을 때 물을 트는 동작 실행
        if (IsHandGrabbingHandle() && !isWaterRunning)
        {
            TurnOnWater();
        }
        else if (!IsHandGrabbingHandle() && isWaterRunning)
        {
            TurnOffWater();
        }
    }

    private bool IsHandGrabbingHandle()
    {
        // 손과 수도꼭지 핸들의 충돌을 감지
        return faucetHandleCollider.bounds.Contains(handTransform.position);
    }

    public void TurnOnWater()
    {
        if (!isWaterRunning)
        {
            animator.SetBool("SinkON",true);  // 애니메이션 실행
            waterParticle.gameObject.SetActive(true);  // 파티클 활성화
            waterParticle.Play();  // 파티클 재생
            isWaterRunning = true;
            Debug.Log("물을 트세요.");
        }
    }

    public void TurnOffWater()
    {
        if (isWaterRunning)
        {
            animator.SetBool("SinkON", false);  // 애니메이션 실행
            waterParticle.Stop();  // 파티클 정지
            waterParticle.gameObject.SetActive(false);  // 파티클 비활성화
            isWaterRunning = false;
            Debug.Log("물을 껐습니다.");
        }
    }
}

