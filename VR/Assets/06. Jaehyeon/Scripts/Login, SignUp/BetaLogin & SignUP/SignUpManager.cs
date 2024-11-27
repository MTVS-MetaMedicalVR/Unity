using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Net;

public class SignUpManager : MonoBehaviour
{
    private string signUpUrl = "http://192.168.0.33:8081/api/auth/commands/signup/doctor";

    // UI ��� ����
    [Header("Step 1 Fields")]
    public GameObject step1Fields; // Step1 �ʵ� (ID, Password, Confirm Password)
    public InputField emailInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;

    [Header("Step 2 Fields")]
    public GameObject step2Fields; // Step2 �ʵ� (Name, Gender, etc.)
    public InputField nameInput;
    public InputField genderInput;
    public InputField specializationInput;
    public InputField licenseNumberInput;
    public Toggle termsToggle;

    private string tempEmail; // Step1���� ����� ������
    private string tempPassword;

    private void Start()
    {
        // Step2 �ʵ� ��Ȱ��ȭ
        step2Fields.SetActive(false);
    }

    public void OnContinueButtonClicked()
    {
        // Step1 ������ ����
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (password != confirmPassword)
        {
            Debug.LogError("Passwords do not match!");
            return;
        }

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email and Password cannot be empty!");
            return;
        }

        // Step1 ������ �ӽ� ����
        tempEmail = email;
        tempPassword = password;

        // Step1Fields ����� Step2Fields Ȱ��ȭ
        step1Fields.SetActive(false);
        step2Fields.SetActive(true);
    }

    public void OnSignUpButtonClicked()
    {
        // Step2 ������ �б�
        string name = nameInput.text;
        string gender = genderInput.text;
        string specialization = specializationInput.text;
        string licenseNumber = licenseNumberInput.text;
        bool termsAgreed = termsToggle.isOn;

        // �ʼ� �Է°� Ȯ��
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(specialization) || string.IsNullOrEmpty(licenseNumber))
        {
            Debug.LogError("All fields in Step 2 are required!");
            return;
        }

        if (!termsAgreed)
        {
            Debug.LogError("You must agree to the terms!");
            return;
        }

        // ȸ������ ��û
        RegisterDoctor(tempEmail, tempPassword, name, gender, specialization, licenseNumber, termsAgreed);
    }

    private void RegisterDoctor(string email, string password, string name, string gender, string specialization, string licenseNumber, bool termsAgreed)
    {
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

        string jsonData = JsonUtility.ToJson(doctorData);
        StartCoroutine(PostRequest(signUpUrl, jsonData));
    }

    private IEnumerator PostRequest(string url, string jsonData)
    {
        // SSL ������ ���� ���� (���߿�)
        //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        byte[] bodyData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registration Success: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Registration Failed: " + request.error);
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
