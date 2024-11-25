using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AudioConverter : MonoBehaviour
{
    public AudioClip audioClip;

    public byte[] ConvertAudioClipToWav()
    {
        if (audioClip == null)
        {
            Debug.LogError("AudioClip is null");
            return null;
        }

        // WavUtility를 사용하여 AudioClip을 WAV 데이터로 변환
        byte[] wavData = WavUtility.FromAudioClip(audioClip);
        return wavData;
    }

    public void SaveWavFile(string filePath)
    {
        byte[] wavData = ConvertAudioClipToWav();
        if (wavData != null)
        {
            File.WriteAllBytes(filePath, wavData);
            Debug.Log($"WAV file saved at {filePath}");
        }
    }
}

