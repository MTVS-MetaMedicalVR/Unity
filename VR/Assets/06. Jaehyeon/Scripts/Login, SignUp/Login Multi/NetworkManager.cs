using Fusion;
//using Photon.Fusion;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public async void StartGameSession()
    {
        var networkRunner = gameObject.AddComponent<NetworkRunner>();
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "MainRoom",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log("Connected to Photon Fusion session");
    }
}
