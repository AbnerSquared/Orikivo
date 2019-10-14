namespace Orikivo
{
    /// <summary>
    /// An object defining the user that executed a command along with its result.
    /// </summary>
    public class GameAction
    {
        internal GameAction(ulong userId, TriggerResult trigger)
        {
            UserId = userId;
            TriggerUsed = trigger;
        }

        public ulong UserId { get; }
        public TriggerResult TriggerUsed { get; }
    }


}
