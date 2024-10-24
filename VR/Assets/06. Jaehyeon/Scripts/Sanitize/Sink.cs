using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : BaseInteraction
{


    private void Update()
    {
        if (IsPlayerNearby(player, proximityThreshold))
        {
            Debug.Log("개수대에 도착했습니다.");
            PerformInteraction();  // 절차 완료 알림
        }
    }

    protected override void OnPlayerInteraction()
    {
        // 특별한 추가 동작 없음 (이동만 필요)
        Debug.Log("개수대 도착 절차가 완료되었습니다.");
    }
}

