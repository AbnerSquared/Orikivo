using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class ParameterNode : ContextNode // maybe add preconditions??
    {
        public ParameterNode(ParameterInfo parameter) : base(parameter)
        {
            var command = new StringBuilder();

            // Getting the full name of the command
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
            OverloadCount = 1; // for now...
            DefaultValue = parameter.DefaultValue;
            ValueType = parameter.Type;
            Tag = GetTagValue(parameter);
            ParseExamples = parameter.Attributes.FirstOrDefault<ExampleAttribute>()?.Examples;
        }

        private static ParameterTag GetTagValue(ParameterInfo parameter)
        {
            ParameterTag tag = 0;

            if (parameter.IsOptional)
                tag |= ParameterTag.Optional;

            if (parameter.IsRemainder)
                tag |= ParameterTag.Trailing;

            if (parameter.Type.EqualsAny(typeof(IMentionable)))
                tag |= ParameterTag.Mentionable;

            return tag;
        }

        public string Command { get; protected set; }

        public int OverloadIndex { get; protected set; }

        public int OverloadCount { get; protected set; }

        public string Syntax
        {
            get
            {
                StringBuilder syntax = new StringBuilder();

                if (Tag.HasFlag(ParameterTag.Mentionable))
                    syntax.Append('@');

                syntax.Append(Name);

                if (Tag.HasFlag(ParameterTag.Trailing))
                {
                    syntax.Append("..");

                    if (!Tag.HasFlag(ParameterTag.Optional))
                        syntax.Append('.');
                }

                if (Tag.HasFlag(ParameterTag.Optional))
                    syntax.Append('?');

                return syntax.ToString();
            }
        }

        public object DefaultValue { get; protected set; }

        public IEnumerable<string> ParseExamples { get; protected set; }

        public Type ValueType { get; protected set; }

        public ParameterTag Tag { get; protected set; }

        public override InfoType Type => InfoType.Parameter;

        protected override string Formatting
        {
            get
            {
                var format = new StringBuilder();

                format.Append(Format.Bold(Name));


                format.Append('[');
                format.Append(Command);

                if (OverloadCount > 1)
                    format.Append(Format.Subscript($" +{OverloadIndex}"));

                format.Append(']');

                if (DefaultValue != null)
                {
                    format.Append(" = ");
                    format.Append(Format.Italics((DefaultValue ?? "null").ToString())); // allow strings that specify the name.
                }

                format.AppendLine();

                // summary
                if (Check.NotNull(Summary))
                {
                    format.Append("⇛ ");
                    format.Append(Summary);
                    format.AppendLine();
                }

                // tags
                if (Tag != 0)
                {
                    format.Append(Format.Bold("#"));
                    format.Append(' ');
                    format.AppendJoin(", ", EnumUtils.GetValues<ParameterTag>()
                        .Where(t => Tag.HasFlag(t))
                        .Select(t => $"`{t.ToString()}`"));
                    format.AppendLine();
                }

                // info
                if (ValueType != null)
                {
                    format.Append("> ");
                    format.Append("typeof ");
                    format.Append(Format.Bold(Format.HumanizeType(ValueType)));
                    format.AppendLine();

                    // If unwanted, simply just revert
                    if (ValueType.IsEnum)
                    {
                        format.AppendLine("```cs");

                        var names = ValueType.GetEnumNames();
                        var values = ValueType.GetEnumValues();

                        for(int i = 0; i < names.Length; i++)
                        {
                            object enumValue = values.GetValue(i);


                            format.AppendLine($"{names[i]} = {Convert.ToInt16(enumValue)}");
                        }

                        format.AppendLine("```");
                    }
                }

                // parsing examples
                if (Check.NotNull(ParseExamples))
                {
                    format.Append("> ");
                    format.Append("**Example**: ");
                    format.AppendJoin(", ", ParseExamples.Select(x => $"`{x}`"));
                    format.AppendLine();
                }

                /*
                // id
                if (Check.NotNull(Id))
                {
                    format.AppendLine();
                    format.Append($"**ID**: `{Id}`");
                }
                */
                return format.ToString();
            }
        }
    }
}
