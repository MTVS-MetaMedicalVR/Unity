using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class LoginManager : MonoBehaviour
{
    private string loginUrl = "http://192.168.0.3:8081/api/auth/commands/login"; // 실제 URL로 변경
    public string accessToken;
    public string refreshToken;

    public void LoginDoctor(string email, string password)
    {
        LoginData loginData = new LoginData
        {
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(loginData);
        StartCoroutine(PostRequest(loginUrl, jsonData));
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
            Debug.Log("Login Success: " + request.downloadHandler.text);
            // 응답에서 토큰을 파싱
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            accessToken = response.accessToken;
            refreshToken = response.refreshToken;
        }
        else
        {
            Debug.LogError("Login Failed: " + request.error);
        }
    }
}

[System.Serializable]
public class LoginData
{
    public string email;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public string id;
    public string userId;
    public string accessToken;
    public string refreshToken;
    public string accessTokenExpiry;
    public string refreshTokenExpiry;
}
