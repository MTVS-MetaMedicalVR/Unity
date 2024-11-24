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
        // �ʿ��� ���� �߰� (��: JSON ������ �Ľ� �� UI ������Ʈ)
    }

    private void OnError(string error)
    {
        Debug.LogError("Failed to retrieve doctor info: " + error);
    }
}

