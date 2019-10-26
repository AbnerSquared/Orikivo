using Discord.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // TODO: Figure out how to insert report information.
    // TODO: Create Subtitle field editor. (Remarks)??
    /// <summary>
    /// A simplified module information container.
    /// </summary>
    public class ModuleDisplayInfo : IDisplayInfo
    {
        internal ModuleDisplayInfo(ModuleInfo module, List<ContextValue> family = null)
        {
            Name = module.Name;
            Subtitle = module.Attributes.GetAttribute<SubtitleAttribute>()?.Subtitle;
            Summary = module.Summary;
            GroupName = module.Group;
            Aliases = module.Aliases.Where(x => x != Name).ToList();

            if (Checks.NotNull(family))
                family.Add(new ContextValue(module));
            Family = family ?? ContextUtils.GetFamilyTree(module);

            Submodules = module.Submodules.Select(x => new ModuleDisplayInfo(x, Family)).ToList();
            Dictionary<string, List<CommandInfo>> commandGroups = new Dictionary<string, List<CommandInfo>>();

            foreach (CommandInfo command in module.Commands)
            {
                if (!commandGroups.TryAdd(command.Name, command.AsList()))
                    commandGroups[command.Name].Add(command);
            }

            Commands = commandGroups.Select(x => new CommandDisplayInfo(x.Value, Family)).ToList();
        }

        public string Id => ContextUtils.ConcatFamilyTree(Family);

        public ContextInfoType Type => ContextInfoType.Module;

        public List<IReport> Reports { get; }

        public List<ContextValue> Family { get; }

        public string Name { get; }

        public string GroupName { get; }

        public List<string> Aliases { get; }

        public string BlockName => $"`{Name}`{(IsGroup ? "**\\***":"")}";

        public string Subtitle { get; }

        public string Summary { get; }
        
        public List<CommandDisplayInfo> Commands { get; }

        public List<ModuleDisplayInfo> Submodules { get; }

        public int TotalCommands => Submodules.Sum(x => x.TotalCommands) + Commands.Count;

        public bool IsGroup => Checks.NotNull(GroupName);

        public string Content => ContextUtils.WriteDisplayContent(this);
    }
}
