using Discord.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class CommandNode : ContextNode
    {
        public CommandNode(CommandInfo command) : base(command)
        {
            Parent = command.Module.Name;
            Group = command.Module.Group;
            MainSummary = command.Attributes.OfType<BaseSummaryAttribute>().FirstOrDefault()?.Summary;

            Overloads = new List<OverloadNode> { new OverloadNode(command) };
        }

        public CommandNode(IEnumerable<CommandInfo> commands) : base(commands.OrderBy(x => x.Priority).First())
        {
            Overloads = commands.Select(c => new OverloadNode(c)).ToList();

            Parent = Default.Parent;
            Group = Default.Group;
            MainSummary = commands
                .FirstOrDefault(c => c.Attributes.OfType<BaseSummaryAttribute>().Any())
                ?.Attributes.OfType<BaseSummaryAttribute>().FirstOrDefault()?.Summary;
        }

        public string Parent { get; protected set; }

        public string Group { get; protected set; }

        public string MainSummary { get; protected set; }

        public List<OverloadNode> Overloads { get; protected set; }

        public OverloadNode Default => Overloads.Count > 1 ? Overloads
            .OrderBy(x => x.Index)
            .First() :
            Overloads.First();

        public override InfoType Type => InfoType.Command;
    }
}
