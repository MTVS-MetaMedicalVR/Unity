using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class SignUpManager : MonoBehaviour
{
    private string signUpUrl = "https://api.example.com/api/auth/commands/signup/doctor"; // 실제 URL로 변경

    public void RegisterDoctor(string email, string password, string name, string gender, string specialization, string licenseNumber, bool termsAgreed)
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
