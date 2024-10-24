using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoapPump : BaseInteraction
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("비누 펌프 사용 중입니다.");
            objectAnimator.SetTrigger("Pump");  // 비누 펌프 애니메이션 실행
            PerformInteraction();  // 절차 완료 알림
        }
    }

    protected override void OnPlayerInteraction()
    {
        // 비누 사용 후 추가 동작 필요 시 이곳에 구현
    }
}

