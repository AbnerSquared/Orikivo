using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Commands;

namespace Orikivo
{
    // TODO: Add preconditions?
    public class ParameterNode : ContextNode
    {
        private static ParameterTag GetTag(ParameterInfo parameter)
        {
            ParameterTag tag = 0;

            if (parameter.IsOptional)
                tag |= ParameterTag.Optional;

            if (parameter.IsRemainder)
                tag |= ParameterTag.Trailing;

            if (parameter.Type.IsEquivalentTo(typeof(IMentionable)))
                tag |= ParameterTag.Mentionable;

            return tag;
        }

        public ParameterNode(ParameterInfo parameter) : base(parameter)
        {
            var command = new StringBuilder();

            if (Check.NotNull(parameter.Command.Module.Group))
            {
                command.Append(parameter.Command.Module.Group);

                if (Check.NotNull(parameter.Command.Name))
                    command.Append(' ');
            }

            if (Check.NotNull(parameter.Command.Name))
                command.Append(parameter.Command.Name);

            Command = command.ToString();
            OverloadIndex = parameter.Command.Priority;
            OverloadCount = parameter.Command.Module.Commands.Count(x => x.Name == parameter.Command.Name);
            CommandId = GetId(parameter.Command, OverloadCount > 1);
            DefaultValue = parameter.DefaultValue;
            ValueType = parameter.Type;
            Tags = GetTag(parameter);

            ParseExamples = parameter.Attributes.FirstOrDefault<ExampleAttribute>()?.Examples;
        }

        public string Command { get; protected set; }

        public string CommandId { get; protected set; }

        public int OverloadIndex { get; protected set; }

        public int OverloadCount { get; protected set; }

        // TODO: Move over to InfoFormatter
        public string Syntax
        {
            get
            {
                var syntax = new StringBuilder();

                if (Tags.HasFlag(ParameterTag.Mentionable))
                    syntax.Append('@');

                syntax.Append(Name);

                if (Tags.HasFlag(ParameterTag.Trailing))
                {
                    syntax.Append("..");

                    if (!Tags.HasFlag(ParameterTag.Optional))
                        syntax.Append('.');
                }

                if (Tags.HasFlag(ParameterTag.Optional))
                    syntax.Append('?');

                return syntax.ToString();
            }
        }

        public object DefaultValue { get; protected set; }

        public IEnumerable<string> ParseExamples { get; protected set; }

        public Type ValueType { get; protected set; }

        public ParameterTag Tags { get; protected set; }

        public override InfoType Type => InfoType.Parameter;
    }
}
