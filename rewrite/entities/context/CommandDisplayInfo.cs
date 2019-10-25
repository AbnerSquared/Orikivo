using Discord.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class CommandDisplayInfo : IDisplayInfo
    {
        internal CommandDisplayInfo(CommandInfo command, List<ContextValue> family = null)
            : this(command.AsList(), family) { }

        public CommandDisplayInfo(List<CommandInfo> commands, List<ContextValue> family = null)
        {
            CommandInfo command = commands.OrderBy(x => x.Priority).First();
            Name = command.Name;
            Aliases = ContextUtils.GetAliases(command);
            IsInGroup = false;
            if (Checks.NotNull(command.Module.Group))
            {
                IsInGroup = true;
                GroupName = command.Module.Group;
                if (string.IsNullOrWhiteSpace(Name))
                    Name = "";
            }

            Summary = command.Summary;

            if (Checks.NotNull(family))
                family?.Add(new ContextValue(command));

            Family = family ?? ContextUtils.GetFamilyTree(command);

            Overloads = commands.Select(x => new OverloadDisplayInfo(x, Family)).ToList();
        }

        public ContextInfoType Type => ContextInfoType.Command;
        
        /// <summary>
        /// The value used to find the command matching this object.
        /// </summary>
        public string Id => ContextUtils.ConcatFamilyTree(Family);

        public string Name { get; }

        public List<string> Aliases { get; }
        
        public string Summary { get; }

        public List<IReport> Reports { get; }

        public List<ContextValue> Family { get; }

        public string ModuleName { get; }

        public string GroupName { get; }

        public bool IsInGroup { get; }

        public string BlockName => $"`{Name}{(HasMultiple ? $"+{Overloads.Count - 1}" : "")}`";
        
        public List<OverloadDisplayInfo> Overloads { get; }

        public OverloadDisplayInfo Default => HasMultiple ? Overloads.OrderBy(x => x.Priority).First() : Overloads.First();

        public bool HasMultiple => Overloads.Count > 1;

        public string Content => ContextUtils.WriteDisplayContent(this);

        public static explicit operator CommandDisplayInfo(CommandInfo info)
            => new CommandDisplayInfo(info);
    }
}
