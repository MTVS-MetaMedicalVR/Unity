using System.Collections;
using UnityEngine;
using Oculus.Interaction;  // Oculus Hand Gesture 사용

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // 바람 파티클
    public OVRHand leftHand;  // 왼손 OVRHand
    public OVRHand rightHand;  // 오른손 OVRHand

    private bool isDrying = false;  // 손 말리기 상태 확인
    private float dryingTime = 5.0f;  // 손 말리는 시간 (초)

    private void Start()
    {
        // 초기에는 파티클 비활성화
        windParticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 두 손이 모였는지 확인 (손 제스처: 손바닥 간 거리 측정)
        if (AreHandsTogether() && !isDrying)
        {
            StartDrying();  // 손 말리기 시작
        }
    }

    private bool AreHandsTogether()
    {
        // 왼손과 오른손의 손바닥 위치를 가져와 거리 계산
        Vector3 leftHandPosition = leftHand.transform.position;
        Vector3 rightHandPosition = rightHand.transform.position;

        float distance = Vector3.Distance(leftHandPosition, rightHandPosition);

        // 두 손의 거리가 일정 이하이면 손이 모인 것으로 간주
        return distance < 0.1f;
    }

    private void StartDrying()
    {
        isDrying = true;
        Debug.Log("손을 말리기 시작합니다.");

        // 바람 파티클 활성화 및 재생
        windParticle.gameObject.SetActive(true);
        windParticle.Play();

        // 손 말리기 타이머 시작
        StartCoroutine(DryingRoutine());
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // 설정한 시간 동안 대기
        CompleteDrying();
    }

    private void CompleteDrying()
    {
        Debug.Log("손 말리기 절차가 완료되었습니다.");

        // 바람 파티클 정지 및 비활성화
        windParticle.Stop();
        windParticle.gameObject.SetActive(false);

        // 다음 절차로 이동
        FindObjectOfType<ProcedureM>().CompleteStep();
        isDrying = false;
    }
}
