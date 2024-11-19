using System.Collections;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioClip audioClip;
    public int sampleRate = 44100;
    private bool isRecording = false;

    void Start()
    {
        StartMicrophone();
    }

    public void StartMicrophone()
    {
        if (!isRecording)
        {
            audioClip = Microphone.Start(Microphone.devices[0], false, 5, sampleRate); // 5�ʰ� ����
            isRecording = true;
            Debug.Log("���� ����...");
            StartCoroutine(StopRecordingAfterDelay(5));
        }
    }

    private IEnumerator StopRecordingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopMicrophone();
    }

    public void StopMicrophone()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            Debug.Log("���� ����.");
            ProcessAudio();
        }
    }

    private void ProcessAudio()
    {
        if (audioClip != null)
        {
            // WAV �����ͷ� ��ȯ
            byte[] wavData = WavUtility.FromAudioClip(audioClip);
            if (wavData != null)
            {
                // STTClient�� ã�� ����� ������ ����
                STTClient sttClient = FindObjectOfType<STTClient>();
                if (sttClient != null)
                {
                    sttClient.SendAudioData(wavData);
                    Debug.Log("STTClient�� ����� ������ ���� �Ϸ�.");
                }
                else
                {
                    Debug.LogError("STTClient�� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogError("WAV �����ͷ� ��ȯ�� �����߽��ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("������ ����� Ŭ���� �����ϴ�.");
        }
    }

    public AudioClip GetRecordedClip()
    {
        return audioClip;
    }
}
