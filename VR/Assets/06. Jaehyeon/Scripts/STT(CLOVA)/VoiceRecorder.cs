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
        audioClip = Microphone.Start(null, false, 10, 44100); // 10�� ���� ����
        Debug.Log("���� ����");
    }

    public void StopRecording()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            SaveAudioClipToFile();
            Debug.Log("���� ���� �� ����");
        }
    }

    private void SaveAudioClipToFile()
    {
        // AudioClip�� ���Ϸ� �����ϴ� �޼��� ����
        var audioData = WavUtility.FromAudioClip(audioClip);
        File.WriteAllBytes(filePath, audioData);
        Debug.Log("���� ���� �Ϸ�: " + filePath);
    }

    public string GetFilePath()
    {
        return filePath;
    }
}

