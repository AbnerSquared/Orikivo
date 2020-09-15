using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents an option for a <see cref="GameBase"/>.
    /// </summary>
    public class GameOption : GameProperty
    {
        public static GameOption Create<T>(string id, string name, T defaultValue, string summary = "", Func<object, bool> validation = null)
        {
            return new GameOption
            {
                Id = id,
                Name = name,
                Summary = summary,
                Value = defaultValue,
                DefaultValue = defaultValue,
                ValueType = typeof(T),
                Validation = validation
            };
        }

        /// <summary>
        /// Represents the display name of this <see cref="GameOption"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the display summary of this <see cref="GameOption"/>.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Represents an optional validator for the value about to be set.
        /// </summary>
        public Func<object, bool> Validation { get; set; }
    }

}
