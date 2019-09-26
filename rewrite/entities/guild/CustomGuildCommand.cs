using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    // the custom command system should only allow
    // displaying information
    // or displaying something from which can mention a user
    public class CustomGuildCommand
    {
        [JsonConstructor]
        internal CustomGuildCommand(string name, List<string> aliases, CustomCommandMessage result)
        {
            Name = name;
            Aliases = aliases ?? new List<string>();
            Message = result;
        }

        public CustomGuildCommand(string name, params string[] aliases)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("The name for a custom command cannot be empty.");
            Name = name;
            Aliases = new List<string>();
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("aliases")]
        public List<string> Aliases { get; set; } // it cannot match any existing aliases

        [JsonProperty("message")]
        public CustomCommandMessage Message { get; set; }

        // in short custom commands should only be bodies of text or images
    }
}
