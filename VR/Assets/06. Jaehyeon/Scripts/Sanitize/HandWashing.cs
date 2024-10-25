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
            Debug.Log($"�� �ı� ���� ��... ���� �ð�: {timer:F1}��");

            if (timer <= 0)
            {
                Debug.Log("�� �ı� �Ϸ�!");
                ProcedureManager.Instance.CompleteStep("hand_washing");  // ���� �Ϸ� �˸�
            }
        }
    }
}
