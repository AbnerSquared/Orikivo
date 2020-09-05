namespace Orikivo.Desync
{
    // TODO: determine how a dialogue continues to branch
    // TODO: Create DialogueMatchAction, which handles the entire communication branching.

    /// <summary>
    /// Represents the way a <see cref="Dialog"/> entry is handled.
    /// </summary>
    public enum DialogType
    {
        /// <summary>
        /// Represents a starting topic for a conversation.
        /// </summary>
        Initial = 1,

        /// <summary>
        /// Closes a conversation.
        /// </summary>
        End = 2,

        /// <summary>
        /// Represents a continuation of a current conversation branch.
        /// </summary>
        Reply = 3,

        /// <summary>
        /// Represents the end of a conversation branch, returning to all available <see cref="Initial"/> dialogue branches.
        /// </summary>
        Answer = 4,

        /// <summary>
        /// Represents the start of a conversation topic. This usually leads to new conversation routes.
        /// </summary>
        Question = 5
    }

}
