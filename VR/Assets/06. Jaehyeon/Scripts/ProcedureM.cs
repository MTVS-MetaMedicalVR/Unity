using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureM : MonoBehaviour
{
    public Text stepDescriptionText;  // UI�� ǥ���� �ܰ� ����
    public GameObject sink;  // ��ũ�� ������Ʈ
    public GameObject faucet;  // �������� ������Ʈ
    public GameObject soapPump;  // �� ���� ������Ʈ
    public GameObject tissue;  // Ƽ�� ������Ʈ
    public GameObject handModel;  // �� �� ������Ʈ
    public Transform player;  // �÷��̾��� Transform
    public FaucetController faucetController;  // FaucetController ����
    public SoapPumpController soapPumpController;
    public HandWashController handWashController;  // HandWashController ����
    public HandGestureController handGestureController;  // HandGestureController ����
    private int currentStep = 0;
    private bool stepInProgress = false;  // ���� �ܰ谡 ���� ������ Ȯ��


    private void Start()
    {
        StartProcedure();
    }

    public void StartProcedure()
    {
        currentStep = 0;
        stepInProgress = false;
        ExecuteStep();
    }

    public void ExecuteStep()
    {
        if (stepInProgress) return;  // ���� �ܰ谡 �Ϸ���� �ʾҴٸ� �ߺ� ���� ����

        stepInProgress = true;  // �ܰ� ���� ǥ��

        switch (currentStep)
        {
            case 0:
                UpdateStep("������� �̵��ϼ���.");
                break;
            case 1:
                UpdateStep("���� Ʈ����.");
                faucet.SetActive(true);
                faucetController.TurnOnWater();
                break;
            case 2:
                UpdateStep("�񴩸� �����ϼ���.");
                soapPump.SetActive(true);
                break;
            case 3:
                UpdateStep("���� 30�ʰ� ��������.");
                StartCoroutine(WashHands());
                break;
            case 4:
                UpdateStep("���� ��������.");
                handGestureController.enabled = true;  // �� ������ ��� Ȱ��ȭ
                break;
            case 5:
                UpdateStep("���� ������.");
                faucetController.RequestTurnOffWater();  // �� ���� ��û
                //faucet.SetActive(true);
                break;
            default:
                CompleteProcedure();
                break;
        }
    }

    private void UpdateStep(string description)
    {
        stepDescriptionText.text = description;
        Debug.Log($"�ܰ�: {description}");
    }

    private IEnumerator WashHands()
    {
        yield return new WaitForSeconds(30);  // 30�� ���
        CompleteStep();
    }

    public void CompleteStep()
    {
        if (!stepInProgress) return;  // �̹� �Ϸ�� �ܰ��� �ߺ� ȣ�� ����

        Debug.Log($"'{currentStep}' �ܰ谡 �Ϸ�Ǿ����ϴ�.");
        stepInProgress = false;  // �ܰ� �Ϸ� ���� ����
        currentStep++;
        Invoke("ExecuteStep", 1.0f);  // 1�� �� ���� �ܰ� ����
    }

    private void CompleteProcedure()
    {
        UpdateStep("�� �ı� ������ �Ϸ�Ǿ����ϴ�!");
        Debug.Log("��� ������ �Ϸ�Ǿ����ϴ�.");
    }

    private void Update()
    {
        if (currentStep == 0 && Vector3.Distance(player.position, sink.transform.position) < 1f)
        {
            CompleteStep();  // ��ũ�� ��ó�� �����ϸ� ���� �ܰ��
        }
    }
}
