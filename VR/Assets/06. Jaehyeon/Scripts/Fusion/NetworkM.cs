using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkM : MonoBehaviour
{
    public NetworkRunner runner;

    private void Start()
    {
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "MyGameSession",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
}
