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

        // WavUtility�� ����Ͽ� AudioClip�� WAV �����ͷ� ��ȯ
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

