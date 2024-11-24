using System.Collections;
using UnityEngine;

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // 바람 파티클
    public Transform handTransform;  // 손의 Transform (손 위치 감지)
    public float activationDistance = 0.1f;  // 손과 오브젝트 간의 활성화 거리
    public float dryingTime = 5.0f;  // 말리는 시간 설정
    public AudioSource dryingAudio;

    private bool isDrying = false;  // 손 말리기 상태 확인
    private bool canStartDrying = true;  // 말리기 가능 상태 확인

    private void Start()
    {
        // 초기 파티클 비활성화
        if (windParticle != null)
        {
            windParticle.gameObject.SetActive(false);
        }

        if (dryingAudio != null)
        {
            dryingAudio.Stop();
        }
    }

    private void Update()
    {
        // 손이 이 오브젝트에 가까워졌을 때 말리기 시작
        if (IsHandNearDryer() && !isDrying && canStartDrying)
        {
            StartDrying();
        }
    }

    private bool IsHandNearDryer()
    {
        // 손과 이 오브젝트 간의 거리를 계산
        if (handTransform != null)
        {
            float distance = Vector3.Distance(handTransform.position, transform.position);
            return distance < activationDistance;
        }
        return false;
    }

    public void StartDrying()
    {
        if (!isDrying)
        {
            Debug.Log("손을 말리기 시작합니다.");
            isDrying = true;
            canStartDrying = false;  // 말리기 중에 다시 시작하지 않도록 설정

            // 파티클 활성화 및 재생
            if (windParticle != null)
            {
                windParticle.gameObject.SetActive(true);
                windParticle.Play();
            }

            // 오디오 재생
            if (dryingAudio != null && !dryingAudio.isPlaying)
            {
                dryingAudio.Play();
            }

            // 말리기 타이머 시작
            StartCoroutine(DryingRoutine());
        }
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // 설정된 시간 동안 대기
        CompleteDrying();
        // 일정 시간 후에 다시 말리기를 허용
        yield return new WaitForSeconds(2.0f);  // 예시로 2초 대기 후 다시 말리기 가능
        canStartDrying = true;
    }

    private void CompleteDrying()
    {
        Debug.Log("손 말리기 완료.");
        if (windParticle != null)
        {
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }

        // 오디오 중지
        if (dryingAudio != null && dryingAudio.isPlaying)
        {
            dryingAudio.Stop();
        }

        isDrying = false;  // 상태 초기화
    }
}
