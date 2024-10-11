using UnityEngine;

public class HandWashTraining : MonoBehaviour
{
    // �� ���� ���� �ܰ� ����
    public enum HandWashStage
    {
        MoveToSink,
        TurnOnWater,
        UseSoap,
        HandWashing,
        DryHandsWithTissue,
        TurnOffWater,
        Completed
    }

    // ���� �� ���� �ǽ� �ܰ�
    private HandWashStage currentStage;
    // �� ���� �ִϸ��̼�
    public Animator handWashAnimator;

    // �÷��̾�� ������ ��ġ Ȯ�ο�
    public Transform sinkTr;
    public Transform playerTr;
    private float sinkProximityThreshold = 1.5f; // ������ ��ó�� �̵��ߴٰ� ������ �Ÿ�

    // �� ���� Ÿ�̸� �� ���� üũ ����
    private float washingTimer = 30.0f; // �� �ı� Ÿ�̸� (30��)
    private bool isInWashingZone = false; // ���� �� �ı� ������ �ִ��� ����
    private bool warningDisplayed = false; // ��� UI�� ǥ�õǾ����� ����

    private void Start()
    {
        StartTraining(); // �ǽ� ����
    }

    // �� ���� �ǽ� ����
    public void StartTraining()
    {
        currentStage = HandWashStage.MoveToSink; // �ʱ� �ܰ� ����
        HandleStage(); // ù �ܰ� ����
    }

    private void Update()
    {
        // �� �ܰ迡 ���� ���� ó��
        if (currentStage == HandWashStage.MoveToSink)
        {
            CheckPlayerProximityToSink(); // ������ ��ó�� �̵� ���� Ȯ��
        }
        else if (currentStage == HandWashStage.HandWashing)
        {
            TrackHandWashing(); // �� �ı� Ÿ�̸� Ʈ��ŷ
        }
    }

    // �ܰ躰 �ȳ� �޽��� �� ����
    private void HandleStage()
    {
        switch (currentStage)
        {
            case HandWashStage.MoveToSink:
                Debug.Log("������� �̵��ϼ���.");
                break;

            case HandWashStage.TurnOnWater:
                Debug.Log("���� Ʈ����.");
                break;

            case HandWashStage.UseSoap:
                Debug.Log("�񴩸� �����ϼ���.");
                break;

            case HandWashStage.HandWashing:
                Debug.Log("���� 30�ʰ� ��������.");
                washingTimer = 30.0f; // �� �ı� Ÿ�̸� �ʱ�ȭ
                isInWashingZone = true;
                warningDisplayed = false;
                break;

            case HandWashStage.DryHandsWithTissue:
                Debug.Log("���� Ƽ���� ��������.");
                break;

            case HandWashStage.TurnOffWater:
                Debug.Log("���� ������.");
                break;

            case HandWashStage.Completed:
                CompleteHandWash(); // �ǽ� �Ϸ� ó��
                break;
        }
    }

    // ���� �ܰ�� �̵�
    private void AdvanceStage()
    {
        currentStage++;
        HandleStage(); // ���ο� �ܰ� ó��
    }

    // �÷��̾ ������ ��ó�� �ִ��� Ȯ��
    private void CheckPlayerProximityToSink()
    {
        float distance = Vector3.Distance(playerTr.position, sinkTr.position);
        if (distance <= sinkProximityThreshold)
        {
            Debug.Log("������ ��ó�� �̵� �Ϸ�");
            AdvanceStage(); // ���� �ܰ�� �̵�
        }
    }

    // �� �ı� Ÿ�̸� ����
    private void TrackHandWashing()
    {
        if (isInWashingZone) // ���� �ı� ���� �ȿ� ���� ��
        {
            washingTimer -= Time.deltaTime; // Ÿ�̸� ����
            Debug.Log("���� �ð�: " + washingTimer);

            if (washingTimer <= 0)
            {
                AdvanceStage(); // �� �ı� �Ϸ� �� ���� �ܰ�� �̵�
            }
        }
        else // ���� �ı� ���� �ۿ� ���� ��
        {
            if (!warningDisplayed)
            {
                ShowWarningUI(); // ��� UI ǥ��
                ResetHandWashing(); // Ÿ�̸� �ʱ�ȭ
                warningDisplayed = true;
            }
        }
    }

