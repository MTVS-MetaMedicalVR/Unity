using UnityEngine;

public class SyringeInteraction : MonoBehaviour
{
    public Animator syringeAnimator; // 주사기 Animator
    public Transform syringeTip; // 주사기 끝 부분 (위치 감지용)
    public Transform patientMouth; // 환자 입 위치
    public float activationDistance = 0.1f; // 주사 활성화 거리

    private bool isInjected = false; // 주사가 완료되었는지 확인

    private void Update()
    {
        // 주사기가 환자 입 근처에 있고 주사가 실행되지 않았을 경우
        if (IsSyringeNearMouth() && !isInjected)
        {
            StartInjection();
        }
    }

    private bool IsSyringeNearMouth()
    {
        // 주사기 끝과 환자 입 사이의 거리 계산
        if (syringeTip != null && patientMouth != null)
        {
            float distance = Vector3.Distance(syringeTip.position, patientMouth.position);
            return distance < activationDistance; // 특정 거리 이내일 경우 true 반환
        }
        return false;
    }

    public void StartInjection()
    {
        if (!isInjected)
        {
            isInjected = true; // 주사 완료 상태 설정

            // 애니메이션 트리거 설정
            if (syringeAnimator != null)
            {
                syringeAnimator.SetTrigger("StartInjection");
            }

            Debug.Log("주사 애니메이션이 실행되었습니다.");
        }
    }
}
