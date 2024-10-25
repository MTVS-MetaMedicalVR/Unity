using UnityEngine;

public class MoveToSink : MonoBehaviour
{
    public string stepId = "move_to_sink";
    public ProcedureManager procedureManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("개수대에 도착했습니다.");
            procedureManager.CompleteStep(stepId);
        }
    }
}
