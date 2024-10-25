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

            if (objectAnimator != null)
            {
                objectAnimator.SetTrigger("Pump");  // 애니메이션 실행
            }
            else
            {
                Debug.LogError("Animator가 설정되지 않았습니다.");
            }

            PerformInteraction();  // 절차 완료 알림
        }
    }

    protected override void OnPlayerInteraction()
    {
        // 비누 사용 후 추가 동작을 구현하는 공간
        Debug.Log("비누 상호작용이 완료되었습니다.");
    }
}

