using UnityEngine;
using Fusion;

public class PlayerSelectionManager : NetworkBehaviour
{
    public NetworkPrefabRef malePrefab;
    public NetworkPrefabRef femalePrefab;
    public Transform spawnPoint; // Player 스폰 위치

    private NetworkObject currentPlayerObject;

    public void SelectPlayer(string gender)
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (runner == null)
        {
            Debug.LogError("NetworkRunner를 찾을 수 없습니다.");
            return;
        }

        if (currentPlayerObject != null)
        {
            // 기존 플레이어 네트워크 객체가 있다면 삭제
            runner.Despawn(currentPlayerObject);
        }

        // 성별에 따라 올바른 네트워크 프리팹을 스폰
        NetworkObject playerObject = null;
        if (gender == "Male")
        {
            playerObject = runner.Spawn(malePrefab, spawnPoint.position, spawnPoint.rotation, Runner.LocalPlayer);
        }
        else if (gender == "Female")
        {
            playerObject = runner.Spawn(femalePrefab, spawnPoint.position, spawnPoint.rotation, Runner.LocalPlayer);
        }

        // 플레이어 오브젝트가 생성되었는지 확인
        if (playerObject != null)
        {
            Debug.Log($"{gender} 캐릭터를 생성했습니다.");
            currentPlayerObject = playerObject; // 현재 플레이어 네트워크 객체를 저장

            // 캐릭터 컨트롤러 관리 스크립트를 설정
            var controllerManager = currentPlayerObject.GetComponent<OVRControllerManager>();
            if (controllerManager != null)
            {
                // CameraRig을 현재 플레이어의 CameraRig로 설정
                controllerManager.cameraRig = currentPlayerObject.transform.Find("CameraRig");
            }
        }
    }
}
