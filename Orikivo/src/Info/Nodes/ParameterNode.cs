using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using ParameterInfo = Discord.Commands.ParameterInfo;

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
            OverloadCount = parameter.Command.Module.Commands.Count(x => x.Name == parameter.Command.Name); // for now...
            CommandId = GetId(parameter.Command, OverloadCount > 1);
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

            if (parameter.Type.IsEquivalentTo(typeof(IMentionable)))
                tag |= ParameterTag.Mentionable;

            return tag;
        }

        public string Command { get; protected set; }
        public string CommandId { get; protected set; }

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

                format.Append($"{Format.Bold(Format.HumanizeType(ValueType))} {Name}");

                if (DefaultValue != null)
                {
                    format.Append($" = {DefaultValue}");
                }

                format.AppendLine();

                // summary
                if (Check.NotNull(Summary))
                    format.AppendLine($"⇛ {Summary}");

                format.AppendLine();

                // info
                if (ValueType != null)
                {
                    if (ValueType.IsEnum)
                    {
                        format.AppendLine($"> **Values**{(ValueType.GetCustomAttribute<FlagsAttribute>() != null ? " (Flags)" : "")}\n```cs");

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

                format.AppendLine($"> **Command**: `{CommandId}`");

                // tags
                if (Tag != 0)
                {
                    format.Append("> **Tags**: ");
                    //format.Append(Format.Bold("#"));
                    //format.Append(' ');
                    format.AppendJoin(", ", EnumUtils.GetValues<ParameterTag>()
                        .Where(t => Tag.HasFlag(t))
                        .Select(t => $"`{t.ToString()}`"));
                    format.AppendLine();
                }

                // parsing examples
                if (Check.NotNullOrEmpty(ParseExamples))
                {
                    format.Append($"> **{Format.TryPluralize("Example", ParseExamples.Count())}**: ");
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
