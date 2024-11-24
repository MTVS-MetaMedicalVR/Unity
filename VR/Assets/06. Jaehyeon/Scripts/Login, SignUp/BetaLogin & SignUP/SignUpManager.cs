using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class SignUpManager : MonoBehaviour
{
    private string signUpUrl = "http://localhost:8081/api/auth/commands/signup/doctor"; // API URL

    // UI ��� ����
    public InputField emailInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public InputField nameInput;
    public InputField genderInput;
    public InputField specializationInput;
    public InputField licenseNumberInput;
    public Toggle termsToggle;

    // ȸ������ ��ư Ŭ�� �̺�Ʈ
    public void OnSignUpButtonClicked()
    {
        // ��� �� �б�
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;
        string name = nameInput.text;
        string gender = genderInput.text;
        string specialization = specializationInput.text;
        string licenseNumber = licenseNumberInput.text;
        bool termsAgreed = termsToggle.isOn;

        // ��й�ȣ Ȯ��
        if (password != confirmPassword)
        {
            Debug.LogError("Passwords do not match!");
            return;
        }

        // �̿��� Ȯ��
        if (!termsAgreed)
        {
            Debug.LogError("You must agree to the terms of use!");
            return;
        }

        // ȸ������ ȣ��
        RegisterDoctor(email, password, name, gender, specialization, licenseNumber, termsAgreed);
    }

    // ȸ������ ��û �޼���
    public void RegisterDoctor(string email, string password, string name, string gender, string specialization, string licenseNumber, bool termsAgreed)
    {
        // Doctor �����͸� ��ü�� ����
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

        // JSON �����ͷ� ��ȯ
        string jsonData = JsonUtility.ToJson(doctorData);

        // POST ��û ����
        StartCoroutine(PostRequest(signUpUrl, jsonData));
    }

    private IEnumerator PostRequest(string url, string jsonData)
    {
        byte[] bodyData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // ��û ����
        yield return request.SendWebRequest();

        // ���� ó��
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ȸ������ ����: " + request.downloadHandler.text);
            // �߰������� ȸ������ ���� �� ó�� ���� �߰� ����
        }
        else
        {
            Debug.LogError("ȸ������ ����: " + request.error);
            // �������� �� ���� �޽��� ���
            Debug.LogError("���� ����: " + request.downloadHandler.text);
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
