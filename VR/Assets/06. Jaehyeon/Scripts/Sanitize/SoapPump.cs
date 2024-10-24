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

            if (objectAnimator != null)
            {
                objectAnimator.SetTrigger("Pump");  // �ִϸ��̼� ����
            }
            else
            {
                Debug.LogError("Animator�� �������� �ʾҽ��ϴ�.");
            }

            PerformInteraction();  // ���� �Ϸ� �˸�
        }
    }

    protected override void OnPlayerInteraction()
    {
        // �� ��� �� �߰� ������ �����ϴ� ����
        Debug.Log("�� ��ȣ�ۿ��� �Ϸ�Ǿ����ϴ�.");
    }
}

