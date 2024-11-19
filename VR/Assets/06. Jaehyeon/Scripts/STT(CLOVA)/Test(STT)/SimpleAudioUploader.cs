using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleAudioUploader : MonoBehaviour
{
    private string apiUrl = "http://192.168.0.61:8000/hoonjang_stt";
    private AudioClip audioClip;

    void Start()
    {
        StartCoroutine(RecordAndSendAudio());
    }

    private IEnumerator RecordAndSendAudio()
    {
        // 녹음 시작
        audioClip = Microphone.Start(null, false, 10, 44100);
        yield return new WaitForSeconds(10); // 10초 녹음

        // 녹음 종료
        Microphone.End(null);

        // 오디오 데이터를 WAV로 변환
        byte[] wavData = WavUtility.FromAudioClip(audioClip);

        // 서버로 전송
        StartCoroutine(UploadAudio(wavData));
    }

    private IEnumerator UploadAudio(byte[] audioData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recordedAudio.wav", "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending audio data: " + request.error);
            }
            else
            {
                Debug.Log("Server response: " + request.downloadHandler.text);
            }
        }
    }
}
