// SharedSessionManager.cs
using System.Collections.Generic;
using UnityEngine;

public class SharedSessionManager : MonoBehaviour
{
	public static SharedSessionManager Instance { get; private set; }

	private Dictionary<string, LinkedRoomInfo> linkedRooms = new Dictionary<string, LinkedRoomInfo>();

	private void Awake()
    {
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
        }
        else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void RegisterLinkedRooms(string baseRoomName, string practiceRoomName, string spectatorRoomName)
	{
		linkedRooms[baseRoomName] = new LinkedRoomInfo
		{
			BaseRoomName = baseRoomName,
			PracticeRoomName = practiceRoomName,
			SpectatorRoomName = spectatorRoomName
		};
	}

	public LinkedRoomInfo GetLinkedRooms(string baseRoomName)
	{
		if (linkedRooms.TryGetValue(baseRoomName, out LinkedRoomInfo info))
		{
			return info;
		}
		return default;
	}
}