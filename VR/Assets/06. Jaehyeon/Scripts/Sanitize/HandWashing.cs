using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWashing : BaseInteraction
{
    public float washingTime = 30f;  // 손 씻기 타이머
    private float timer;

    private void Start()
    {
        timer = washingTime;
    }

    private void Update()
    {
        if (IsPlayerNearby(player, 1.5f) && timer > 0)
        {
            timer -= Time.deltaTime;
            Debug.Log($"손 씻기 진행 중... 남은 시간: {timer:F1}초");

            if (timer <= 0)
            {
                Debug.Log("손 씻기 완료!");
                PerformInteraction();  // 절차 완료 알림
            }
        }
    }

    protected override void OnPlayerInteraction()
    {
        // 손 씻기 완료 후 처리 (필요 시 구현)
    }
}

