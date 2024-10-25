using UnityEngine;

public class MoveToSink : MonoBehaviour
{
    public string stepId = "move_to_sink";
    public ProcedureManager procedureManager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"충돌한 오브젝트: {other.name}");  // 충돌한 오브젝트 이름 확인

        if (other.CompareTag("Player"))
        {
            Debug.Log("개수대에 도착했습니다.");
            if (procedureManager != null)
            {
                procedureManager.CompleteStep(stepId);
            }
            else
            {
                Debug.LogError("ProcedureManager가 연결되지 않았습니다.");
            }
        }
    }

}
