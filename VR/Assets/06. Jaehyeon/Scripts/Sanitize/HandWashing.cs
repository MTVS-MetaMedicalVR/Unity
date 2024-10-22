using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWashing : BaseInteraction
{
    public float washingTime = 30f;  // �� �ı� Ÿ�̸�
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
            Debug.Log($"�� �ı� ���� ��... ���� �ð�: {timer:F1}��");

            if (timer <= 0)
            {
                Debug.Log("�� �ı� �Ϸ�!");
                PerformInteraction();  // ���� �Ϸ� �˸�
            }
        }
    }

    protected override void OnPlayerInteraction()
    {
        // �� �ı� �Ϸ� �� ó�� (�ʿ� �� ����)
    }
}

