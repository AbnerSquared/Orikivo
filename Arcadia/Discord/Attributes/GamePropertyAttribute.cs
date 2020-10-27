using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Marks the specified property as a <see cref="GameProperty"/> for a <see cref="GameBase"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Represents the unique identifier for a <see cref="GameProperty"/>.
        /// </summary>
        public string Id { get; internal set; }
    }
}
