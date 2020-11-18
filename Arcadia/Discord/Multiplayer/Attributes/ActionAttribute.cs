using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Marks the specified method as a <see cref="GameAction"/> for a <see cref="GameBase"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(string id, bool updateOnExecute = true)
        {
            Id = id;
            UpdateOnExecute = updateOnExecute;
        }

        // Represents the Id of the game action
        public string Id { get; internal set; }

        // Determines if the GameServer updates all connected displays when this action is executed
        public bool UpdateOnExecute { get; internal set; }
    }
}
