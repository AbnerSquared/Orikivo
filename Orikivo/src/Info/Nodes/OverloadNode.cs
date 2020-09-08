using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class OverloadNode : ContextNode
    {
        public OverloadNode(CommandInfo command) : base(command, true)
        {
            Parent = command.Module.Name; // Use Family to get the full name
            Group = command.Module.Group; // Use Family to get the full group
            Index = command.Priority;
            Count = command.Module.Commands.Count(x => x.Name == command.Name); // You could probably use command.Module to get all of the other ones
            Cooldown = command.Attributes.FirstOrDefault<CooldownAttribute>()?.Duration;
            Access = command.Attributes.FirstOrDefault<AccessAttribute>()?.Level;
            //Permissions = ... // This could probably use RequireBotPermissions and RequireUserPermissions
            Parameters = command.Parameters.Select(p => new ParameterNode(p)).ToList();
        }

        public string Parent { get; protected set; }
        public string Group { get; protected set; }

        public int Index { get; protected set; }
        public int Count { get; protected set; } // the amount of total overloads existing

        internal string Example { get; set; }
        public TimeSpan? Cooldown { get; protected set; }
        public AccessLevel? Access { get; protected set; }
        public GuildPermission? Permissions { get; protected set; }

        public List<OptionNode> Options { get; protected set; }

        // bool? RequireUser RequireGuild RequireHusk

        public List<ParameterNode> Parameters { get; protected set; }

        public override InfoType Type => InfoType.Overload;

        internal string FullName
        {
            get
            {
                StringBuilder format = new StringBuilder();

                if (Check.NotNull(Group))
                {
                    format.Append(Group);

                    if (Check.NotNull(Name))
                        format.Append(' ');
                }

                if (Check.NotNull(Name))
                    format.Append(Name);

                return format.ToString();
            }
        }
        internal string Syntax
        {
            get
            {
                StringBuilder format = new StringBuilder();

                if (Check.NotNull(Group))
                {
                    if (!Check.NotNull(Name))
                    {
                        format.Append(Discord.Format.Bold(Group));

                    }
                    else
                    {
                        format.Append(Group);
                        format.Append(' ');
                    }
                }

                if (Check.NotNull(Name))
                    format.Append(Discord.Format.Bold(Name));

                // parameters
                format.Append('(');

                if (Parameters.Count > 0)
                {
                    // implement Parameters
                    format.AppendJoin(", ", Parameters.Select(x => x.Syntax));
                }

                format.Append(')');

                // index marker if more than 1
                if (Count > 1)
                {
                    format.Append(' ');
                    format.Append(Format.Subscript($"+{Index}"));
                }

                return format.ToString();
            }
        }

        protected override string Formatting
        {
            get
            {
                StringBuilder format = new StringBuilder();

                // page bar, if more than 1
                if (Count > 1)
                {
                    format.Append('▰', Index + 1);
                    format.Append('▱', Count - (Index + 1));
                    format.AppendLine();
                }

                // grouping
                if (Check.NotNull(Group))
                {
                    if (!Check.NotNull(Name))
                    {
                        format.Append(Discord.Format.Bold(Group));
                    }
                    else
                    {
                        format.Append(Group);
                        format.Append(' ');
                    }
                }

                if (Check.NotNull(Name))
                    format.Append(Discord.Format.Bold(Name));

                // parameters
                format.Append('(');

                if (Parameters.Count > 0)
                {
                    // implement Parameters
                    format.AppendJoin(", ", Parameters.Select(x => x.Syntax));
                }

                format.Append(')');

                // index marker if more than 1
                if (Count > 1)
                {
                    format.Append(' ');
                    format.Append(Format.Subscript($"+{Index}"));
                }

                // end of line 2
                format.AppendLine();

                // summary

                if (Check.NotNull(Summary))
                    format.AppendLine($"⇛ {Summary}");

                format.AppendLine();

                if (Aliases.Count > 1)
                {
                    format.Append("> **Aliases**: ");
                    format.AppendJoin(", ",
                        (Check.NotNull(Group) ? Aliases.Select(x => x.Substring(Group.Length + 1)) : Aliases)
                        .Where(x => x != Name)
                        .OrderByDescending(x => x.Length)
                        .Select(x => $"`{x}`"));
                    format.AppendLine();
                }

                if (Access.HasValue)
                    format.AppendLine($"> **Access**: **{Access.Value.ToString()}**");

                if (Cooldown.HasValue)
                    format.AppendLine($"> **Cooldown**: {Format.LongCounter(Cooldown.Value)}");

                if (Check.NotNull(Example))
                    format.Append($"> **Example**: `{Example}`");

                return format.ToString();
            }
        }
    }
}
