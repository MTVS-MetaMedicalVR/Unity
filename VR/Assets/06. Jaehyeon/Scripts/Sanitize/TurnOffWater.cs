using UnityEngine;

public class TurnOffWater : MonoBehaviour
{
    private Animator faucetAnimator;

    private void Awake()
    {
        faucetAnimator = GetComponent<Animator>();
        if (faucetAnimator == null)
        {
            Debug.LogError("수도꼭지에 Animator가 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("수도꼭지를 조작하여 물을 끕니다.");
            TurnOffWaterAction();  // 물 끄기 실행
        }
    }

    private void TurnOffWaterAction()
    {
        faucetAnimator?.SetTrigger("TurnOff");
        ProcedureManager.Instance.CompleteStep("turn_off_water");  // 절차 완료 알림
    }
}
