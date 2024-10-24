using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoapPump : BaseInteraction
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("�� ���� ��� ���Դϴ�.");
            objectAnimator.SetTrigger("Pump");  // �� ���� �ִϸ��̼� ����
            PerformInteraction();  // ���� �Ϸ� �˸�
        }
    }

    protected override void OnPlayerInteraction()
    {
        // �� ��� �� �߰� ���� �ʿ� �� �̰��� ����
    }
}

