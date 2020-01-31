using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // $"Custom commands built by members within {GuildName}.";
    public class CommandNode : ContextNode
    {
        public CommandNode(CommandInfo command) : base(command)
        {
            Parent = command.Module.Name; // Use Family to get the full name
            Group = command.Module.Group; // Use Family to get the full group
            MainSummary = command.Attributes.GetAttribute<BatchSummaryAttribute>()?.Summary;

            Overloads = new List<OverloadNode> { new OverloadNode(command) };
        }

        public CommandNode(IEnumerable<CommandInfo> commands) : base(commands.OrderBy(x => x.Priority).First())
        {
            Overloads = commands.Select(c => new OverloadNode(c)).ToList();

            Parent = Default.Parent; // Use Family to get the full name
            Group = Default.Group; // Use Family to get the full group
            MainSummary = commands
                .FirstOrDefault(c => c.Attributes.GetAttribute<BatchSummaryAttribute>() != null)
                ?.Attributes.GetAttribute<BatchSummaryAttribute>()?.Summary;
            // maybe check to see if all of their names line up
        }

        // the parent of the command.
        public string Parent { get; protected set; }
        public string Group { get; protected set; }


        // this is the main summary of the command, overshadowing the cluster of overloads
        public string MainSummary { get; protected set; }
        public List<OverloadNode> Overloads { get; protected set; }

        public OverloadNode Default => Overloads.Count > 1 ? Overloads
            .OrderBy(x => x.Index)
            .First() :
            Overloads.First();

        public override InfoType Type => InfoType.Command;
        protected override string Formatting
        {
            get
            {
                if (Overloads.Count > 1)
                {
                    StringBuilder format = new StringBuilder();

                    // Group
                    if (Checks.NotNull(Group))
                    {
                        if (!Checks.NotNull(Name))
                        {
                            format.Append(Format.Bold(Group));

                        }
                        else
                        {
                            format.Append(Group);
                            format.Append(' ');
                        }
                    }
                    // Name
                    if (Checks.NotNull(Name))
                        format.Append(Format.Bold(Name));

                    format.AppendLine();

                    // Main summary
                    if (Checks.NotNull(MainSummary))
                    {
                        format.Append("⇛ ");
                        format.Append(MainSummary);
                        format.AppendLine();
                    }
                    // Write overloads
                    format.AppendJoin("\n", Overloads.OrderBy(o => o.Index).Select(o => $"> {o.Syntax}"));
                    format.AppendLine();

                    // Write ID (DEBUG)
                    if (Checks.NotNull(Id))
                    {
                        format.AppendLine();
                        format.Append($"**ID**: `{Id}`");
                    }

                    return format.ToString();
                }
                else
                {
                    return Default.ToString();
                }
            }
        }
    }
}
