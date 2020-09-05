namespace Orikivo.Desync
{
    public enum CharacterState
    {
        // the character is hanging around; free to interact
        Idle = 1,

        // the character is doing something; cannot interact with players
        Busy = 2,

        // the character is hidden; cannot be seen at this location.
        Hidden = 4
    }
}
