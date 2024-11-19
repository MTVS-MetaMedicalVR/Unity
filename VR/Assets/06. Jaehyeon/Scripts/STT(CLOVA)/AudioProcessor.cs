using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class AudioProcessor : MonoBehaviour
{
    public MicrophoneInput microphoneInput;
    public STTClient sttClient;
    private AudioClip recordedClip;

    void Start()
    {
        if (microphoneInput != null)
        {
            recordedClip = microphoneInput.GetRecordedClip();
            if (recordedClip != null)
            {
                ProcessAudioClip(recordedClip);
            }
            else
            {
                Debug.LogWarning("No audio clip found from MicrophoneInput.");
            }
        }
        else
        {
            Debug.LogError("MicrophoneInput is not assigned in the AudioProcessor.");
        }
    }

    void ProcessAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            Debug.Log("Processing recorded audio clip...");

            // Convert AudioClip to WAV byte array
            byte[] wavData = WavUtility.FromAudioClip(clip);

            if (wavData != null)
            {
                // Send WAV data to STTClient
                if (sttClient != null)
                {
                    sttClient.SendAudioData(wavData);
                }
                else
                {
                    Debug.LogError("STTClient is not assigned in the AudioProcessor.");
                }
            }
            else
            {
                Debug.LogWarning("Failed to convert AudioClip to WAV data.");
            }
        }
        else
        {
            Debug.LogWarning("No audio clip found to process.");
        }
    }
}
