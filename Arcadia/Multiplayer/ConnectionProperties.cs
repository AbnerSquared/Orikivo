using System;
using System.Collections.Generic;

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
            BlockInput = false,
            State = GameState.Waiting,
            Frequency = 0,
            Origin = OriginType.Unknown
        };

        // The default frequency to set this connection to
        public int Frequency { get; set; } = 0;

        // The game state that should be set for this connection
        public GameState State { get; set; } = GameState.Waiting;

        public OriginType Origin { get; set; } = OriginType.Unknown;

        // If this connection can delete messages
        public bool CanDeleteMessages { get; set; } = false;

        // If this connection should ignore input
        public bool BlockInput { get; set; } = false;

        // After 4 messages is sent that CANNOT be deleted, this screen is refreshed, which resends the content into
        // a new message body

        // The size of the auto refresh counter, used to handle auto-refreshing
        public int AutoRefreshCounter { get; set; } = 4;

        // The default value for this connection REMOVE
        public DisplayContent ContentOverride { get; set; } = null;

        public List<IInput> Inputs { get; set; } = new List<IInput>();

        public TimeSpan RefreshRate { get; set; } = TimeSpan.FromSeconds(1);
    }
}
