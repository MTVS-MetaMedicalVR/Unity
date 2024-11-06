using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    public GameObject malePlayerPrefab;
    public GameObject femalePlayerPrefab;
    private GameObject currentPlayer;

    // Player 스폰 위치
    public Transform spawnPoint;

    public void SelectPlayer(string gender)
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer); // 기존 플레이어 삭제
        }

        // 성별에 따라 올바른 Player Prefab을 인스턴스화
        if (gender == "Male")
        {
            currentPlayer = Instantiate(malePlayerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else if (gender == "Female")
        {
            currentPlayer = Instantiate(femalePlayerPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        // 캐릭터 컨트롤러 관리 스크립트를 연결
        var controllerManager = currentPlayer.GetComponent<OVRControllerManager>();
        if (controllerManager != null)
        {
            controllerManager.cameraRig = currentPlayer.transform.Find("CameraRig"); // CameraRig 설정
        }
    }
}
