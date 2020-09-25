using System;

namespace Arcadia.Multiplayer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GamePropertyAttribute : Attribute
    {
        public GamePropertyAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; internal set; }
    }
}