using System.Collections;
using UnityEngine;

public class SoapPumpController : MonoBehaviour
{
    public Animator pumpAnimator;  // 비누 펌프 애니메이터
    public ParticleSystem foamParticle;  // 거품 파티클
    public Transform handTransform;  // 손의 Transform (손 위치 감지)
    public float activationDistance = 0.1f;  // 비누 펌프 활성화 거리

    public HandWashController handWashController;  // 손 씻기 컨트롤러
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
        // 손이 펌프 근처에 왔을 때 비누 펌프 동작 실행
        if (IsHandNearPump() && !isPumped)
        {
            PumpSoap();
        }
    }

    private bool IsHandNearPump()
    {
        // 손과 펌프 오브젝트 사이의 거리 계산
        if (handTransform != null)
        {
            float distance = Vector3.Distance(handTransform.position, transform.position);
            return distance < activationDistance;  // 특정 거리 이내일 때 true 반환
        }
        return false;
    }

    public void PumpSoap()
    {
        if (!isPumped)
        {
            isPumped = true;  // 펌프 완료 상태 설정

            // 애니메이션 실행
            if (pumpAnimator != null)
            {
                pumpAnimator.SetTrigger("Pump");
            }

            // 파티클 재생
            if (foamParticle != null)
            {
                foamParticle.gameObject.SetActive(true);
                foamParticle.Play();
                StartCoroutine(StopFoamParticleAfterDelay(3f));  // 3초 후에 파티클 중지
            }

            // 손 씻기 애니메이션 시작
            if (handWashController != null && !handWashController.IsWashing)
            {
                handWashController.StartHandWash();
            }

            Debug.Log("비누를 펌프했습니다.");
        }
    }
    private IEnumerator StopFoamParticleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (foamParticle != null)
        {
            foamParticle.Stop();
            foamParticle.gameObject.SetActive(false);
        }
    }
}
