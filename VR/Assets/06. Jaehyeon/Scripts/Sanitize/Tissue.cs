using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tissue : BaseInteraction
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("���� Ƽ���� �۰� �ֽ��ϴ�.");
            objectAnimator.SetTrigger("Dry");  // �� �۱� �ִϸ��̼� ����
            PerformInteraction();  // ���� �Ϸ� �˸�
        }
    }

    protected override void OnPlayerInteraction()
    {
        // Ƽ�� ��� �� �߰� ���� (�ʿ� �� ����)
    }
}
