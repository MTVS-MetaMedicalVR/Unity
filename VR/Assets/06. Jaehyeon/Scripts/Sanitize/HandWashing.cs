using UnityEngine;

public class HandWashing : MonoBehaviour
{
    public float washingTime = 30f;
    private float timer;

    private void Start()
    {
        timer = washingTime;
    }

    private void Update()
    {
        if (Vector3.Distance(ProcedureManager.Instance.player.position, transform.position) < 1.5f)
        {
            timer -= Time.deltaTime;
            Debug.Log($"손 씻기 진행 중... 남은 시간: {timer:F1}초");

            if (timer <= 0)
            {
                Debug.Log("손 씻기 완료!");
                ProcedureManager.Instance.CompleteStep("hand_washing");  // 절차 완료 알림
            }
        }
    }
}
