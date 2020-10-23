using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Marks a command as a session, which prevents other sessions from initializing until the existing session ends.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SessionAttribute : Attribute { }
}