namespace Arcadia.Multiplayer
{
    public class ServerInvite
    {
        public ServerInvite(ulong userId, string description)
        {
            UserId = userId;
            Description = description;
        }

        // who is available to join this game lobby?
        public ulong UserId { get; }

        // What does the invite say?
        public string Description { get; }
    }
}
