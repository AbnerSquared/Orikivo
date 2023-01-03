using System;

namespace Arcadia
{
    /// <summary>
    /// Defines a collection of triggers for criterion judgement.
    /// </summary>
    [Flags]
    public enum CriterionTriggers
    {
        /// <summary>
        /// Marks a criterion judgement to execute after a command execution.
        /// </summary>
        Command = 1,

        /// <summary>
        /// Marks a criterion judgement to execute after a game session.
        /// </summary>
        Game = 2
    }
}
