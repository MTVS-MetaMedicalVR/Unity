using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareRig : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Transform References")]
    public Transform playerTransform;
    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    private NetworkRunner runner;
    private bool isInitialized = false;

    private void Start()
    {
        StartCoroutine(WaitForPhotonNetworkManager());
    }

    private IEnumerator WaitForPhotonNetworkManager()
    {
        while (PhotonNetworkManager.Instance == null || PhotonNetworkManager.Instance.Runner == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // PhotonNetworkManager와 Runner가 준비되면 초기화
        runner = PhotonNetworkManager.Instance.Runner;
        runner.AddCallbacks(this);
        isInitialized = true;
        Debug.Log("HardwareRig initialized successfully");
    }

    private void OnDestroy()
    {
        if (isInitialized && runner != null)
        {
            runner.RemoveCallbacks(this);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!isInitialized || !gameObject.activeInHierarchy) return;

        try
        {
            var rigState = new RigState
            {
                HeadsetPosition = headTransform.position,
                HeadsetRotation = headTransform.rotation,
                PlayerPosition = playerTransform.position,
                PlayerRotation = playerTransform.rotation,
                LeftHandPosition = leftHandTransform.position,
                LeftHandRotation = leftHandTransform.rotation,
                RightHandPosition = rightHandTransform.position,
                RightHandRotation = rightHandTransform.rotation
            };

            input.Set(rigState);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in OnInput: {e.Message}");
        }
    }

    #region INetworkRunnerCallbacks Implementation
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}
public struct RigState : INetworkInput
{
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;

    public Vector3 HeadsetPosition;
    public Quaternion HeadsetRotation;

    public Vector3 LeftHandPosition;
    public Quaternion LeftHandRotation;

    public Vector3 RightHandPosition;
    public Quaternion RightHandRotation;
}
