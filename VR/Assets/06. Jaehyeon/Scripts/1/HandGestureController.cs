using System.Collections;
using UnityEngine;

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // 바람 파티클
    public Transform handTransform;  // 손의 Transform (손의 위치 감지)
    public Transform targetObject;  // 손을 가까이 가져갈 대상 오브젝트 (예: 핸드 드라이어)
    public float activationDistance = 0.1f;  // 손과 오브젝트 간의 활성화 거리

    private bool isDrying = false;  // 손 말리기 상태 확인
    private float dryingTime = 5.0f;  // 말리는 시간 설정

    private void Start()
    {
        if (windParticle != null)
        {
            windParticle.gameObject.SetActive(false);  // 초기 파티클 비활성화
        }
    }

    private void Update()
    {
        // 손이 대상 오브젝트에 가까이 왔을 때 파티클 활성화
        if (IsHandNearTarget() && !isDrying)
        {
            StartDrying();
        }
    }

    private bool IsHandNearTarget()
    {
        // 손과 대상 오브젝트 간의 거리를 계산하여 일정 거리 이내인지 확인
        if (handTransform != null && targetObject != null)
        {
            float distance = Vector3.Distance(handTransform.position, targetObject.position);
            return distance < activationDistance;
        }
        return false;
    }

    public void StartDrying()
    {
        if (windParticle != null)
        {
            isDrying = true;
            Debug.Log("손을 말리기 시작합니다.");

            // 파티클 활성화 및 재생
            windParticle.gameObject.SetActive(true);
            windParticle.Play();

            // 말리기 타이머 시작
            StartCoroutine(DryingRoutine());
        }
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // 설정한 시간 대기
        CompleteDrying();
    }

    private void CompleteDrying()
    {
        if (windParticle != null)
        {
            Debug.Log("손 말리기 완료.");

            // 파티클 정지 및 비활성화
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }
        isDrying = false;  // 상태 초기화
    }
}
