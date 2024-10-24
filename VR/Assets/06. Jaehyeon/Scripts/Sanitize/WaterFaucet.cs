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
        else
        {
            Debug.LogWarning("PlayerHand �±׸� ���� ��ü�� �ƴմϴ�.");
        }
    }

    protected override void OnPlayerInteraction()
    {
        // �߰� ���� �ʿ� �� �̰��� ����
    }
}

