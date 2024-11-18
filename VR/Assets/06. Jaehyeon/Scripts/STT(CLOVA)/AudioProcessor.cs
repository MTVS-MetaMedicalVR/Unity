using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class AudioProcessor : MonoBehaviour
{
    public MicrophoneInput microphoneInput;
    private AudioClip recordedClip;
    private string apiUrl = "http://192.168.0.61:8000/hoonjang_stt"; // API ���� URL�� �����ϼ���.

    void Start()
    {
        if (microphoneInput != null)
        {
            recordedClip = microphoneInput.GetRecordedClip();
            if (recordedClip != null)
            {
                ProcessAudioClip(recordedClip);
            }
            else
            {
                Debug.LogWarning("No audio clip found from MicrophoneInput.");
            }
        }
        else
        {
            Debug.LogError("MicrophoneInput is not assigned in the AudioProcessor.");
        }
    }

    void ProcessAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            Debug.Log("Processing recorded audio clip...");

            // AudioClip�� WAV ����Ʈ �迭�� ��ȯ
            byte[] wavData = WavUtility.FromAudioClip(clip);

            if (wavData != null)
            {
                // WAV �����͸� Base64 ���ڿ��� ��ȯ
                string base64Audio = Convert.ToBase64String(wavData);
                Debug.Log("Base64 Encoded Audio Data: " + base64Audio);

                // Base64 �����͸� API�� ����
                StartCoroutine(SendAudioToAPI(base64Audio));
            }
            else
            {
                Debug.LogWarning("Failed to convert AudioClip to WAV data.");
            }
        }
        else
        {
            Debug.LogWarning("No audio clip found to process.");
        }
    }

    IEnumerator SendAudioToAPI(string base64Audio)
    {
        // JSON ������ ����
        string jsonData = "{\"audio\":\"" + base64Audio + "\"}";

        // UnityWebRequest ����
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // ��û ����
            yield return request.SendWebRequest();

            // ���� Ȯ��
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending audio data: " + request.error);
            }
            else
            {
                Debug.Log("Server response: " + request.downloadHandler.text);
                // ���� �����͸� ó���ϴ� ���� �߰� ����
            }
        }
    }
}
