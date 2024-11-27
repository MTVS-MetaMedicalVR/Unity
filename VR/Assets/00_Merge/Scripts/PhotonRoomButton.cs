// PhotonRoomButtonUI.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PhotonRoomButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI practicePlayersText;
    [SerializeField] private TextMeshProUGUI spectatorPlayersText;
    [SerializeField] private Toggle practiceToggle;
    [SerializeField] private Toggle spectatorToggle;

    private PhotonRoomButtonInfo roomInfo;

    public void Initialize(PhotonRoomButtonInfo info)
    {
        roomInfo = info;

        // �ؽ�Ʈ ������Ʈ
        roomNameText.text = info.RoomName;
        practicePlayersText.text = $"Players: {info.PracticeCurrentPlayers}/{RoomSettings.PRACTICE_ROOM_MAX_PLAYERS}";
        spectatorPlayersText.text = $"Spectators: {info.SpectatorCurrentPlayers}/{RoomSettings.SPECTATOR_ROOM_MAX_PLAYERS}";

        // ��� �̺�Ʈ ����
        SetupToggles();

        // ��� ���� ������Ʈ
        UpdateToggleStates();
    }

    private void SetupToggles()
    {
        // ���� ������ ����
        practiceToggle.onValueChanged.RemoveAllListeners();
        spectatorToggle.onValueChanged.RemoveAllListeners();

        // �� ������ �߰�
        practiceToggle.onValueChanged.AddListener((isOn) => {
            if (isOn)
            {
                spectatorToggle.isOn = false;
                TryJoinRoom(SceneType.PracticeRoom);
            }
        });

        spectatorToggle.onValueChanged.AddListener((isOn) => {
            if (isOn)
            {
                practiceToggle.isOn = false;
                TryJoinRoom(SceneType.SpectatorRoom);
            }
        });
    }

    private void UpdateToggleStates()
    {
        // �ǽ��� ��� ����
        practiceToggle.interactable = roomInfo.HasPracticeRoom && !roomInfo.IsPracticeRoomFull;
        if (!practiceToggle.interactable)
        {
            practiceToggle.isOn = false;
        }

        // ������ ��� ����
        spectatorToggle.interactable = roomInfo.HasSpectatorRoom && !roomInfo.IsSpectatorRoomFull;
        if (!spectatorToggle.interactable)
        {
            spectatorToggle.isOn = false;
        }

        // ���� ������Ʈ �� �ð��� �ǵ��
        UpdateVisualFeedback();
    }

    private void UpdateVisualFeedback()
    {
        // Practice Toggle ����
        var practiceColors = practiceToggle.colors;
        if (!roomInfo.HasPracticeRoom)
            practiceColors.disabledColor = Color.gray;
        else if (roomInfo.IsPracticeRoomFull)
            practiceColors.disabledColor = Color.yellow;
        practiceToggle.colors = practiceColors;

        // Spectator Toggle ����
        var spectatorColors = spectatorToggle.colors;
        if (!roomInfo.HasSpectatorRoom)
            spectatorColors.disabledColor = Color.gray;
        else if (roomInfo.IsSpectatorRoomFull)
            spectatorColors.disabledColor = Color.yellow;
        spectatorToggle.colors = spectatorColors;
    }

    private void TryJoinRoom(SceneType sceneType)
    {
        if (sceneType == SceneType.PracticeRoom && roomInfo.IsPracticeRoomFull)
        {
            PhotonMessageSystem.Instance.ShowMessage("Practice room is full!");
            practiceToggle.isOn = false;
            return;
        }

        if (sceneType == SceneType.SpectatorRoom && roomInfo.IsSpectatorRoomFull)
        {
            PhotonMessageSystem.Instance.ShowMessage("Spectator room is full!");
            spectatorToggle.isOn = false;
            return;
        }

        string roomName = sceneType == SceneType.PracticeRoom ?
            roomInfo.PracticeRoomName : roomInfo.SpectatorRoomName;

        PhotonNetworkManager.Instance.JoinSession(roomName, sceneType);
    }
}