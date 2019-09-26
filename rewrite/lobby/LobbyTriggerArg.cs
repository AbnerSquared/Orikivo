using System;

namespace Orikivo
{
    // an argument within a lobby trigger.
    public class LobbyTriggerArg
    {
        public LobbyTriggerArg(string name, Type type, bool optional = false)
        {
            Name = name;
            Type = type;
            Optional = optional;
        }

        public Type Type { get; }
        public string Name { get; }
        public bool Optional { get; }
    }
}