    // �� �ı� ������ �ִ��� Ȯ��
    private bool IsHandInWashingZone()
    {
        return isInWashingZone;
    }

    // �� �ı� �������� ��� ��� UI ǥ��
    private void ShowWarningUI()
    {
        Debug.Log("�� �ı� �������� ������ϴ�. �ٽ� �����ϼ���.");
    }

    // �� �ı� Ÿ�̸� �ʱ�ȭ
    private void ResetHandWashing()
    {
        washingTimer = 30.0f;
        Debug.Log("�� �ı� Ÿ�̸Ӱ� �ʱ�ȭ�Ǿ����ϴ�.");
    }

    // Ư�� �±� ������Ʈ�� ���� �浹�� ó���ϴ� Trigger �̺�Ʈ
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            // �� �ı� �ܰ迡�� ���� �ı� ������ ������ ��
            if (currentStage == HandWashStage.HandWashing)
            {
                isInWashingZone = true; // �ı� ���� ������ ���� �� true�� ����
                warningDisplayed = false;
                Debug.Log("���� �� �ı� ������ ���Խ��ϴ�.");
            }
            // �� Ʈ�� ��ȣ�ۿ�
            else if (currentStage == HandWashStage.TurnOnWater && other.gameObject.CompareTag("WaterFaucet"))
            {
                TurnOnWater();
            }
            // �� ���� ��ȣ�ۿ�
            else if (currentStage == HandWashStage.UseSoap && other.gameObject.CompareTag("SoapPump"))
            {
                PumpSoap();
            }
            // �� �۱� ��ȣ�ۿ�
            else if (currentStage == HandWashStage.DryHandsWithTissue && other.gameObject.CompareTag("Tissue"))
            {
                DryHands();
            }
            // �� ���� ��ȣ�ۿ�
            else if (currentStage == HandWashStage.TurnOffWater && other.gameObject.CompareTag("WaterFaucet"))
            {
                TurnOffWater();
            }
        }
    }

    // �� �ı� �������� ���� ��� �� ȣ��
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand") && currentStage == HandWashStage.HandWashing)
        {
            isInWashingZone = false; // ���� �ı� ������ ��� �� false�� ����
            Debug.Log("���� �� �ı� �������� ������ϴ�.");
        }
    }

    // �� Ʈ�� �ִϸ��̼� ����
    private void TurnOnWater()
    {
        Debug.Log("Water is now running.");
        handWashAnimator.SetTrigger("TurnOnWater");
        AdvanceStage(); // ���� �ܰ�� �̵�
    }

    // �� ���� �ִϸ��̼� ����
    private void PumpSoap()
    {
        Debug.Log("Soap dispensed.");
        handWashAnimator.SetTrigger("PumpSoap");
        AdvanceStage(); // ���� �ܰ�� �̵�
    }

    // �� �۱� �ִϸ��̼� ����
    private void DryHands()
    {
        Debug.Log("Hands dried with tissue.");
        handWashAnimator.SetTrigger("DryHands");
        AdvanceStage(); // ���� �ܰ�� �̵�
    }

    // �� ���� �ִϸ��̼� ����
    private void TurnOffWater()
    {
        Debug.Log("Water is now turned off.");
        handWashAnimator.SetTrigger("TurnOffWater");
        AdvanceStage(); // ���� �ܰ�� �̵�
    }

    // �� ���� �ǽ� �Ϸ� ó��
    private void CompleteHandWash()
    {
        Debug.Log("�� ���� �ǽ��� �Ϸ��߽��ϴ�.");
    }
}
