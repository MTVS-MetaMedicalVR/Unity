using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Scene ��ȯ�� ���� ���ӽ����̽� �߰�

public class NetworkSetup : MonoBehaviour
{
    // ���� �α��� UI ���
    public NetworkRunner networkRunner;
    public InputField idInputField;
    public InputField passwordInputField;
    public Button loginButton;

    // ���� �߰��� ȸ������ UI ���
    public InputField registerIdInputField;
    public InputField registerPasswordInputField;
    public InputField confirmPasswordInputField;
    public InputField emailInputField;
    public Button registerButton;

    // ������ ��� ��ư
    public Button developerModeButton;

    private void Start()
    {
        // NetworkRunner�� null�� ��� GameObject�� �߰�
        if (networkRunner == null)
        {
            networkRunner = gameObject.AddComponent<NetworkRunner>();
        }

        // �α��� ��ư Ŭ�� �̺�Ʈ ������ �߰�
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(() => HandleLogin().Forget());
        }

        // ȸ������ ��ư Ŭ�� �̺�Ʈ ������ �߰�
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(HandleRegistration);
        }

        // ������ ��� ��ư Ŭ�� �̺�Ʈ ������ �߰�
        if (developerModeButton != null)
        {
            developerModeButton.onClick.AddListener(EnterDeveloperMode);
        }
    }

    private async Task HandleLogin()
    {
        string userId = idInputField != null ? idInputField.text.Trim() : string.Empty;
        string password = passwordInputField != null ? passwordInputField.text.Trim() : string.Empty;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("ID �Ǵ� ��й�ȣ�� �Էµ��� �ʾҽ��ϴ�.");
            return;
        }

        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "MySession_" + userId,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        try
        {
            await networkRunner.StartGame(startArgs);
            Debug.Log("Connected to session successfully!");
            // �α��� ���� ��, �ٸ� ������ �̵�
            SceneManager.LoadScene("PROTO_VR Jae");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to connect to session: {ex.Message}");
        }
    }

    private void HandleRegistration()
    {
        string userId = registerIdInputField != null ? registerIdInputField.text.Trim() : string.Empty;
        string password = registerPasswordInputField != null ? registerPasswordInputField.text.Trim() : string.Empty;
        string confirmPassword = confirmPasswordInputField != null ? confirmPasswordInputField.text.Trim() : string.Empty;
        string email = emailInputField != null ? emailInputField.text.Trim() : string.Empty;

        // ������ ��ȿ�� �˻�
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("��� �ʵ带 �Է��ϼ���.");
            return;
        }

        if (password != confirmPassword)
        {
            Debug.LogWarning("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
            return;
        }

        // �̸��� ���� ������ ��ȿ�� �˻� (�ʿ� �� ���Խ��� ����� �� ����)
        if (!IsValidEmail(email))
        {
            Debug.LogWarning("��ȿ���� ���� �̸��� �����Դϴ�.");
            return;
        }

        // ȸ������ ó�� ���� (��: �����ͺ��̽��� ����, REST API ����)
        Debug.Log($"����� {userId}�� ���������� ��ϵǾ����ϴ�!");
    }

    private void EnterDeveloperMode()
    {
        Debug.Log("������ ���� �����մϴ�.");
        try
        {
            SceneManager.LoadScene("PROTO_VR Jae");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"�� ��ȯ �� ���� �߻�: {ex.Message}");
        }
    }
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

// �߰����� �񵿱� �۾� ���� ��ƿ��Ƽ
public static class TaskExtensions
{
    public static async void Forget(this Task task)
    {
        try
        {
            await task;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Unhandled exception: {ex}");
        }
    }
}
