using UnityEngine;

public class Tissue : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("손을 티슈로 닦고 있습니다.");
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Dry");  // 손 닦기 애니메이션 실행
            }

            ProcedureManager.Instance.CompleteStep("dry_hands");  // 절차 완료 알림
        }
    }
}
