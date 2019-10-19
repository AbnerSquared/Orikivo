namespace Orikivo
{
    /// <summary>
    /// An object defining the user that executed a command along with its result.
    /// </summary>
    public class GameAction
    {
        internal GameAction(ulong userId, GameTriggerResult trigger)
        {
            UserId = userId;
            TriggerUsed = trigger;
        }

        public ulong UserId { get; }
        public GameTriggerResult TriggerUsed { get; }
    }


}
