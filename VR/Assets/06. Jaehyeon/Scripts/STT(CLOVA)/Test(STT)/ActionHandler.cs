using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    //public List<JawFollow> jawControllers = new List<JawFollow>();
    //public List<HeadFollow> headControllers = new List<HeadFollow>();
    public JawFollow jawController;  // ���� JawFollow
    public HeadFollow headController;  // ���� HeadFollow

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

    private void HandleSTTResponse(string response)
    {
        if (response.Contains("�� �غ�����"))
        {
            HandleAction("OpenMouth");
        }
        else if (response.Contains("�� �������"))
        {
            HandleAction("CloseMouth");
        }
        else if (response.Contains("�������� ���� ������"))
        {
            HandleAction("TurnLeftHead");
        }
        else if (response.Contains("�������� ���� ������"))
        {
            HandleAction("TurnRightHead");
        }
        else
        {
            Debug.Log("�νĵ� �ؽ�Ʈ: " + response + " (�ش��ϴ� ���� ����)");
        }
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
