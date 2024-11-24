using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class SignUpManager : MonoBehaviour
{
    private string signUpUrl = "http://localhost:8081/api/auth/commands/signup/doctor"; // API URL

    // UI 요소 연결
    public InputField emailInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public InputField nameInput;
    public InputField genderInput;
    public InputField specializationInput;
    public InputField licenseNumberInput;
    public Toggle termsToggle;

    // 회원가입 버튼 클릭 이벤트
    public void OnSignUpButtonClicked()
    {
        // 모든 값 읽기
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;
        string name = nameInput.text;
        string gender = genderInput.text;
        string specialization = specializationInput.text;
        string licenseNumber = licenseNumberInput.text;
        bool termsAgreed = termsToggle.isOn;

        // 비밀번호 확인
        if (password != confirmPassword)
        {
            Debug.LogError("Passwords do not match!");
            return;
        }

        // 이용약관 확인
        if (!termsAgreed)
        {
            Debug.LogError("You must agree to the terms of use!");
            return;
        }

        // 회원가입 호출
        RegisterDoctor(email, password, name, gender, specialization, licenseNumber, termsAgreed);
    }

    // 회원가입 요청 메서드
    public void RegisterDoctor(string email, string password, string name, string gender, string specialization, string licenseNumber, bool termsAgreed)
    {
        // Doctor 데이터를 객체로 생성
        DoctorData doctorData = new DoctorData
        {
            email = email,
            password = password,
            name = name,
            gender = gender,
            specialization = specialization,
            licenseNumber = licenseNumber,
            termsAgreed = termsAgreed
        };

        // JSON 데이터로 변환
        string jsonData = JsonUtility.ToJson(doctorData);

        // POST 요청 시작
        StartCoroutine(PostRequest(signUpUrl, jsonData));
    }

    private IEnumerator PostRequest(string url, string jsonData)
    {
        byte[] bodyData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();

        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("회원가입 성공: " + request.downloadHandler.text);
            // 추가적으로 회원가입 성공 시 처리 로직 추가 가능
        }
        else
        {
            Debug.LogError("회원가입 실패: " + request.error);
            // 서버에서 온 에러 메시지 출력
            Debug.LogError("응답 내용: " + request.downloadHandler.text);
        }
    }
}

[System.Serializable]
public class DoctorData
{
    public string email;
    public string password;
    public string name;
    public string gender;
    public string specialization;
    public string licenseNumber;
    public bool termsAgreed;
}
