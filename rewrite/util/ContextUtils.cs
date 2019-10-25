﻿using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public static class ContextUtils
    {
        public static ParameterMod GetMod(ParameterInfo parameter)
        {
            ParameterMod mod = 0;
            if (parameter.IsOptional)
                mod |= ParameterMod.Optional;
            if (parameter.IsRemainder)
                mod |= ParameterMod.Trailing;
            if (parameter.Type == typeof(SocketUser))
                mod |= ParameterMod.Mentionable;

            return mod;
        }

        public static List<string> GetAliases(CommandInfo command)
        {
            List<string> aliases = command.Aliases.Where(x => x != command.Name).ToList();

            if (Checks.NotNull(command.Module.Group))
            {
                string groupName = command.Module.Group;

                aliases = aliases
                    .Where(x => x.StartsWith($"{command.Module.Group} "))
                    .Select(x => x.Replace($"{groupName} ", "").Trim())
                    .Where(x => x != command.Name).ToList();

                if (!Checks.NotNull(command.Name))
                    aliases.Clear();
            }

            return aliases;
        }

        public static string WriteDisplayContent(ModuleDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            if (!info.IsGroup)
            {
                sb.AppendLine($"{ConcatFamilyTree(info.Family.Skip(1).ToList())}**{info.Name}**");
                if (info.Summary != null)
                {
                    sb.AppendLine($"```\n{info.Summary}```");
                }

                if (info.Commands.Count > 0)
                {
                    sb.AppendLine($"**Commands** {OriFormat.Subscript($"({info.Commands.Count})")}:");
                    sb.AppendLine($"{string.Join(" ", info.Commands.Select(x => x.BlockName).Concat(info.Submodules.Where(x => x.IsGroup).Select(x => x.BlockName)).OrderBy(x => x[1]).ToList())}");
                }
                if (info.Submodules.Count > 0)
                {
                    sb.AppendLine($"**Submodules** {OriFormat.Subscript($"({info.Submodules.Count})")}:");
                    sb.AppendLine($"{string.Join("\n", info.Submodules.Select(x => $"🔹**{x.Name}** {OriFormat.Subscript($"(+{x.TotalCommands})")}"))}");
                }
            }
            else
            {
                sb.AppendLine($"{(info.Aliases.Count > 0 ? $"[**{info.Name}**, {string.Join(", ", info.Aliases.OrderByDescending(x => x.Length))}]" : $"**{info.Name}**")}");
                sb.AppendLine($"{string.Join("\n", info.Commands.Select(x => WriteCommandOverloads(x)))}");
            }
            return sb.ToString();
        }

        public static string WriteCommandOverloads(CommandDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Join("\n", info.Overloads.Select(x =>
            {
                StringBuilder ob = new StringBuilder();
                // List<string> name = x.Name.Split(' ').ToList();
                // string.Join(' ', name.Select(y => name.IndexOf(y) == (name.Count - 1) ? $"**{y}**" : y)
                string fullName = x.Name;
                if (x.IsInGroup)
                {
                    fullName = info.GroupName;
                    if (!string.IsNullOrWhiteSpace(x.Name))
                        fullName += $" {x.Name}";
                }
                List<string> name = fullName.Split(' ').ToList();
                ob.Append($"> {string.Join(' ', name.Select(y => name.IndexOf(y) == (name.Count - 1) ? $"**{y}**" : y))}");
                ob.Append($"{(info.HasMultiple ? OriFormat.Subscript($"+{x.Priority}") : "")}({string.Join(", ", x.Parameters.Select(y => y.SyntaxName))})");
                return ob.ToString();
            })));
            return sb.ToString();
        }

        public static string WriteDisplayContent(CommandDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            if (info.HasMultiple)
            {
                sb.AppendLine($"{(info.IsInGroup ? $"{info.GroupName} " : "")}{(info.Aliases.Count > 0 ? $"[**{info.Name}**, {string.Join(", ", info.Aliases)}]" : $"**{info.Name}**")}");
                sb.AppendLine($"• **Overloads** {OriFormat.Subscript($"({info.Overloads.Count})")}:");
                sb.Append(WriteCommandOverloads(info));
            }
            else
            {
                sb.AppendLine($"> {(info.IsInGroup ? $"{info.GroupName} " : "")}{(info.Aliases.Count > 0 ? $"[**{info.Name}**, {string.Join(", ", info.Aliases.OrderByDescending(x => x.Length))}]" : $"**{info.Name}**")}({string.Join(", ", info.Default.Parameters.Select(x => x.SyntaxName))})");
                if (info.Default.Permissions != null)
                    sb.AppendLine($"> **Permissions Required**: {string.Join(", ", info.Default.Permissions.Select(x => $"`{x.ToString()}`"))}");
                if (info.Default.TrustLevel.HasValue)
                    sb.AppendLine($"> **Elevation**: {info.Default.TrustLevel.Value.ToString()}");
                if (info.Default.CooldownLength.HasValue)
                    sb.AppendLine($"> **Cooldown**: {OriFormat.GetShortTime(info.Default.CooldownLength.Value)}");
                if (info.Summary != null)
                    sb.AppendLine($"> ⇛ {info.Default.Summary}");
            }

            return sb.ToString();
        }

        public static string WriteDisplayContent(OverloadDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"> {(info.IsInGroup ? $"{info.GroupName} " : "")}**{info.Name}**{OriFormat.Subscript($"+{info.Priority}")}({string.Join(", ", info.Parameters.Select(x => x.SyntaxName))})");
            if (info.Permissions != null)
                sb.AppendLine($"> **Permissions Required**: {string.Join(", ", info.Permissions.Select(x => $"`{x.ToString()}`"))}");
            if (info.TrustLevel.HasValue)
                sb.AppendLine($"> **Elevation**: {info.TrustLevel.Value.ToString()}");
            if (info.CooldownLength.HasValue)
                sb.AppendLine($"> **Cooldown**: {OriFormat.GetShortTime(info.CooldownLength.Value)}");
            if (info.Summary != null)
                sb.Append($"> ⇛ {info.Summary}");
            return sb.ToString();
        }

        public static string WriteDisplayContent(ParameterDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**{info.Name}**[{info.CommandName}{OriFormat.Subscript($"+{info.CommandPriority}")}]{(info.IsOptional ? $" = **{info.DefaultValue?.ToString() ?? "null"}**" : "")}");
            if (info.Mod > 0)
            {
                sb.AppendLine($"**+** {string.Join(", ", EnumUtils.GetValues<ParameterMod>().Where(x => info.Mod.HasFlag(x)).Select(x => $"`{x.ToString()}`"))}");
            }
            sb.AppendLine($"typeof(**{info.ValueType.Name}**)");
            if (info.Summary != null)
                sb.AppendLine($"⇛ {info.Summary}");
            return sb.ToString();
        }

        public static List<ContextValue> GetFamilyTree(ParameterInfo parameter)
        {
            List<ContextValue> values = new List<ContextValue>();
            values.AddRange(GetFamilyTree(parameter.Command));
            values.Add(new ContextValue(parameter));
            return values;
        }

        public static List<ContextValue> GetFamilyTree(CommandInfo command)
        {
            List<ContextValue> values = new List<ContextValue>();
            values.AddRange(GetFamilyTree(command.Module));
            values.Add(new ContextValue(command));
            return values;
        }

        public static List<ContextValue> GetFamilyTree(ModuleInfo module)
        {
            List<ContextValue> values = new List<ContextValue>();
            values.Add(new ContextValue(module));

            ModuleInfo parent = module.Parent;
            while (Checks.NotNull(parent))
            {
                values.Add(new ContextValue(module));
                parent = parent.Parent;
            }

            values.Reverse(); // the first one is now the oldest.
            return values;
        }

        public static string ConcatFamilyTree(List<ContextValue> family)
        {
            if (!Checks.NotNull(family))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < family.Count; i++)
            {
                sb.Append(family[i].Name);

                if (i == family.Count - 1) // what this does on the last match.
                    break;

                if (family[i + 1].Type == ContextInfoType.Parameter) // what todo when the next value is a parameter.
                    sb.Append('(');
                else
                    sb.Append(GetValueSeparator(family[i]));
            }

            return sb.ToString();
        }

        private static string GetValueSeparator(ContextValue value)
        {
            switch (value.Type)
            {
                case ContextInfoType.Module:
                    return ".";

                case ContextInfoType.Group:
                    return " ";

                default:
                    return string.Empty;
            }
        }
    }
}
