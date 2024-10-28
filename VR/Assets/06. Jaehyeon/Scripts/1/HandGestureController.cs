using System.Collections;
using UnityEngine;
using Oculus.Interaction;

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // 바람 파티클
    public OVRHand leftHand;  // 왼손 OVRHand
    public OVRHand rightHand;  // 오른손 OVRHand

    private bool isDrying = false;  // 손 말리기 상태 확인
    private float dryingTime = 5.0f;  // 말리는 시간 설정

    private void Start()
    {
        if (windParticle != null)
        {
            windParticle.gameObject.SetActive(false);  // 파티클 초기 비활성화
        }
    }

    private void Update()
    {
        // 두 손이 모였는지 확인하고 말리기 시작
        if (AreHandsTogether() && !isDrying)
        {
            StartDrying();
        }
    }

    private bool AreHandsTogether()
    {
        if (leftHand != null && rightHand != null)
        {
            // 손 위치를 계산하여 거리가 일정 이하이면 true 반환
            Vector3 leftHandPosition = leftHand.transform.position;
            Vector3 rightHandPosition = rightHand.transform.position;
            float distance = Vector3.Distance(leftHandPosition, rightHandPosition);
            return distance < 0.1f;  // 손이 가까이 모인 경우
        }
        return false;
    }

    public void StartDrying()
    {
        if (windParticle != null)
        {
            isDrying = true;
            Debug.Log("손을 말리기 시작합니다.");
            windParticle.gameObject.SetActive(true);  // 파티클 활성화
            windParticle.Play();  // 파티클 재생

            StartCoroutine(DryingRoutine());
        }
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // 설정한 시간 동안 대기
        CompleteDrying();
    }

    private void CompleteDrying()
    {
        if (windParticle != null)
        {
            Debug.Log("손 말리기 완료.");
            windParticle.Stop();  // 파티클 정지
            windParticle.gameObject.SetActive(false);  // 파티클 비활성화
        }
        isDrying = false;  // 상태 초기화
    }
}
