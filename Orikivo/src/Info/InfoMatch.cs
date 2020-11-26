using Discord.Commands;

namespace Orikivo
{
    public class InfoMatch
    {
        public InfoMatch(ModuleInfo module)
        {
            Type = Check.NotNull(module.Group) ? InfoType.Group : InfoType.Module;
            Name = module.Name;
            Id = ContextNode.GetId(module);
        }

        public InfoMatch(CommandInfo command)
        {
            Type = InfoType.Command;
            Name = command.Name;
            Id = ContextNode.GetId(command);
        }

        public InfoType Type { get; }

        public string Name { get; }

        public string Id { get; }
    }
}
