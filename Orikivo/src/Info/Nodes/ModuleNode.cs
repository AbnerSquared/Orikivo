using Discord.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class ModuleNode : ContextNode
    {
        public ModuleNode(ModuleInfo module) : base(module)
        {
            Group = module.Group;
            Icon = module.Attributes.FirstOrDefault<IconAttribute>()?.Icon;

            Submodules = module.Submodules.Select(s => new ModuleNode(s)).ToList();

            var commandGroups = new Dictionary<string, List<CommandInfo>>();

            foreach (CommandInfo command in module.Commands)
            {
                if (!commandGroups.TryAdd(command.Name, command.AsList()))
                    commandGroups[command.Name].Add(command);
            }

            Commands = commandGroups.Select(x => new CommandNode(x.Value)).ToList();
        }

        public override InfoType Type => InfoType.Module;

        public string Group { get; protected set; }

        public string Icon { get; protected set; }

        public int CommandCount => Submodules.Sum(x => x.CommandCount) + Commands.Count;

        public int TotalCommandCount => CommandCount + Submodules.Count(x => x.IsGroup);

        public bool IsGroup => Check.NotNull(Group);

        public List<ModuleNode> Submodules { get; protected set; }

        public List<CommandNode> Commands { get; protected set; }
    }
}
