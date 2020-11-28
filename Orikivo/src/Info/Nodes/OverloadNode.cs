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
        private readonly CommandInfo _command;
        public OverloadNode(CommandInfo command) : base(command, true)
        {
            Parent = command.Module.Name;
            Group = command.Module.Group;
            Index = command.Priority;
            Count = command.Module.Commands.Count(x => x.Name == command.Name);
            Cooldown = command.Attributes.FirstOrDefault<CooldownAttribute>()?.Duration;
            // Access = command.Attributes.FirstOrDefault<AccessAttribute>()?.Level;
            //Permissions = ... // This could probably use RequireBotPermissions and RequireUserPermissions
            Parameters = command.Parameters.Select(p => new ParameterNode(p)).ToList();
            _command = command;
        }

        public bool CanExecute(ICommandContext ctx)
            => _command.CheckPreconditionsAsync(ctx).ConfigureAwait(false).GetAwaiter().GetResult().IsSuccess;

        public string Parent { get; protected set; }

        public string Group { get; protected set; }

        public int Index { get; protected set; }

        public int Count { get; protected set; }

        internal string Example { get; set; }

        public TimeSpan? Cooldown { get; protected set; }

        public AccessLevel? Access { get; protected set; }

        public GuildPermission? Permissions { get; protected set; }

        public List<ParameterNode> Parameters { get; protected set; }

        public override InfoType Type => InfoType.Overload;

        // TODO: Move over to InfoFormatter
        internal string Syntax
        {
            get
            {
                var format = new StringBuilder();

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
    }
}
