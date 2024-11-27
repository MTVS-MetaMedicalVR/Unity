// PhotonRoomManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;
using System.Collections.Generic;

public class PhotonRoomManager : MonoBehaviour
{
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject PhotonRoomButtonPrefab;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Toggle createRoomToggle;  // 버튼에서 토글로 변경

    private Dictionary<string, GameObject> PhotonRoomButtons = new Dictionary<string, GameObject>();
    private Dictionary<string, LinkedSessionInfo> roomInfos = new Dictionary<string, LinkedSessionInfo>();

    private void Start()
    {
        // 토글 이벤트 리스너로 변경
        createRoomToggle.onValueChanged.AddListener(OnCreateRoomToggled);

    }
    private void OnCreateRoomToggled(bool isOn)
    {
        if (isOn)
        {
            if (string.IsNullOrEmpty(roomNameInput.text))
            {
                PhotonMessageSystem.Instance.ShowMessage("Please enter a room name!");
                createRoomToggle.isOn = false;
                return;
            }
            PhotonNetworkManager.Instance.CreateSession(roomNameInput.text);
        }
    }
    private void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            PhotonMessageSystem.Instance.ShowMessage("Please enter a room name!");
            return;
        }
        PhotonNetworkManager.Instance.CreateSession(roomNameInput.text);
    }

    public void UpdateRoomList(List<SessionInfo> sessionList)
    {
        ProcessSessionList(sessionList);
        UpdateRoomListUI();
    }

    private void ProcessSessionList(List<SessionInfo> sessionList)
    {
        roomInfos.Clear();

        foreach (var session in sessionList)
        {
            string baseRoomName = ExtractBaseRoomName(session.Name);
            bool isPracticeRoom = IsPracticeRoom(session.Name);

            if (!roomInfos.ContainsKey(baseRoomName))
            {
                roomInfos.Add(baseRoomName, new LinkedSessionInfo
                {
                    BaseRoomName = baseRoomName,
                    PracticeSession = null,
                    SpectatorSession = null
                });
            }

            var currentInfo = roomInfos[baseRoomName];
            if (isPracticeRoom)
            {
                currentInfo.PracticeSession = session;
            }
            else
            {
                currentInfo.SpectatorSession = session;
            }
            roomInfos[baseRoomName] = currentInfo;
        }
    }

    private void UpdateRoomListUI()
    {
        foreach (var button in PhotonRoomButtons.Values)
        {
            Destroy(button);
        }
        PhotonRoomButtons.Clear();

        foreach (var roomInfo in roomInfos.Values)
        {
            CreatePhotonRoomButton(roomInfo);
        }
    }

    private void CreatePhotonRoomButton(LinkedSessionInfo roomInfo)
    {
        GameObject buttonObj = Instantiate(PhotonRoomButtonPrefab, roomListContent);
        PhotonRoomButton PhotonRoomButton = buttonObj.GetComponent<PhotonRoomButton>();

        int practiceCount = roomInfo.PracticeSession?.PlayerCount ?? 0;
        int spectatorCount = roomInfo.SpectatorSession?.PlayerCount ?? 0;

        PhotonRoomButton.Initialize(new PhotonRoomButtonInfo
        {
            RoomName = roomInfo.BaseRoomName,
            PracticeRoomName = roomInfo.PracticeSession?.Name,
            SpectatorRoomName = roomInfo.SpectatorSession?.Name,
            PracticeCurrentPlayers = practiceCount,
            SpectatorCurrentPlayers = spectatorCount,
            HasPracticeRoom = roomInfo.PracticeSession != null,
            HasSpectatorRoom = roomInfo.SpectatorSession != null,
            IsPracticeRoomFull = practiceCount >= RoomSettings.PRACTICE_ROOM_MAX_PLAYERS,
            IsSpectatorRoomFull = spectatorCount >= RoomSettings.SPECTATOR_ROOM_MAX_PLAYERS
        });

        PhotonRoomButtons[roomInfo.BaseRoomName] = buttonObj;
    }

    private string ExtractBaseRoomName(string fullRoomName)
    {
        if (fullRoomName.EndsWith("_Practice"))
            return fullRoomName.Substring(0, fullRoomName.Length - 9);
        if (fullRoomName.EndsWith("_Spectator"))
            return fullRoomName.Substring(0, fullRoomName.Length - 10);
        return fullRoomName;
    }

    private bool IsPracticeRoom(string roomName)
    {
        return roomName.EndsWith("_Practice");
    }
}