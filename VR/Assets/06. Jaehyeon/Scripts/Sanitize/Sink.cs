using UnityEngine;

public class MoveToSink : MonoBehaviour
{
    public string stepId = "move_to_sink";
    public ProcedureManager procedureManager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"�浹�� ������Ʈ: {other.name}");  // �浹�� ������Ʈ �̸� Ȯ��

        if (other.CompareTag("Player"))
        {
            Debug.Log("�����뿡 �����߽��ϴ�.");
            if (procedureManager != null)
            {
                procedureManager.CompleteStep(stepId);
            }
            else
            {
                Debug.LogError("ProcedureManager�� ������� �ʾҽ��ϴ�.");
            }
        }
    }

}
