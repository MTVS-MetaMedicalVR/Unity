using UnityEngine;

public class TurnOffWater : BaseInteraction
{
    private Animator faucetAnimator;  // ���������� �ִϸ��̼�

    protected override void Awake()
    {
        base.Awake();
        // ���������� Animator ������Ʈ ã��
        faucetAnimator = GetComponent<Animator>();
        if (faucetAnimator == null)
        {
            Debug.LogError("���������� Animator�� �����ϴ�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾��� ���� ���������� ������ ��
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("���������� �����Ͽ� ���� ���ϴ�.");
            TurnOffWaterAction();  // �� ���� ���� ����
        }
    }

    private void TurnOffWaterAction()
    {
        if (faucetAnimator != null)
        {
            faucetAnimator.SetTrigger("TurnOff");  // �� ���� �ִϸ��̼� ����
        }
        PerformInteraction();  // �ش� ���� �Ϸ� �˸�
    }

    protected override void OnPlayerInteraction()
    {
        // �߰� ��ȣ�ۿ��� �ʿ��� ��� �̰��� ����
    }
}
