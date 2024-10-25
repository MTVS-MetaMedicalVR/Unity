using UnityEngine;

public class HandWash : MonoBehaviour
{
    public string stepId = "hand_wash";  // 손 씻기 절차 ID
    private bool isCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 손 씻기 영역에 들어왔을 때 실행
        if (other.CompareTag("Player") && !isCompleted)
        {
            Debug.Log("손 씻기 절차가 시작되었습니다.");
            CompleteHandWash();

        }
    }

    private void CompleteHandWash()
    {
        isCompleted = true;
        Debug.Log($"{stepId} 단계가 완료되었습니다.");

        // ProcedureManager에서 다음 단계 실행
        ProcedureManager.Instance.CompleteStep(stepId);
    }
}
