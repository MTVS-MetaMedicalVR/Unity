using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public JawFollow jawController;  // ���� JawFollow
    public HeadFollow headController;  // ���� HeadFollow

    //private const float AccuracyThreshold = 0.8f; // �ּ� �νķ� (80%)
    // ��ɾ� ����Ʈ
    private readonly List<string> openMouthCommands = new List<string> { "�� �غ�����", "�� �� ������", "�� ���� ������", "���� ������", "�غ�����" };
    private readonly List<string> closeMouthCommands = new List<string> { "�� �������", "�� �ٹ��� ������", "�ٹ��� ������", "�ٹ������", "�������ϴ�" };
    private readonly List<string> turnLeftCommands = new List<string> { "�������� ���� ������", "�������� ���� ������", "��������" };
    private readonly List<string> turnRightCommands = new List<string> { "�������� ���� ������", "���������� ���� ������", "��������" };

    private void OnEnable()
    {
        // STTClient�� �̺�Ʈ ����
        STTClient.OnSTTResponseReceived += HandleSTTResponse;
    }

    private void OnDisable()
    {
        // STTClient�� �̺�Ʈ ���� ����
        STTClient.OnSTTResponseReceived -= HandleSTTResponse;
    }

    private void HandleSTTResponse(string recognizedText, string recognitionAccuracy)
    {
        Debug.Log($"�νĵ� �ؽ�Ʈ: {recognizedText} (��Ȯ��: {recognitionAccuracy})");

        if (IsCommandMatched(recognizedText, openMouthCommands))
        {
            HandleAction("OpenMouth");
        }
        else if (IsCommandMatched(recognizedText, closeMouthCommands))
        {
            HandleAction("CloseMouth");
        }
        else if (IsCommandMatched(recognizedText, turnLeftCommands))
        {
            HandleAction("TurnLeftHead");
        }
        else if (IsCommandMatched(recognizedText, turnRightCommands))
        {
            HandleAction("TurnRightHead");
        }
        else
        {
            Debug.Log("�νĵ� �ؽ�Ʈ�� �ش��ϴ� ������ �����ϴ�.");
        }
    }

    // �־��� ������ ��ɾ� ����Ʈ�� ���ԵǾ� �ִ��� Ȯ���ϴ� �޼���
    private bool IsCommandMatched(string response, List<string> commands)
    {
        foreach (string command in commands)
        {
            if (response.Contains(command))
            {
                return true;
            }
        }
        return false;
    }

    public void HandleAction(string action)
    {
        switch (action)
        {
            case "OpenMouth":
                if (jawController != null)
                {
                    jawController.isOpen = true;
                    Debug.Log("ȯ�� �� ������ ���� ����");
                }
                else
                {
                    Debug.LogWarning("JawFollow ��Ʈ�ѷ��� �������� �ʾҽ��ϴ�.");
                }
                break;

            case "CloseMouth":
                if (jawController != null)
                {
                    jawController.isOpen = false;
                    Debug.Log("ȯ�� �� �ݱ� ���� ����");
                }
                else
                {
                    Debug.LogWarning("JawFollow ��Ʈ�ѷ��� �������� �ʾҽ��ϴ�.");
                }
                break;

            case "TurnLeftHead":
                if (headController != null)
                {
                    headController.isLeft = true;
                    headController.isRight = false;
                    Debug.Log("ȯ�� �������� �� ������ ���� ����");
                }
                else
                {
                    Debug.LogWarning("HeadFollow ��Ʈ�ѷ��� �������� �ʾҽ��ϴ�.");
                }
                break;

            case "TurnRightHead":
                if (headController != null)
                {
                    headController.isRight = true;
                    headController.isLeft = false;
                    Debug.Log("ȯ�� ���������� �� ������ ���� ����");
                }
                else
                {
                    Debug.LogWarning("HeadFollow ��Ʈ�ѷ��� �������� �ʾҽ��ϴ�.");
                }
                break;

            default:
                Debug.Log("�� �� ���� ����: " + action);
                break;
        }
    }
}
