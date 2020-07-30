namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the properties of a <see cref="ServerConnection"/>.
    /// </summary>
    public class ConnectionProperties
    {
        public static readonly ConnectionProperties Default = new ConnectionProperties
        {
            AutoRefreshCounter = 4,
            CanDeleteMessages = false,
            State = GameState.Waiting,
            Frequency = 0
        };

        public int Frequency { get; set; } = 0;

        public GameState State { get; set; } = GameState.Waiting;

        public bool CanDeleteMessages { get; set; } = false;

        // After 4 messages is sent that CANNOT be deleted, this screen is refreshed, which resends the content into
        // a new message body
        public int AutoRefreshCounter { get; set; } = 4;
    }
}