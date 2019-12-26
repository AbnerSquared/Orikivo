using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public static class ContextUtils
    {
        public const int YIELD_THRESHOLD = 1; // if > 1, deem as warning
        public const int CRITICAL_THRESHOLD = 10; // if > 10, deem as critical

        private const string STABLE_EMOJI = "\uD83D\uDD39"; /* :small_blue_diamond: */
        private const string YIELD_EMOJI = "\uD83D\uDD38"; /* :small_orange_diamond: */
        private const string CRITICAL_EMOJI = "\uD83D\uDD3A"; /* :small_red_triangle: */
        public static string GetSeverityIcon(int reportCount)
            => reportCount >= CRITICAL_THRESHOLD ? CRITICAL_EMOJI : reportCount >= YIELD_THRESHOLD ? YIELD_EMOJI : STABLE_EMOJI;

        public static string GetSeverityIcon(IDisplayInfo info)
            => GetSeverityIcon(info.Reports.Where(x => x.State == ReportState.Open).Count());

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
                    .Select(x => x.Replace($"{groupName} ", string.Empty).Trim())
                    .Where(x => x != command.Name).ToList();

                if (!Checks.NotNull(command.Name))
                    aliases.Clear();
            }

            return aliases;
        }

        public static string WriteDisplayContent(GuildDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**{info.Name}** ({info.GuildName})");
            sb.AppendLine(OriFormat.CodeBlock(info.Summary));
            if (info.Customs.Count > 0)
            {
                sb.AppendLine($"**Commands** {OriFormat.Subscript($"({info.Customs.Count})")}:");
                sb.AppendLine($"{string.Join(" ", info.Customs.Select(x => $"`{x.Name}`").OrderBy(x => x).ToList())}");
            }

            return sb.ToString();
        }

        public static string WriteDisplayContent(ModuleDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            if (!info.IsGroup)
            {
                /* {ConcatFamilyTree(info.Family.Skip(1).ToList())} */
                sb.AppendLine($"**{info.Name}**");
                if (info.Summary != null)
                {
                    sb.AppendLine(OriFormat.CodeBlock(info.Summary));
                }

                if (info.Commands.Count > 0)
                {
                    sb.AppendLine($"**Commands** {OriFormat.Subscript($"({info.Commands.Count})")}:");
                    sb.AppendLine($"{string.Join(" ", info.Commands.Select(x => x.BlockName).Concat(info.Submodules.Where(x => x.IsGroup).Select(x => x.BlockName)).OrderBy(x => x[1]).ToList())}");
                }
                IEnumerable<ModuleDisplayInfo> subs = info.Submodules.Where(x => x.IsGroup == false);
                if (subs.Count() > 0)
                {
                    sb.AppendLine($"**Submodules** {OriFormat.Subscript($"({subs.Count()})")}:");
                    sb.AppendLine($"{string.Join("\n", subs.Select(x => $"{GetSeverityIcon(x.Reports?.Count ?? 0)}**{x.Name}** {OriFormat.Subscript($"(+{x.TotalCommands})")}"))}");
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

            string name =
                    info.IsInGroup ?
                    Checks.NotNull(info.Name) ?
                    $"{info.GroupName} **{info.Name}**" :
                    $"**{info.GroupName}**" :
                    $"**{info.Name}**";

            if (info.HasMultiple)
            {
                

                sb.AppendLine($"{(info.Aliases.Count > 0 ? $"[{name}, {string.Join(", ", info.Aliases)}]" : name)}");
                sb.AppendLine($"• **Overloads** {OriFormat.Subscript($"({info.Overloads.Count})")}:");
                sb.Append(WriteCommandOverloads(info));
            }
            else
            {
                // NAMING
                sb.Append($"{(info.Aliases.Count > 0 ? $"[{name}, {string.Join(", ", info.Aliases.OrderByDescending(x => x.Length))}]" : name)}");
                // PARAMETERS
                sb.AppendLine($"{string.Join(", ", info.Default.Parameters.Select(x => x.SyntaxName))})");
                if (info.Summary != null)
                    sb.AppendLine($"⇛ {info.Default.Summary}");

                // EXTRA
                
                if (info.Default.TrustLevel.HasValue)
                    sb.AppendLine($"> **Access**: {info.Default.TrustLevel.Value.ToString()}");
                if (info.Default.CooldownLength.HasValue)
                    sb.AppendLine($"> **Cooldown**: {OriFormat.GetShortTime(info.Default.CooldownLength.Value)}");
                if (info.Default.Permissions != null)
                    sb.AppendLine($"> **Permissions**: {string.Join(", ", info.Default.Permissions.Select(x => $"`{x.ToString()}`"))}");

                sb.Append($"> **ID**: `{info.Id}`");

            }

            return sb.ToString();
        }

        public static string WriteDisplayContent(OverloadDisplayInfo info)
        {
            string name =
                    info.IsInGroup ?
                    Checks.NotNull(info.Name) ?
                    $"{info.GroupName} **{info.Name}**" :
                    $"**{info.GroupName}**" :
                    $"**{info.Name}**";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{name}({string.Join(", ", info.Parameters.Select(x => x.SyntaxName))}) {OriFormat.Subscript($"+{info.Priority}")}");
            if (info.Summary != null)
                sb.AppendLine($"⇛ {info.Summary}");

            if (info.TrustLevel.HasValue)
                sb.AppendLine($"> **Access**: {info.TrustLevel.Value.ToString()}");
            if (info.CooldownLength.HasValue)
                sb.AppendLine($"> **Cooldown**: {OriFormat.GetShortTime(info.CooldownLength.Value)}");
            if (info.Permissions != null)
                sb.AppendLine($"> **Permissions**: {string.Join(", ", info.Permissions.Select(x => $"`{x.ToString()}`"))}");

            sb.Append($"> **ID**: `{info.Id}`");

            return sb.ToString();
        }

        // guildcommands
        public static string WriteDisplayContent(CustomDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**{info.Name}**()"); // info.Mentions

            if (info.HasImage)
                sb.AppendLine("`Contains Imagery`");

            if (info.Author != null)
                sb.AppendLine($"> **Creator**: **{info.Author.Name}**");

            sb.Append($"> **ID**: `{info.Id}`");

            return sb.ToString();
        }


        public static string WriteDisplayContent(ParameterDisplayInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**{info.Name}**[{info.CommandName}{OriFormat.Subscript($"+{info.CommandPriority}")}]{(info.IsOptional ? $" = **{info.DefaultValue?.ToString() ?? "null"}**" : "")}");
            if (info.Summary != null)
                sb.AppendLine($"⇛ {info.Summary}");

            if (info.Mod > 0)
            {
                sb.AppendLine($"> **Mods**: {string.Join(", ", EnumUtils.GetValues<ParameterMod>().Where(x => info.Mod.HasFlag(x)).Select(x => $"`{x.ToString()}`"))}");
            }
            sb.AppendLine($"> typeof(**{info.ValueType.Name}**)");
            
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

        // TODO: Fix family tree ID
        public static string ConcatFamilyTree(List<ContextValue> family, ContextInfoType infoType)
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
            return value.Type switch
            {
                ContextInfoType.Module => ".",
                ContextInfoType.Group => " ",
                _ => string.Empty
            };
        }
    }
}
