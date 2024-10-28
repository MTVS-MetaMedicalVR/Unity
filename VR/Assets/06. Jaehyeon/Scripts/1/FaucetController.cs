using Oculus.Interaction;
using System.Collections;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // 물 파티클
    private Animator animator;  // 애니메이터
    private bool isWaterRunning = false;  // 물 상태 확인
    private bool isWaterTurnedOn = false;  // 물이 켜진 상태 유지

    public OVRGrabbable faucetHandle;  // OVRGrabbable 핸들 참조

    private void Start()
    {
        animator = GetComponent<Animator>();

        // 파티클 초기 비활성화
        if (waterParticle != null)
        {
            waterParticle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // 핸들이 잡혔고 물이 켜지지 않았다면 물을 틈
        if (faucetHandle.isGrabbed && !isWaterTurnedOn)
        {
            TurnOnWater();
        }

        // 핸들에서 손을 떼면 물을 끔
        if (!faucetHandle.isGrabbed && isWaterTurnedOn)
        {
            TurnOffWater();
        }
    }

    public void TurnOnWater()
    {
        if (!isWaterRunning && animator != null && waterParticle != null)
        {
            animator.SetBool("SinkON", true);  // 애니메이션 시작
            waterParticle.gameObject.SetActive(true);  // 파티클 활성화
            waterParticle.Play();  // 파티클 재생
            isWaterRunning = true;
            isWaterTurnedOn = true;  // 물이 켜진 상태 유지

            Debug.Log("물을 틀었습니다.");
        }
    }

    public void TurnOffWater()
    {
        if (isWaterRunning && animator != null && waterParticle != null)
        {
            animator.SetBool("SinkON", false);  // 애니메이션 종료
            waterParticle.Stop();  // 파티클 정지
            waterParticle.gameObject.SetActive(false);  // 파티클 비활성화
            isWaterRunning = false;
            isWaterTurnedOn = false;  // 상태 초기화

            Debug.Log("물을 껐습니다.");
        }
    }

    public void RequestTurnOffWater()
    {
        TurnOffWater();
        Debug.Log("외부에서 물 끄기 요청을 처리했습니다.");
    }

}
