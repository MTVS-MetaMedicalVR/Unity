using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class ClovaSpeechClient : MonoBehaviour
{
    private string apiUrl = "https://naveropenapi.apigw.ntruss.com/recog/v1/stt?lang=Kor"; // Clova Speech API 엔드포인트
    private string clientId = "YOUR_CLIENT_ID"; // Clova Speech API 클라이언트 ID
    private string clientSecret = "YOUR_CLIENT_SECRET"; // Clova Speech API 클라이언트 Secret

    public void SendAudioDataToClova(string base64Audio)
    {
        StartCoroutine(SendRequest(base64Audio));
    }

    private IEnumerator SendRequest(string base64Audio)
    {
        // Prepare the data as a byte array
        byte[] audioData = System.Convert.FromBase64String(base64Audio);

        // Create UnityWebRequest for POST request
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(audioData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/octet-stream");
            request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", clientId);
            request.SetRequestHeader("X-NCP-APIGW-API-KEY", clientSecret);

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // Handle the response from Clova
                Debug.Log("Response from Clova: " + request.downloadHandler.text);
            }
        }
    }
}
