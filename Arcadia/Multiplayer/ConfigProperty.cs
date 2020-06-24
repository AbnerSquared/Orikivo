using System;

namespace Arcadia
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

        public string Name { get; set; }
        public string Summary { get; set; }

        public Func<object, bool> Validation { get; set; }
    }

}
