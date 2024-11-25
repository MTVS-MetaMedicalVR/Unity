using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;

public class VoiceRecorder : MonoBehaviour
{
    private AudioClip audioClip;
    private string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath + "/recordedAudio.wav";
    }

    public void StartRecording()
    {
        audioClip = Microphone.Start(null, false, 10, 44100); // 10초 동안 녹음
        Debug.Log("녹음 시작");
    }

    public void StopRecording()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            SaveAudioClipToFile();
            Debug.Log("녹음 중지 및 저장");
        }
    }

    private void SaveAudioClipToFile()
    {
        // AudioClip을 파일로 저장하는 메서드 구현
        var audioData = WavUtility.FromAudioClip(audioClip);
        File.WriteAllBytes(filePath, audioData);
        Debug.Log("파일 저장 완료: " + filePath);
    }

    public string GetFilePath()
    {
        return filePath;
    }
}

