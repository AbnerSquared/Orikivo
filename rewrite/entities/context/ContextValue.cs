using Discord.Commands;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic trigger.
    /// </summary>
    public class ContextValue
    {
        public ContextValue(ModuleInfo module)
        {
            if (module.Group != null)
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

        public string Name { get; internal set; }

        public ContextInfoType Type { get; internal set; }
    }
}
