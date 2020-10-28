using System;

namespace Arcadia
{
    /// <summary>
    /// Defines a collection of triggers for criteria judgement.
    /// </summary>
    [Flags]
    public enum CriteriaTriggers
    {
        /// <summary>
        /// Marks a criteria judgement to execute after a command execution.
        /// </summary>
        Command = 1,

        /// <summary>
        /// Marks a criteria judgement to execute after a game session.
        /// </summary>
        Game = 2
    }
}