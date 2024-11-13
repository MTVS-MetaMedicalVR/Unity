using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class HttpClientUtility : MonoBehaviour
{
    public IEnumerator SendGetRequest(string url, string accessToken, Action<string> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            onError?.Invoke(request.error);
        }
    }

    public IEnumerator SendPostRequest(string url, string jsonData, string accessToken, Action<string> onSuccess, Action<string> onError)
    {
        byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            onError?.Invoke(request.error);
        }
    }
}
