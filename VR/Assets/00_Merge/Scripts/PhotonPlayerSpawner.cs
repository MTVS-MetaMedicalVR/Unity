using Fusion;
using UnityEngine;
using System;
using System.Collections.Generic;
using Fusion.Sockets;

public class PhotonPlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef practicePlayerPrefab;
    [SerializeField] private NetworkPrefabRef spectatorPrefab;

    private NetworkRunner runner;
    private Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        runner = GetComponent<NetworkRunner>();
    }

    public void SpawnPlayer(PlayerRef player, SceneType sceneType)
    {
        if (!runner.IsServer) return;

        int currentPlayerCount = spawnedPlayers.Count;
        int maxPlayers = sceneType == SceneType.PracticeRoom ?
                        RoomSettings.PRACTICE_ROOM_MAX_PLAYERS :
                        RoomSettings.SPECTATOR_ROOM_MAX_PLAYERS;

        if (currentPlayerCount >= maxPlayers)
        {
            Debug.Log($"Room is full. Current: {currentPlayerCount}, Max: {maxPlayers}");
            runner.Disconnect(player);
            return;
        }

        NetworkPrefabRef prefabToSpawn = sceneType == SceneType.PracticeRoom ?
                                        practicePlayerPrefab :
                                        spectatorPrefab;

        Vector3 spawnPosition = GetSpawnPosition(sceneType, currentPlayerCount);

        NetworkObject playerObject = runner.Spawn(
            prefabToSpawn,
            spawnPosition,
            Quaternion.identity,
            player
        );

        if (playerObject != null)
        {
            spawnedPlayers.Add(player, playerObject);
            Debug.Log($"Successfully spawned player {player.PlayerId}");
        }
    }

    public void DespawnPlayer(PlayerRef player)
    {
        if (spawnedPlayers.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnedPlayers.Remove(player);
            Debug.Log($"Despawned player {player.PlayerId}");
        }
    }

    private Vector3 GetSpawnPosition(SceneType sceneType, int playerIndex)
    {
        if (sceneType == SceneType.SpectatorRoom)
            return new Vector3(0, 3, 0);

        float xOffset = playerIndex * 2.0f;
        return new Vector3(xOffset, 0, 0);
    }

    #region INetworkRunnerCallbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        SceneType currentSceneType = (SceneType)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        SpawnPlayer(player, currentSceneType);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        DespawnPlayer(player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    #endregion
}