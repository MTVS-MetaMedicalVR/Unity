using UnityEngine;

public class MoveToSink : MonoBehaviour
{
    public string stepId = "move_to_sink";
    public ProcedureManager procedureManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�����뿡 �����߽��ϴ�.");
            procedureManager.CompleteStep(stepId);
        }
    }
}
