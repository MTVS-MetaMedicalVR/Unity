using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class STTVoiceRecorder : MonoBehaviour
{
    public AudioClip audioClip;
    private string serverUrl = "http://192.168.204.27:8000/hoonjang_stt";  // Python ���� URL
    private ActionController actionController;

    void Start()
    {
        // ActionController �ν��Ͻ� ã��
        actionController = FindObjectOfType<ActionController>();

        // ����ũ ���� ��û (�ʿ��� ���)
        StartCoroutine(RequestMicrophonePermission());
    }

    IEnumerator RequestMicrophonePermission()
    {
        // PC������ ������ ����ũ ���� ��û�� �ǳʶݴϴ�.
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
        else
        {
            yield break;  // PC������ ��� ��ȯ
        }
    }


    public void StartRecording()
    {
        // ��� ������ ����ũ ��ġ ��� ���
        string[] devices = Microphone.devices;
        if (devices.Length > 0)
        {
            // ù ��° ��ġ�� �⺻���� ����ϰų� ���ϴ� ��ġ�� ����
            string selectedDevice = devices[0]; // "Microphone Array"�� ������ �� ����
            audioClip = Microphone.Start(selectedDevice, false, 5, 16000);
            Debug.Log($"���� ���� ���� - ���õ� ��ġ: {selectedDevice}");
        }
        else
        {
            Debug.LogWarning("��� ������ ����ũ ��ġ�� �����ϴ�.");
        }
    }


    public void StopRecordingAndSend()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            Debug.Log("���� ���� ����");

            // ������ ����� �����͸� byte[]�� ��ȯ�Ͽ� ������ ����
            float[] samples = new float[audioClip.samples];
            audioClip.GetData(samples, 0);
            byte[] audioData = ConvertAudioClipToByteArray(samples);

            StartCoroutine(SendAudioToServer(audioData));
        }
    }

    private byte[] ConvertAudioClipToByteArray(float[] samples)
    {
        // PCM 16��Ʈ ����� �����ͷ� ��ȯ
        byte[] data = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = (short)(samples[i] * short.MaxValue);
            data[i * 2] = (byte)(sample & 0xFF);
            data[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }
        return data;
    }

    IEnumerator SendAudioToServer(byte[] audioData)
    {
        string base64Audio = System.Convert.ToBase64String(audioData);
        string jsonPayload = "{\"audio\": \"" + base64Audio + "\"}";

        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("������ ������ ���� ��...");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("���� ���� ����: " + www.downloadHandler.text);
                HandleServerResponse(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("���� ��û ����: " + www.error);
            }
        }
    }


    private void HandleServerResponse(string responseJson)
    {
        // JSON ������ �Ľ��ϰ� �ൿ�� ���� VR �� ����
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJson);
        if (responseData.status == "success" && responseData.action != null)
        {
            if (actionController != null)
            {
                actionController.ExecuteAction(responseData.action);
            }
            else
            {
                Debug.LogWarning("ActionController�� �������� �ʾҽ��ϴ�.");
            }
        }
        else
        {
            Debug.Log("��ȿ�� ��ɾ �����ϴ�.");
        }
    }

    [System.Serializable]
    public class ResponseData
    {
        public string status;
        public string text;
        public string matched_command;
        public float similarity;
        public string action;
    }
}
