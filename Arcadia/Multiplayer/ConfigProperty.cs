using System;

namespace Arcadia.Multiplayer
{
    public class ConfigProperty : GameProperty
    {
        public static ConfigProperty Create<T>(string id, string name, T defaultValue, string summary = "")
        {
            return new ConfigProperty
            {
                Id = id,
                Name = name,
                Summary = summary,
                Value = defaultValue,
                DefaultValue = defaultValue,
                ValueType = typeof(T)
            };
        }

        /// <summary>
        /// Represents the display name of this <see cref="ConfigProperty"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the display summary of this <see cref="ConfigProperty"/>.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Represents an optional validator for the value about to be set.
        /// </summary>
        public Func<object, bool> Validation { get; set; }
    }

}
