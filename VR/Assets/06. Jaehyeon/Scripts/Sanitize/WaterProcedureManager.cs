using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaterProcedureManager : MonoBehaviour
{
    public Transform player;
    public float proximityThreshold = 1.5f;
    public Text stepDescriptionText;

    private int currentStepIndex = 0;
    private string[] steps = { "turn_on_water", "use_soap", "turn_off_water" };
    private bool isNearSink = false;

    private void Update()
    {
        // �� �����Ӹ��� �÷��̾���� �Ÿ��� Ȯ��
        CheckPlayerProximity();

        // ��� ���� �Ϸ� �� ó��
        if (currentStepIndex >= steps.Length)
        {
            Debug.Log("��� ������ �Ϸ�Ǿ����ϴ�.");
            return;
        }

        // ���� �ܰ� ���� ���� Ȯ��
        if (isNearSink && Input.GetKeyDown(KeyCode.Space)) // Space Ű�� ��ȣ�ۿ� Ʈ����
        {
            ExecuteCurrentStep();
        }
    }

    // �÷��̾ ��ũ�� ��ó�� �ִ��� Ȯ���ϴ� �޼���
    private void CheckPlayerProximity()
    {
        //float distance = Vector3.Distance(player.position, transform.position);
        //isNearSink = distance <= proximityThreshold;
    }

    // ���� �ܰ踦 �����ϴ� �޼���
    private void ExecuteCurrentStep()
    {
        string step = steps[currentStepIndex];
        Debug.Log($"{step} �ܰ谡 ����Ǿ����ϴ�.");
        UpdateStepDescription($"{step} �ܰ� ���� ��...");

        // �� �ܰ迡 �´� ���� ����
        switch (step)
        {
            case "turn_on_water":
                TurnOnWater();
                break;
            case "use_soap":
                UseSoap();
                break;
            case "turn_off_water":
                TurnOffWater();
                break;
        }

        currentStepIndex++; // ���� �ܰ�� �̵�
    }

    // �ܰ� ���� ������Ʈ
    private void UpdateStepDescription(string description)
    {
        if (stepDescriptionText != null)
        {
            stepDescriptionText.text = description;
        }
    }

    // �� Ʈ��
    private void TurnOnWater()
    {
        Debug.Log("���� Ʋ�����ϴ�.");
    }

    // �� ���
    private void UseSoap()
    {
        Debug.Log("�񴩸� ����߽��ϴ�.");
    }

    // �� ����
    private void TurnOffWater()
    {
        Debug.Log("���� �����ϴ�.");
    }
}
