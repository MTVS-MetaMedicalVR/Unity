using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // 물 파티클
    private Animator animator;  // 애니메이터
    private bool isWaterRunning = false;  // 물 상태 확인
    private bool isWaterTurnedOn = false;  // 물이 한 번 켜졌는지 여부

    public Transform handTransform;  // 사용자의 손 Transform
    public Collider faucetHandleCollider;  // 수도꼭지의 Collider
    public ProcedureM procedureM;  // ProcedureManager 참조


    private void Start()
    {
        animator = GetComponent<Animator>();

        // 초기 파티클 비활성화
        waterParticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 손이 핸들을 잡고 있고 물이 아직 트여 있지 않다면 물을 틈
        if (IsHandGrabbingHandle() && !isWaterTurnedOn)
        {
            TurnOnWater();
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
            animator.SetBool("SinkON", true);  // 애니메이션 실행
            waterParticle.gameObject.SetActive(true);  // 파티클 활성화
            waterParticle.Play();  // 파티클 재생
            isWaterRunning = true;
            isWaterTurnedOn = true;  // 물이 한 번 켜진 상태로 유지
            Debug.Log("물을 트세요.");

            procedureM.CompleteStep();
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

    // 외부에서 물 끄기 요청을 받을 수 있도록 메서드 추가
    public void RequestTurnOffWater()
    {
        if (isWaterTurnedOn)
        {
            TurnOffWater();
            isWaterTurnedOn = false;  // 물 상태 초기화
        }
    }
}
