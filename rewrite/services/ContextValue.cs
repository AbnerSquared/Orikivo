using Discord.Commands;

namespace Orikivo
{
    public class ContextValue
    {
        public ContextValue(ModuleInfo module)
        {
            Type = Checks.NotNull(module.Group) ? InfoType.Group : InfoType.Module;
            Name = module.Name;
        }

        public ContextValue(CommandInfo command)
        {
            Type = InfoType.Command;
            Name = command.Name;
        }

        public InfoType Type { get; }
        public string Name { get; }
    }
}
