using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : BaseInteraction
{


    private void Update()
    {
        if (IsPlayerNearby(player, proximityThreshold))
        {
            Debug.Log("�����뿡 �����߽��ϴ�.");
            PerformInteraction();  // ���� �Ϸ� �˸�
        }
    }

    protected override void OnPlayerInteraction()
    {
        // Ư���� �߰� ���� ���� (�̵��� �ʿ�)
        Debug.Log("������ ���� ������ �Ϸ�Ǿ����ϴ�.");
    }
}

