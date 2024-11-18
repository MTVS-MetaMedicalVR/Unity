using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    private AudioClip audioClip;
    private bool isRecording = false;
    public int sampleRate = 16000;

    void Start()
    {
        // 마이크로부터 오디오 녹음 시작
        StartMicrophone();
    }

    void StartMicrophone()
    {
        if (!isRecording)
        {
            audioClip = Microphone.Start(null, false, 5, sampleRate);
            isRecording = true;
            Debug.Log("Recording started...");
        }
    }

    public AudioClip StopMicrophone()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            Debug.Log("Recording stopped.");
        }
        return audioClip;
    }

    public AudioClip GetRecordedClip()
    {
        return audioClip;
    }
}

