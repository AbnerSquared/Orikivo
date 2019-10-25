using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// A simplified paramter information container.
    /// </summary>
    public class ParameterDisplayInfo : IDisplayInfo
    {
        internal ParameterDisplayInfo(ParameterInfo parameter, List<ContextValue> family = null)
        {
            CommandName = parameter.Command.Name;
            CommandPriority = parameter.Command.Priority;
            Name = parameter.Name;
            Summary = parameter.Summary;
            ValueType = parameter.Type;
            DefaultValue = parameter.DefaultValue;
            Mod = ContextUtils.GetMod(parameter);

            if (Checks.NotNull(family))
                family.Add(new ContextValue(parameter));
            Family = family ?? ContextUtils.GetFamilyTree(parameter);
        }

        public ContextInfoType Type => ContextInfoType.Parameter;

        public string Id => ContextUtils.ConcatFamilyTree(Family);

        public string Name { get; }

        public string Summary { get; }

        public List<string> Aliases { get; }

        public List<IReport> Reports { get; }

        public List<ContextValue> Family { get; }

        public Type ValueType { get; }

        public string SyntaxName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Mod.HasFlag(ParameterMod.Mentionable))
                    sb.Append("@");
                sb.Append(Name);
                if (Mod.HasFlag(ParameterMod.Trailing))
                    sb.Append("...");
                if (Mod.HasFlag(ParameterMod.Optional))
                    sb.Append("?");
                return sb.ToString();
            }
        }

        public string CommandName { get; }

        public int CommandPriority { get; }
        
        public object DefaultValue { get; }

        public ParameterMod Mod { get; }

        public bool IsOptional => Mod.HasFlag(ParameterMod.Optional);

        public string Content => ContextUtils.WriteDisplayContent(this);
    }
}
