using Discord.Commands;

namespace Orikivo
{
    public class InfoMatch
    {
        public InfoMatch(ModuleInfo module)
        {
            Type = Check.NotNull(module.Group) ? InfoType.Group : InfoType.Module;
            Name = module.Name;
        }

        public InfoMatch(CommandInfo command)
        {
            Type = InfoType.Command;
            Name = command.Name;
        }

        public InfoType Type { get; }
        public string Name { get; }
    }
}
