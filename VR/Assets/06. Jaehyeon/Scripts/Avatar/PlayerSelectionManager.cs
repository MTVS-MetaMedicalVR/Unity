using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    public GameObject malePlayerPrefab;
    public GameObject femalePlayerPrefab;
    private GameObject currentPlayer;

    // Player ���� ��ġ
    public Transform spawnPoint;

    public void SelectPlayer(string gender)
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer); // ���� �÷��̾� ����
        }

        // ������ ���� �ùٸ� Player Prefab�� �ν��Ͻ�ȭ
        if (gender == "Male")
        {
            currentPlayer = Instantiate(malePlayerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else if (gender == "Female")
        {
            currentPlayer = Instantiate(femalePlayerPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        // ĳ���� ��Ʈ�ѷ� ���� ��ũ��Ʈ�� ����
        var controllerManager = currentPlayer.GetComponent<OVRControllerManager>();
        if (controllerManager != null)
        {
            controllerManager.cameraRig = currentPlayer.transform.Find("CameraRig"); // CameraRig ����
        }
    }
}
