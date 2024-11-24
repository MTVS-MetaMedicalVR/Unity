using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordShareQuery : MonoBehaviour
{
    [SerializeField] private HttpClientUtility httpClient;

    private string baseUrl = "https://api.example.com/api/records/queries/";

    public void GetRecordShareStatus(string recordId)
    {
        string url = baseUrl + recordId + "/shares";
        StartCoroutine(httpClient.SendGetRequest(url, "YOUR_ACCESS_TOKEN", OnSuccess, OnError));
    }

    private void OnSuccess(string response)
    {
        Debug.Log("Record Share Status: " + response);
        // �ʿ��� ���� �߰� (��: ������ ó��)
    }

    private void OnError(string error)
    {
        Debug.LogError("Failed to retrieve record share status: " + error);
    }
}
