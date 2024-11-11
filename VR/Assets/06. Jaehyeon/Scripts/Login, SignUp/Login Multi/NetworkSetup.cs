using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Scene 전환을 위한 네임스페이스 추가

public class NetworkSetup : MonoBehaviour
{
    // 기존 로그인 UI 요소
    public NetworkRunner networkRunner;
    public InputField idInputField;
    public InputField passwordInputField;
    public Button loginButton;

    // 새로 추가된 회원가입 UI 요소
    public InputField registerIdInputField;
    public InputField registerPasswordInputField;
    public InputField confirmPasswordInputField;
    public InputField emailInputField;
    public Button registerButton;

    // 개발자 모드 버튼
    public Button developerModeButton;

    private void Start()
    {
        // NetworkRunner가 null인 경우 GameObject에 추가
        if (networkRunner == null)
        {
            networkRunner = gameObject.AddComponent<NetworkRunner>();
        }

        // 로그인 버튼 클릭 이벤트 리스너 추가
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(() => HandleLogin().Forget());
        }

        // 회원가입 버튼 클릭 이벤트 리스너 추가
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(HandleRegistration);
        }

        // 개발자 모드 버튼 클릭 이벤트 리스너 추가
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
            Debug.LogWarning("ID 또는 비밀번호가 입력되지 않았습니다.");
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
            // 로그인 성공 시, 다른 씬으로 이동
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

        // 간단한 유효성 검사
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("모든 필드를 입력하세요.");
            return;
        }

        if (password != confirmPassword)
        {
            Debug.LogWarning("비밀번호가 일치하지 않습니다.");
            return;
        }

        // 이메일 형식 간단한 유효성 검사 (필요 시 정규식을 사용할 수 있음)
        if (!IsValidEmail(email))
        {
            Debug.LogWarning("유효하지 않은 이메일 형식입니다.");
            return;
        }

        // 회원가입 처리 로직 (예: 데이터베이스에 저장, REST API 연동)
        Debug.Log($"사용자 {userId}가 성공적으로 등록되었습니다!");
    }

    private void EnterDeveloperMode()
    {
        Debug.Log("개발자 모드로 진입합니다.");
        try
        {
            SceneManager.LoadScene("PROTO_VR Jae");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"씬 전환 중 오류 발생: {ex.Message}");
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

// 추가적인 비동기 작업 관리 유틸리티
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
