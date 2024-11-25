using UnityEngine;

public class ActionController : MonoBehaviour
{
    public HeadFollow headFollow;
    public JawFollow jawFollow;

    void Start()
    {
        if (headFollow == null)
            headFollow = FindObjectOfType<HeadFollow>();

        if (jawFollow == null)
            jawFollow = FindObjectOfType<JawFollow>();
    }

    public void ExecuteAction(string action)
    {
        switch (action)
        {
            case "OpenMouth":
                if (jawFollow != null)
                {
                    jawFollow.isOpen = true;  // JawFollow���� isOpen �÷��׸� true�� �����Ͽ� ���� �������� ��
                    Debug.Log("ȯ�� ���� ���� �����ϴ�.");
                }
                break;

            case "CloseMouth":
                if (jawFollow != null)
                {
                    jawFollow.isOpen = false;  // JawFollow���� isOpen �÷��׸� false�� �����Ͽ� ���� �ݵ��� ��
                    Debug.Log("ȯ�� ���� ���� �ݽ��ϴ�.");
                }
                break;

            case "TurnRightHead":
                if (headFollow != null)
                {
                    headFollow.isRight = true;  // HeadFollow���� isRight �÷��׸� true�� �����Ͽ� ���� ���������� �������� ��
                    headFollow.isLeft = false;  // �ݴ� ���� ȸ�� ����
                    Debug.Log("ȯ�� ���� ���� ���������� �����ϴ�.");
                }
                break;

            case "TurnLeftHead":
                if (headFollow != null)
                {
                    headFollow.isLeft = true;  // HeadFollow���� isLeft �÷��׸� true�� �����Ͽ� ���� �������� �������� ��
                    headFollow.isRight = false;  // �ݴ� ���� ȸ�� ����
                    Debug.Log("ȯ�� ���� ���� �������� �����ϴ�.");
                }
                break;

            case "ResetHead":
                if (headFollow != null)
                {
                    headFollow.isLeft = false;
                    headFollow.isRight = false;
                    Debug.Log("ȯ�� ���� ���� �������� �ǵ����ϴ�.");
                }
                break;

            default:
                Debug.Log("�� �� ���� ����: " + action);
                break;
        }
    }
}
