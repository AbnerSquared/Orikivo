using Discord.Commands;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic context from the command family.
    /// </summary>
    public class ContextValue
    {
        public ContextValue(GuildCommand custom)
        {
            Name = custom.Name;
            Type = ContextInfoType.Custom;
        }

        public ContextValue(ModuleInfo module)
        {
            if (Checks.NotNull(module.Group))
            {
                Name = module.Group;
                Type = ContextInfoType.Group;
            }
            else
            {
                Name = module.Name;
                Type = ContextInfoType.Module;
            }
        }

        public ContextValue(CommandInfo command)
        {
            Name = command.Name;
            Type = ContextInfoType.Command;
        }

        public ContextValue(ParameterInfo parameter)
        {
            Name = parameter.Name;
            Type = ContextInfoType.Parameter;
        }

        public string Name { get; }

        public ContextInfoType Type { get; }

        public int Depth { get; internal set; }

        public static bool operator ==(ContextValue valueA, ContextValue valueB)
        {
            return valueA.Name == valueB.Name && valueA.Type == valueB.Type;
        }

        public static bool operator !=(ContextValue valueA, ContextValue valueB)
        {
            return !(valueA == valueB);
        }

        public override string ToString()
            => $"{Name}: {Type}";
    }
}
