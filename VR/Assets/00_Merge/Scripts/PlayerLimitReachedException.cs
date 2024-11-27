using System;

public class PlayerLimitReachedException : Exception
{
    public PlayerLimitReachedException() : base("Player limit reached for this room.") { }
    public PlayerLimitReachedException(string message) : base(message) { }
}