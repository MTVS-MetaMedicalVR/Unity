using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // 물 파티클
    private Animator animator;  // 수도꼭지 애니메이터
    public AudioSource waterAudio; //물 소리 오디오 소스

    private bool isWaterRunning = false;  // 물 상태 확인
    private bool isWaterTurnedOn = false;  // 물이 한 번 켜졌는지 여부

    public Transform handTransform;  // 손의 Transform (손 위치 감지)
    public float activationDistance = 0.1f;  // 수도꼭지를 작동시킬 거리
    public float waterDuration = 30.0f;  // 물이 켜진 상태 지속 시간 (초)

    // 손 씻기 컨트롤러 참조 (이 부분은 제거 가능)
    // public HandWashController handWashController;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (waterParticle != null)
        {
            // 초기 파티클 비활성화
            waterParticle.gameObject.SetActive(false);
            //파티클 초기화 
            waterParticle.Stop();
        }


        // 초기 손 씻기 애니메이션 비활성화 (필요시 유지)
        // if (handWashController != null)
        // {
        //     handWashController.DisableAnimators();
        // }

        if (waterAudio != null)
        {
            waterAudio.Stop();
        }

    }

    private void Update()
    {
        // 손이 수도꼭지에 가까워졌을 때 물을 틂
        if (IsHandNearFaucet() && !isWaterTurnedOn)
        {
            TurnOnWater();
        }
    }

    private bool IsHandNearFaucet()
    {
        // 손과 수도꼭지 간의 거리를 계산
        float distance = Vector3.Distance(handTransform.position, transform.position);
        return distance < activationDistance;
    }

    public void TurnOnWater()
    {
        if (!isWaterRunning)
        {
            animator.SetBool("SinkON", true);  // 애니메이션 실행
            waterParticle.gameObject.SetActive(true);  // 파티클 활성화
            waterParticle.Play();  // 파티클 재생
            isWaterRunning = true;
            isWaterTurnedOn = true;  // 물이 켜진 상태 유지

            Debug.Log("물을 틀었습니다.");

            // 물 파티클 재생
            if (waterParticle != null)
            {
                waterParticle.gameObject.SetActive(true);
                waterParticle.Play();

                // 물 파티클이 재생될 때 소리 시작
                if (waterAudio != null && !waterAudio.isPlaying)
                {
                    waterAudio.Play();
                }
            }

            // 30초 후에 자동으로 물을 끔

            StartCoroutine(WaterDurationRoutine());
        }
    }

    private IEnumerator WaterDurationRoutine()
    {
        yield return new WaitForSeconds(waterDuration);  // 30초 대기
        RequestTurnOffWater();  // 물 끄기 요청
    }

    public void RequestTurnOffWater()
    {
        if (isWaterRunning)
        {
            if (animator != null)
            {
                animator.SetBool("SinkON", false);  // 애니메이션 종료
            }

            if (waterParticle != null)
            {
                waterParticle.Stop();  // 파티클 정지
                waterParticle.gameObject.SetActive(false);  // 파티클 비활성화
            }


            // 물 소리 중지
            if (waterAudio != null && waterAudio.isPlaying)
            {
                waterAudio.Stop();
            }


            isWaterRunning = false;

            Debug.Log("물을 껐습니다.");

            // 손 씻기 애니메이터 비활성화 (제거 또는 필요에 따라 유지)
            // if (handWashController != null && handWashController.IsWashing)
            // {
            //     handWashController.StopHandWash();
            // }
        }
    }
}