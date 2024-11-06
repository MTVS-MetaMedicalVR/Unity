using UnityEngine;
using Fusion;

public class PlayerSelectionManager : NetworkBehaviour
{
    public NetworkPrefabRef malePrefab;
    public NetworkPrefabRef femalePrefab;
    public Transform spawnPoint; // Player ���� ��ġ

    private NetworkObject currentPlayerObject;

    public void SelectPlayer(string gender)
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (runner == null)
        {
            Debug.LogError("NetworkRunner�� ã�� �� �����ϴ�.");
            return;
        }

        if (currentPlayerObject != null)
        {
            // ���� �÷��̾� ��Ʈ��ũ ��ü�� �ִٸ� ����
            runner.Despawn(currentPlayerObject);
        }

        // ������ ���� �ùٸ� ��Ʈ��ũ �������� ����
        NetworkObject playerObject = null;
        if (gender == "Male")
        {
            playerObject = runner.Spawn(malePrefab, spawnPoint.position, spawnPoint.rotation, Runner.LocalPlayer);
        }
        else if (gender == "Female")
        {
            playerObject = runner.Spawn(femalePrefab, spawnPoint.position, spawnPoint.rotation, Runner.LocalPlayer);
        }

        // �÷��̾� ������Ʈ�� �����Ǿ����� Ȯ��
        if (playerObject != null)
        {
            Debug.Log($"{gender} ĳ���͸� �����߽��ϴ�.");
            currentPlayerObject = playerObject; // ���� �÷��̾� ��Ʈ��ũ ��ü�� ����

            // ĳ���� ��Ʈ�ѷ� ���� ��ũ��Ʈ�� ����
            var controllerManager = currentPlayerObject.GetComponent<OVRControllerManager>();
            if (controllerManager != null)
            {
                // CameraRig�� ���� �÷��̾��� CameraRig�� ����
                controllerManager.cameraRig = currentPlayerObject.transform.Find("CameraRig");
            }
        }
    }
}
