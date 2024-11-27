// RoomInfo.cs
using Fusion;

public struct LinkedRoomInfo
{
    public string BaseRoomName;
    public string PracticeRoomName;
    public string SpectatorRoomName;
}

public struct LinkedSessionInfo
{
    public string BaseRoomName;
    public SessionInfo PracticeSession;
    public SessionInfo SpectatorSession;
}

public struct PhotonRoomButtonInfo
{
    public string RoomName;
    public string PracticeRoomName;
    public string SpectatorRoomName;
    public int PracticeCurrentPlayers;
    public int SpectatorCurrentPlayers;
    public bool HasPracticeRoom;
    public bool HasSpectatorRoom;
    public bool IsPracticeRoomFull;
    public bool IsSpectatorRoomFull;
}