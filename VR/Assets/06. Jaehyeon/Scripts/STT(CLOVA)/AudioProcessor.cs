using System.Collections.Generic;
using UnityEngine;

public class AudioProcessor : MonoBehaviour
{
    public MicrophoneInput microphoneInput;
    public STTClient sttClient;
    private AudioClip recordedClip;

    // 예상 텍스트를 리스트로 관리
    [SerializeField]
    private List<string> expectedTexts = new List<string> { "안녕하세요, 저는 테스트 중입니다.", "아 해보세요", "입 벌려 보세요" };

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
                // Send WAV data and expected texts to STTClient
                if (sttClient != null)
                {
                    foreach (string expectedText in expectedTexts)
                    {
                        sttClient.SendAudioData(wavData, expectedText);
                    }
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
