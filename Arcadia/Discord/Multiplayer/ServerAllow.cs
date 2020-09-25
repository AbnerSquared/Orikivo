using System;

namespace Arcadia.Multiplayer
{
    [Flags]
    public enum ServerAllow
    {
        Spectate = 1,
        Chat = 2,
        Invite = 4
    }
}