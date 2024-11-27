using Fusion;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class PhotonNetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static PhotonNetworkManager Instance { get; private set; }

    [SerializeField] private GameObject _runnerPrefab;
    public NetworkRunner Runner { get; private set; }

    private Dictionary<string, NetworkRunner> _activeRunners = new Dictionary<string, NetworkRunner>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async void CreateSession(string baseRoomName)
    {
        try
        {
            string practiceRoomName = baseRoomName + "_Practice";
            string spectatorRoomName = baseRoomName + "_Spectator";

            // 각 방에 대해 새로운 Runner 생성
            var practiceRunner = await CreateRoom(practiceRoomName, SceneType.PracticeRoom);
            var spectatorRunner = await CreateRoom(spectatorRoomName, SceneType.SpectatorRoom);

            if (practiceRunner != null)
                _activeRunners[practiceRoomName] = practiceRunner;
            if (spectatorRunner != null)
                _activeRunners[spectatorRoomName] = spectatorRunner;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create session: {e.Message}");
        }
    }

    private async Task<NetworkRunner> CreateRoom(string roomName, SceneType sceneType)
    {
        // 매번 새로운 Runner 인스턴스 생성
        var runnerObject = Instantiate(_runnerPrefab);
        var runner = runnerObject.GetComponent<NetworkRunner>();
        runner.AddCallbacks(this);

        var args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            Scene = SceneRef.FromIndex((int)sceneType),
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = sceneType == SceneType.PracticeRoom ?
                RoomSettings.PRACTICE_ROOM_MAX_PLAYERS :
                RoomSettings.SPECTATOR_ROOM_MAX_PLAYERS
        };

        try
        {
            await runner.StartGame(args);
            Debug.Log($"Successfully created room: {roomName}");
            return runner;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create room {roomName}: {e.Message}");
            Destroy(runnerObject);
            return null;
        }
    }

    public async void JoinSession(string roomName, SceneType sceneType)
    {
        var runnerObject = Instantiate(_runnerPrefab);
        var runner = runnerObject.GetComponent<NetworkRunner>();
        runner.AddCallbacks(this);

        var args = new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomName,
            Scene = SceneRef.FromIndex((int)sceneType),
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        try
        {
            await runner.StartGame(args);
            _activeRunners[roomName] = runner;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join room: {e.Message}");
            Destroy(runnerObject);
        }
    }

    private void OnDestroy()
    {
        // 모든 활성 Runner 정리
        foreach (var runner in _activeRunners.Values)
        {
            if (runner != null)
            {
                runner.Shutdown();
                Destroy(runner.gameObject);
            }
        }
        _activeRunners.Clear();
    }

    // INetworkRunnerCallbacks 구현
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("<<<<<<<< A new player joined to the session >>>>>>>");
        Debug.Log("<<<<<<< IsMasterClient >>>>>>>>" + player.IsMasterClient);
        Debug.Log("<<<<<<< PlayerID >>>>>>>>" + player.PlayerId);
        Debug.Log("<<<<<<< IsRealPlayer >>>>>>>>" + player.IsRealPlayer);

        var PhotonPlayerSpawner = runner.GetComponent<PhotonPlayerSpawner>();
        SceneType currentScene = (SceneType)SceneManager.GetActiveScene().buildIndex;

        PhotonPlayerSpawner.SpawnPlayer(player, currentScene);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("<<<<<<<< A player left the session >>>>>>>");
        Debug.Log("<<<<<<< IsMasterClient >>>>>>>>" + player.IsMasterClient);
        Debug.Log("<<<<<<< PlayerID >>>>>>>>" + player.PlayerId);
        Debug.Log("<<<<<<< IsRealPlayer >>>>>>>>" + player.IsRealPlayer);

        var PhotonPlayerSpawner = runner.GetComponent<PhotonPlayerSpawner>();
        PhotonPlayerSpawner.DespawnPlayer(player);
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        var PhotonRoomManager = FindObjectOfType<PhotonRoomManager>();
        if (PhotonRoomManager != null)
        {
            PhotonRoomManager.UpdateRoomList(sessionList);
        }
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("<<<<<<< Runner Shutdown >>>>>>>>");

    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}