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
            audioClip = Microphone.Start(Microphone.devices[0], false, 5, sampleRate); // 5초간 녹음
            isRecording = true;
            Debug.Log("녹음 시작...");
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
            Debug.Log("녹음 중지.");
            ProcessAudio();
        }
    }

    private void ProcessAudio()
    {
        if (audioClip != null)
        {
            // WAV 데이터로 변환
            byte[] wavData = WavUtility.FromAudioClip(audioClip);
            if (wavData != null)
            {
                // STTClient를 찾아 오디오 데이터 전송
                STTClient sttClient = FindObjectOfType<STTClient>();
                if (sttClient != null)
                {
                    sttClient.SendAudioData(wavData);
                    Debug.Log("STTClient로 오디오 데이터 전송 완료.");
                }
                else
                {
                    Debug.LogError("STTClient를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogError("WAV 데이터로 변환에 실패했습니다.");
            }
        }
        else
        {
            Debug.LogWarning("녹음된 오디오 클립이 없습니다.");
        }
    }

    public AudioClip GetRecordedClip()
    {
        return audioClip;
    }
}
