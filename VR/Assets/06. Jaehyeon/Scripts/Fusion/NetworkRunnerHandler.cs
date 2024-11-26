using System.Collections;
using System.Collections.Generic;
using Fusion.Sockets;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    private NetworkRunner _runner;

    private void Start()
    {
        StartGame();
    }

    private async void StartGame()
    {
        // NetworkRunner�� �̹� �����ϴ��� Ȯ��
        _runner = FindObjectOfType<NetworkRunner>();
        if (_runner == null)
        {
            GameObject runnerObject = new GameObject("NetworkRunner");
            _runner = runnerObject.AddComponent<NetworkRunner>();
            DontDestroyOnLoad(runnerObject); // NetworkRunner�� ����
        }
        else
        {
            Debug.Log("NetworkRunner already exists. Reusing existing instance.");
            return; // �ߺ� ���� ����
        }

        // SceneManager �߰�
        var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        // StartGameArgs ����
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "ConferenceRoom",
            SceneManager = sceneManager,
            Scene = SceneRef.None // ���� �� �������� ����
        };

        // ���� ����
        var result = await _runner.StartGame(startGameArgs);

        if (result.Ok)
        {
            Debug.Log("Session started successfully!");
        }
        else
        {
            Debug.LogError($"Failed to start session: {result.ShutdownReason}");
        }
    }
}
