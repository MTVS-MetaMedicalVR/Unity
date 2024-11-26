using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommConnectionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public void CreateRoom()
    {
        CommNetworkManager.Instance.CreateSession(inputField.text);
    }

    public void JoinRoom()
    {
        CommNetworkManager.Instance.JoinSession(inputField.text);

    }
}
