﻿namespace Orikivo
{
    /// <summary>
    /// Defines a type of cooldown to be used or called.
    /// </summary>
    public enum CooldownType
    {
        /// <summary>
        /// Marks the cooldown to be for a command.
        /// </summary>
        Command = 1,

        // EXAMPLE: user spams too much = 5 second global command cooldown.
        /// <summary>
        /// Marks the cooldown as a global cooldown.
        /// </summary>
        Global = 2
    }
}
