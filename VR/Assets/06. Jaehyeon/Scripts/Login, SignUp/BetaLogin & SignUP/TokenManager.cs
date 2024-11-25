using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class TokenManager : MonoBehaviour
{
    private string refreshTokenUrl = "http://localhost:8081/api/auth/commands/token/refresh"; // 실제 URL로 변경

    public void RefreshAccessToken(string refreshToken, string userId)
    {
        RefreshTokenData tokenData = new RefreshTokenData
        {
            refreshToken = refreshToken,
            userId = userId
        };

        string jsonData = JsonUtility.ToJson(tokenData);
        StartCoroutine(PostRequest(refreshTokenUrl, jsonData));
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
            Debug.Log("Token Refresh Success: " + request.downloadHandler.text);
            // 새로운 토큰 갱신 로직
            TokenResponse response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
            // 필요에 따라 새로운 토큰을 저장합니다.
        }
        else
        {
            Debug.LogError("Token Refresh Failed: " + request.error);
        }
    }
}

[System.Serializable]
public class RefreshTokenData
{
    public string refreshToken;
    public string userId;
}

[System.Serializable]
public class TokenResponse
{
    public string accessToken;
    public string refreshToken;
    public string accessTokenExpiry;
    public string refreshTokenExpiry;
}
