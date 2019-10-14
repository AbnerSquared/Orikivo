using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // this class is used to help signify and identify information about a parameter/arg
    public class ParameterDisplayInfo : IDisplayInfo
    {
        public ParameterDisplayInfo(ParameterInfo parameter)
        {
            CommandName = parameter.Command.Name;
            CommandPriority = parameter.Command.Priority;
            Name = parameter.Name;
            Summary = parameter.Summary;
            Type = parameter.Type;
            DefaultValue = parameter.DefaultValue;
            Mods = new List<ParameterMod>();
            if (parameter.IsOptional)
                Mods.Add(ParameterMod.Optional);
            if (parameter.IsRemainder)
                Mods.Add(ParameterMod.Trailing);
            if (parameter.Type == typeof(SocketUser))
                Mods.Add(ParameterMod.Mentionable);
        }
        public string SyntaxName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Mods.Contains(ParameterMod.Mentionable))
                    sb.Append("@");
                sb.Append(Name);
                if (Mods.Contains(ParameterMod.Trailing))
                    sb.Append("...");
                if (Mods.Contains(ParameterMod.Optional))
                    sb.Append("?");
                return sb.ToString();
            }
        }

        public string CommandName { get; }
        public int CommandPriority { get; }
        public string Name { get; }
        private string DefaultName { get; } // the name that the parameter was given in the code
        public string Summary { get; }
        public Type Type { get; }
        public object DefaultValue { get; } // the default value if one is set
        public List<ParameterMod> Mods { get; } // replace with flag variants
        public bool IsOptional => Mods.Contains(ParameterMod.Optional);

        public string GetDisplay()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**{Name}**[{CommandName}{OriFormat.Subscript($"+{CommandPriority}")}]{(IsOptional ? $" = **{DefaultValue}**" : "")}");
            if (Mods.Count > 0)
            {
                sb.AppendLine($"**+** {string.Join(", ", Mods.Select(x => $"`{x.ToString()}`"))}");
            }
            sb.AppendLine($"typeof(**{Type.Name}**)");
            if (Summary != null)
                sb.AppendLine($"⇛ {Summary}");
            return sb.ToString();
        }
    }
}
