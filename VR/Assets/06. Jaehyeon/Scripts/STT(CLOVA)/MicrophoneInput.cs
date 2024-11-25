using System.Collections;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioClip audioClip;
    public int sampleRate = 44100;
    private bool isRecording = false;

    // 예상 텍스트 리스트를 지원하도록 수정
    [SerializeField]
    private string expectedText = "안녕하세요, 저는 테스트 중입니다.";

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
            yield return new WaitForSeconds(3); // 3초 간격으로 녹음 데이터를 처리
        }
    }

    private void StartMicrophoneRecording()
    {
        if (!isRecording)
        {
            // 기본 마이크 장치로 5초간 녹음
            if (Microphone.devices.Length > 0)
            {
                audioClip = Microphone.Start(Microphone.devices[0], false, 5, sampleRate);
                isRecording = true;
                Debug.Log("마이크 녹음 시작...");
                StartCoroutine(StopMicrophoneRecording());
            }
            else
            {
                Debug.LogError("마이크 장치가 없습니다.");
            }
        }
    }

    private IEnumerator StopMicrophoneRecording()
    {
        yield return new WaitForSeconds(3);
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            Debug.Log("마이크 녹음 종료.");
            ProcessAudio();
        }
    }

    private void ProcessAudio()
    {
        if (audioClip != null)
        {
            // 오디오 데이터를 WAV 형식으로 변환
            byte[] wavData = WavUtility.FromAudioClip(audioClip);
            if (wavData != null)
            {
                // STTClient를 찾아 오디오 데이터와 예상 텍스트 전송
                STTClient sttClient = FindObjectOfType<STTClient>();
                if (sttClient != null)
                {
                    sttClient.SendAudioData(wavData, expectedText);
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
