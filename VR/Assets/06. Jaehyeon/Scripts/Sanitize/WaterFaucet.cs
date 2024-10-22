using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFaucet : BaseInteraction
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("물을 트는 중입니다.");
            objectAnimator.SetTrigger("TurnOn");  // 물 트는 애니메이션 실행
            PerformInteraction();  // 절차 완료 알림
        }
    }

    protected override void OnPlayerInteraction()
    {
        // 추가 동작 필요 시 이곳에 구현
    }
}

