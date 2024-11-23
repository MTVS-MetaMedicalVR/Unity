using UnityEngine;

public class STTActionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private STTManager sttManager;    // STT �Ŵ���
    [SerializeField] private JawFollow jawController;  // �� ��Ʈ�ѷ�
    [SerializeField] private HeadFollow headController; // �� ��Ʈ�ѷ�

    // �ٸ� ��Ʈ�ѷ��鵵 �ʿ��ϴٸ� ���⿡ �߰�
    // [SerializeField] private EyeController eyeController;

    private void OnEnable()
    {
        // STT �̺�Ʈ ����
        STTManager.OnSTTResponse += HandleSTTResponse;
    }

    private void OnDisable()
    {
        // ���� ����
        STTManager.OnSTTResponse -= HandleSTTResponse;
    }

    private void Start()
    {
        // ������Ʈ���� �Ҵ���� �ʾҴٸ� �ڵ����� ã��
        if (sttManager == null)
            sttManager = FindObjectOfType<STTManager>();

        if (jawController == null)
            jawController = FindObjectOfType<JawFollow>();
        
        if (headController == null)
            headController = FindObjectOfType<HeadFollow>();
    }

    // STT ���� ó��
    private void HandleSTTResponse(STTResponse response)
    {
        if (!response.is_valid_command || string.IsNullOrEmpty(response.action))
            return;

        Debug.Log($"STT �׼� ��Ʈ�ѷ�: {response.text} ��� ����");

        // �׼ǿ� ���� ���� ó��
        switch (response.action)
        {
            case "OpenMouth":
                if (jawController != null)
                {
                    jawController.isOpen = true;
                    Debug.Log("�� ������ ���� ����");
                }
                break;

            case "CloseMouth":
                if (jawController != null)
                {
                    jawController.isOpen = false;
                    Debug.Log("�� �ݴ� ���� ����");
                }
                break;

            case "TurnRightHead":
                if (headController != null)
                {
                    headController.isRight = true;
                    headController.isLeft = false;
                    Debug.Log("���������� �� ������ ����");
                }
                break;

            case "TurnLeftHead":
                if (headController != null)
                {
                    headController.isLeft = true;
                    headController.isRight = false;
                    Debug.Log("�������� �� ������ ����");
                }
                break;

            // �ٸ� �׼ǵ鵵 ���⿡ �߰�
            // case "TurnHead":
            //     if (headController != null)
            //         headController.TurnHead();
            //     break;

            // case "CloseEyes":
            //     if (eyeController != null)
            //         eyeController.CloseEyes();
            //     break;

            default:
                Debug.Log($"���ǵ��� ���� �׼�: {response.action}");
                break;
        }
    }

    // �ʿ��� ��� �������� �׼� ����
    public void ExecuteAction(string actionName)
    {
        HandleSTTResponse(new STTResponse
        {
            is_valid_command = true,
            action = actionName
        });
    }
}