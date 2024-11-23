using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;
    
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Debug.Log("Local Player" + player.PlayerId + "joined !");
            Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        }
    }
}
