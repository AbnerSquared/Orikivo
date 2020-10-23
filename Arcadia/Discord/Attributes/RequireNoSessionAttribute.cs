using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Marks a command to require the specified user to not be in any sessions to execute.
    /// </summary>
    public class RequireNoSessionAttribute : Attribute { }
}