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
    public Transform player;  // �÷��̾��� Transform
    public FaucetController faucetController;  // FaucetController ����

    private int currentStep = 0;

    private void Start()
    {
        StartProcedure();
    }

    public void StartProcedure()
    {
        currentStep = 0;
        ExecuteStep();
    }

    public void ExecuteStep()
    {
        switch (currentStep)
        {
            case 0:
                UpdateStep("������� �̵��ϼ���.");
                break;
            case 1:
                UpdateStep("���� Ʈ����.");
                faucet.SetActive(true);
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
                UpdateStep("���� Ƽ���� ��������.");
                tissue.SetActive(true);
                break;
            case 5:
                UpdateStep("���� ������.");
                faucet.SetActive(true);
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
        Debug.Log($"'{currentStep}' �ܰ谡 �Ϸ�Ǿ����ϴ�.");
        currentStep++;
        ExecuteStep();
    }

    private void CompleteProcedure()
    {
        UpdateStep("�� �ı� ������ �Ϸ�Ǿ����ϴ�!");
        Debug.Log("��� ������ �Ϸ�Ǿ����ϴ�.");
    }

    private void Update()
    {
        if (currentStep == 0 && Vector3.Distance(player.position, sink.transform.position) < 1.5f)
        {
            CompleteStep();  // ��ũ�� ��ó�� �����ϸ� ���� �ܰ��
        }
    }
}
