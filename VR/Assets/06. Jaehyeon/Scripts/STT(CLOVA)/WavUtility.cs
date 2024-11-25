using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        // Retrieve audio samples from the clip
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);

        // Convert samples to WAV format byte array
        byte[] wavData = ConvertSamplesToWav(samples, clip.channels, clip.frequency);
        return wavData;
    }

    private static byte[] ConvertSamplesToWav(float[] samples, int channels, int sampleRate)
    {
        int byteLength = samples.Length * 2; // 16-bit samples
        byte[] wav = new byte[44 + byteLength];

        // WAV header
        byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        byte[] data = System.Text.Encoding.UTF8.GetBytes("data");

        Buffer.BlockCopy(riff, 0, wav, 0, 4);
        BitConverter.GetBytes(36 + byteLength).CopyTo(wav, 4);
        Buffer.BlockCopy(wave, 0, wav, 8, 4);
        Buffer.BlockCopy(fmt, 0, wav, 12, 4);
        BitConverter.GetBytes(16).CopyTo(wav, 16); // PCM format
        BitConverter.GetBytes((short)1).CopyTo(wav, 20); // Audio format (PCM)
        BitConverter.GetBytes((short)channels).CopyTo(wav, 22);
        BitConverter.GetBytes(sampleRate).CopyTo(wav, 24);
        BitConverter.GetBytes(sampleRate * channels * 2).CopyTo(wav, 28); // Byte rate
        BitConverter.GetBytes((short)(channels * 2)).CopyTo(wav, 32); // Block align
        BitConverter.GetBytes((short)16).CopyTo(wav, 34); // Bits per sample
        Buffer.BlockCopy(data, 0, wav, 36, 4);
        BitConverter.GetBytes(byteLength).CopyTo(wav, 40);

        // WAV data
        int sampleIndex = 0;
        for (int i = 44; i < wav.Length; i += 2)
        {
            short sample = (short)(samples[sampleIndex++] * short.MaxValue);
            BitConverter.GetBytes(sample).CopyTo(wav, i);
        }

        return wav;
    }
}

