using System.Collections;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioClip audioClip;
    public int sampleRate = 44100;
    private bool isRecording = false;

    void Start()
    {
        StartCoroutine(ContinuousMicrophoneRecording());
    }

    private IEnumerator ContinuousMicrophoneRecording()
    {
        while (true)
        {
            if (!isRecording)
            {
                StartMicrophoneRecording();
            }
            yield return new WaitForSeconds(3); // 3�� �������� ���� �����͸� ó��
        }
    }

    private void StartMicrophoneRecording()
    {
        foreach(string a in Microphone.devices)
        {
            print(a);
        }

        if (!isRecording)
        {
            // �⺻ ����ũ ��ġ�� 5�ʰ� ����
            audioClip = Microphone.Start(Microphone.devices[0], false, 5, sampleRate);
            isRecording = true;
            Debug.Log("����ũ ���� ����...");
            StartCoroutine(StopMicrophoneRecording());
        }
    }

    private IEnumerator StopMicrophoneRecording()
    {
        yield return new WaitForSeconds(3);
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            Debug.Log("����ũ ���� ����.");
            ProcessAudio();
        }
    }

    private void ProcessAudio()
    {
        if (audioClip != null)
        {
            // ����� �����͸� WAV �������� ��ȯ
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
