using UnityEngine;

public class TurnOffWater : BaseInteraction
{
    private Animator faucetAnimator;  // 수도꼭지의 애니메이션

    protected override void Awake()
    {
        base.Awake();
        // 수도꼭지의 Animator 컴포넌트 찾기
        faucetAnimator = GetComponent<Animator>();
        if (faucetAnimator == null)
        {
            Debug.LogError("수도꼭지에 Animator가 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어의 손이 수도꼭지와 접촉할 때
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("수도꼭지를 조작하여 물을 끕니다.");
            TurnOffWaterAction();  // 물 끄는 동작 수행
        }
    }

    private void TurnOffWaterAction()
    {
        if (faucetAnimator != null)
        {
            faucetAnimator.SetTrigger("TurnOff");  // 물 끄기 애니메이션 실행
        }
        PerformInteraction();  // 해당 절차 완료 알림
    }

    protected override void OnPlayerInteraction()
    {
        // 추가 상호작용이 필요한 경우 이곳에 구현
    }
}
