using System;

namespace Orikivo
{
    // find a better name, or replace Discord.Net's version.
    // Since discord.net doesn't allow you to set it on a property.
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AliasesAttribute : Attribute
    {
        public AliasesAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }

        public string[] Aliases { get; }
    }
}
