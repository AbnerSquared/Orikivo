using System;

namespace Arcadia.Multiplayer
{
    // [Action("start_day")]
    // This attribute is used to mark a method as a game action
    public class ActionAttribute : Attribute
    {
        // Name
        // UpdateOnExecute
        // 
    }

    /// <summary>
    /// Marks a command as a session, which prevents other sessions from initializing until the existing session ends.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SessionAttribute : Attribute { }

    /// <summary>
    /// Marks a command to require the specified user to not be in any sessions to execute.
    /// </summary>
    public class RequireNoSessionAttribute : Attribute { }
}