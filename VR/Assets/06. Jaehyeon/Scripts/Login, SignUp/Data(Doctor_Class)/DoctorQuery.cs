using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorQuery : MonoBehaviour
{
    [SerializeField] private HttpClientUtility httpClient;

    private string baseUrl = "https://api.example.com/api/doctors/queries/";

    public void GetDoctorInfo(string doctorId)
    {
        string url = baseUrl + doctorId;
        StartCoroutine(httpClient.SendGetRequest(url, "YOUR_ACCESS_TOKEN", OnSuccess, OnError));
    }

    private void OnSuccess(string response)
    {
        Debug.Log("Doctor Info: " + response);
        // 필요한 로직 추가 (예: JSON 데이터 파싱 및 UI 업데이트)
    }

    private void OnError(string error)
    {
        Debug.LogError("Failed to retrieve doctor info: " + error);
    }
}

