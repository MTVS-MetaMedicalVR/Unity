using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureM : MonoBehaviour
{
    public Text stepDescriptionText;  // UI�� ǥ���� �ܰ� ����
    public GameObject sink;  // ��ũ�� ������Ʈ
    public GameObject faucet;  // �������� ������Ʈ
    public GameObject soapPump;  // �� ���� ������Ʈ
    public GameObject tissue;  // Ƽ�� ������Ʈ
    public Transform player;  // �÷��̾��� Transform

    public FaucetController faucetController;  // FaucetController ����
    public SoapPumpController soapPumpController;
    public HandWashController handWashController;  // HandWashController ����
    public HandGestureController handGestureController;  // HandGestureController ����

    private void Start()
    {
        DisplayMessage("VR ȯ�濡 ���� ���� ȯ���մϴ�!");
    }

    private void Update()
    {
        // �÷��̾ ��ũ�� ��ó�� �������� ��
        if (Vector3.Distance(player.position, sink.transform.position) < 0.3f)
        {
            DisplayMessage("��ũ�뿡 �����߽��ϴ�.");
        }
    }

    public void DisplayMessage(string message)
    {
        stepDescriptionText.text = message;
        Debug.Log(message);
    }

    public void TriggerFaucet()
    {
        // �������� ������� �� ��ƼŬ �ѱ�
        DisplayMessage("���� Ʈ����.");
        faucetController.TurnOnWater();
    }

    public void TriggerSoapPump()
    {
        // �� ���� ���
        DisplayMessage("�񴩸� �����ϼ���.");
        soapPumpController.PumpSoap();
    }

    public void TriggerHandWash()
    {
        // �� �ı� 30�� ����
        DisplayMessage("���� 30�� ���� ��������.");
        handWashController.StartHandWash();
    }

    public void TriggerHandDry()
    {
        // �� ������ ����
        DisplayMessage("���� ��������.");
        handGestureController.StartDrying();
    }

    public void TurnOffFaucet()
    {
        // �� ����
        DisplayMessage("���� ������.");
        faucetController.RequestTurnOffWater();
    }
}
