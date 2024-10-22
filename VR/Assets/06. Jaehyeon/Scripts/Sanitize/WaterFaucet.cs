using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFaucet : BaseInteraction
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("���� Ʈ�� ���Դϴ�.");
            objectAnimator.SetTrigger("TurnOn");  // �� Ʈ�� �ִϸ��̼� ����
            PerformInteraction();  // ���� �Ϸ� �˸�
        }
    }

    protected override void OnPlayerInteraction()
    {
        // �߰� ���� �ʿ� �� �̰��� ����
    }
}

