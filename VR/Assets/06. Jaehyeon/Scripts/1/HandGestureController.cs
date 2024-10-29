using System.Collections;
using UnityEngine;
using Oculus.Interaction;

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // 바람 파티클
    public Transform leftHandTransform;  // 왼손 Transform
    public Transform rightHandTransform;  // 오른손 Transform
    public float activationDistance = 0.05f;  // 두 손이 가까이 모이는 거리

    private bool isDrying = false;  // 손 말리기 상태 확인
    private float dryingTime = 5.0f;  // 손 말리기 시간

    private void Start()
    {
        if (windParticle != null)
        {
            windParticle.gameObject.SetActive(false);  // 파티클 초기 비활성화
        }
    }

    private void Update()
    {
        // 손이 가까이 모였을 때 손 말리기 시작
        if (AreHandsTogether() && !isDrying)
        {
            StartDrying();
        }
    }

    private bool AreHandsTogether()
    {
        if (leftHandTransform != null && rightHandTransform != null)
        {
            // 두 손의 거리를 계산하여 일정 거리 이하면 true 반환
            float distance = Vector3.Distance(leftHandTransform.position, rightHandTransform.position);
            return distance < activationDistance;
        }
        return false;
    }

    public void StartDrying()
    {
        if (windParticle != null && !isDrying)
        {
            isDrying = true;
            Debug.Log("손을 말리기 시작합니다.");

            // 바람 파티클 활성화 및 재생
            windParticle.gameObject.SetActive(true);
            windParticle.Play();

            // 일정 시간 후 파티클 정지
            StartCoroutine(DryingRoutine());
        }
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // 손 말리기 시간 대기
        CompleteDrying();
    }

    private void CompleteDrying()
    {
        if (windParticle != null)
        {
            Debug.Log("손 말리기 완료.");

            // 바람 파티클 정지 및 비활성화
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }
        isDrying = false;  // 상태 초기화
    }
}
