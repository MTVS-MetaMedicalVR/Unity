using UnityEngine;

public class HandWash : MonoBehaviour
{
    public string stepId = "hand_wash";  // �� �ı� ���� ID
    private bool isCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ �� �ı� ������ ������ �� ����
        if (other.CompareTag("Player") && !isCompleted)
        {
            Debug.Log("�� �ı� ������ ���۵Ǿ����ϴ�.");
            CompleteHandWash();

        }
    }

    private void CompleteHandWash()
    {
        isCompleted = true;
        Debug.Log($"{stepId} �ܰ谡 �Ϸ�Ǿ����ϴ�.");

        // ProcedureManager���� ���� �ܰ� ����
        ProcedureManager.Instance.CompleteStep(stepId);
    }
}
