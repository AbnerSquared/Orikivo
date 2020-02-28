using System;

namespace Orikivo
{
    [Flags]
    public enum VoiceFlag
    {
        Muted = 1,
        SelfMuted = 2,
        Deafened = 4,
        SelfDeafened = 8,
        Suppressed = 16
    }
}