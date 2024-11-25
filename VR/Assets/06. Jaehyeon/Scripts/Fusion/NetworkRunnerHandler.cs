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
        // NetworkRunner가 이미 존재하는지 확인
        _runner = FindObjectOfType<NetworkRunner>();
        if (_runner == null)
        {
            GameObject runnerObject = new GameObject("NetworkRunner");
            _runner = runnerObject.AddComponent<NetworkRunner>();
            DontDestroyOnLoad(runnerObject); // NetworkRunner를 유지
        }
        else
        {
            Debug.Log("NetworkRunner already exists. Reusing existing instance.");
            return; // 중복 실행 방지
        }

        // SceneManager 추가
        var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        // StartGameArgs 설정
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "ConferenceRoom",
            SceneManager = sceneManager,
            Scene = SceneRef.None // 현재 씬 관리하지 않음
        };

        // 게임 시작
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
