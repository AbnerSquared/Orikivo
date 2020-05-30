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
            Count = command.Module.Commands.Where(x => x.Name == command.Name).Count(); // You could probably use command.Module to get all of the other ones
            Cooldown = command.Attributes.FirstAttribute<CooldownAttribute>()?.Duration;
            Access = command.Attributes.FirstAttribute<AccessAttribute>()?.Level;
            //Permissions = ... // This could probably use RequireBotPermissions and RequireUserPermissions
            Parameters = command.Parameters.Select(p => new ParameterNode(p)).ToList();
        }

        public string Parent { get; protected set; }
        public string Group { get; protected set; }

        public int Index { get; protected set; }
        public int Count { get; protected set; } // the amount of total overloads existing

        public TimeSpan? Cooldown { get; protected set; }
        public AccessLevel? Access { get; protected set; }
        public GuildPermission? Permissions { get; protected set; }

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
                        format.Append(Format.Bold(Group));

                    }
                    else
                    {
                        format.Append(Group);
                        format.Append(' ');
                    }
                }

                if (Check.NotNull(Name))
                    format.Append(Format.Bold(Name));

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
                    format.Append(OriFormat.Subscript($"+{Index}"));
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
                        format.Append(Format.Bold(Group));
                        
                    }
                    else
                    {
                        format.Append(Group);
                        format.Append(' ');
                    }
                }

                if (Check.NotNull(Name))
                {
                    // name
                    if (Aliases.Count > 1)
                    {
                        format.Append('[');
                        // initial name
                        format.Append(Format.Bold(Name));
                        format.Append(", ");


                        // extra aliases AFTER main name


                        if (Check.NotNull(Group))
                        {
                            format.AppendJoin(", ", Aliases.Select(x => x.Substring(Group.Length + 1)).Where(x => x != Name).OrderBy(x => x.Length));
                        }
                        else
                        {
                            //format.Append(Format.Bold(Name));
                            //format.Append(", ");

                            format.AppendJoin(", ", Aliases.Where(x => x != Name)
                                .OrderBy(x => x.Length));
                        }

                        format.Append(']');
                    }
                    else
                    {
                        format.Append(Format.Bold(Name));
                    }
                }

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
                    format.Append(OriFormat.Subscript($"+{Index}"));
                }

                // end of line 2
                format.AppendLine();

                // summary

                if (Check.NotNull(Summary))
                {
                    format.Append("⇛ ");
                    format.Append(Summary);
                    format.AppendLine();
                }

                // all attributes that the overload might have.

                // access
                if (Access.HasValue)
                {
                    format.Append("> ");
                    format.Append("**Access**: ");
                    format.Append($"**{Access.Value.ToString()}**");
                    format.AppendLine();
                }

                // cooldowns
                if (Cooldown.HasValue)
                {
                    format.Append("> ");
                    format.Append("**Cooldown**: ");
                    format.Append(OriFormat.GetShortTime(Cooldown.Value.TotalSeconds));
                    format.AppendLine();
                }

                // permissions

                // id
                if (Check.NotNull(Id))
                {
                    format.AppendLine();
                    format.Append($"**ID**: `{Id}`");
                }

                return format.ToString();
            }
        }
    }
}
