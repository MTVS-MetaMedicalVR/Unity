using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tissue : BaseInteraction
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("손을 티슈로 닦고 있습니다.");
            objectAnimator.SetTrigger("Dry");  // 손 닦기 애니메이션 실행
            PerformInteraction();  // 절차 완료 알림
        }
    }

    protected override void OnPlayerInteraction()
    {
        // 티슈 사용 후 추가 동작 (필요 시 구현)
    }
}
