using UnityEngine;

public class TurnOffWater : MonoBehaviour
{
    private Animator faucetAnimator;

    private void Awake()
    {
        faucetAnimator = GetComponent<Animator>();
        if (faucetAnimator == null)
        {
            Debug.LogError("���������� Animator�� �����ϴ�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("���������� �����Ͽ� ���� ���ϴ�.");
            TurnOffWaterAction();  // �� ���� ����
        }
    }

    private void TurnOffWaterAction()
    {
        faucetAnimator?.SetTrigger("TurnOff");
        ProcedureManager.Instance.CompleteStep("turn_off_water");  // ���� �Ϸ� �˸�
    }
}
