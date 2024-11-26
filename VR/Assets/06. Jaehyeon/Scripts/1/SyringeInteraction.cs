using UnityEngine;

public class SyringeInteraction : MonoBehaviour
{
    public Animator syringeAnimator; // 주사기 Animator 연결
    private bool hasInjected = false; // 주사가 이미 실행되었는지 확인

    private void OnTriggerEnter(Collider other)
    {
        // "Gum" 태그를 가진 오브젝트와 충돌 확인
        if (other.CompareTag("Gum") && !hasInjected)
        {
            hasInjected = true; // 주사 실행 플래그 설정
            syringeAnimator.SetTrigger("StartInjesction"); // 애니메이션 실행
            Debug.Log("주사기 애니메이션 실행");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 필요 시 TriggerExit 처리
        if (other.CompareTag("Gum"))
        {
            Debug.Log("주사기가 입에서 떨어짐");
        }
    }
}
