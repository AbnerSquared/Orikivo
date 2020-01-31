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

        public static string GetSeverityIcon(ContextNode info)
            => GetSeverityIcon(info.Reports.Where(x => x.State == ReportState.Open).Count());

        public static ParameterTag GetMod(ParameterInfo parameter)
        {
            ParameterTag mod = 0;
            if (parameter.IsOptional)
                mod |= ParameterTag.Optional;
            if (parameter.IsRemainder)
                mod |= ParameterTag.Trailing;
            if (parameter.Type == typeof(SocketUser))
                mod |= ParameterTag.Mentionable;

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

        /*
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
        }*/

        public static string WriteDisplayContent(ModuleNode info)
        {
            StringBuilder sb = new StringBuilder();
            if (!Checks.NotNull(info.Group))
            {
                /* {ConcatFamilyTree(info.Family.Skip(1).ToList())} */
                sb.AppendLine($"**{info.Name}**");
                if (info.Summary != null)
                {
                    sb.AppendLine(OriFormat.Code(info.Summary));
                }

                if (info.Commands.Count > 0)
                {
                    sb.AppendLine($"**Commands** {OriFormat.Subscript($"({info.Commands.Count})")}:");
                    sb.AppendJoin(" ", info.Commands
                        .Select(x => $"`{x.Name}`")
                        .Concat(info.Submodules.Where(x => Checks.NotNull(x.Group))
                        .Select(x => $"`{x.Name}*`"))
                        .OrderBy(x => x[1]));
                }

                IEnumerable<ModuleNode> subs = info.Submodules.Where(x => !Checks.NotNull(x.Group));
                if (subs.Count() > 0)
                {
                    sb.AppendLine($"**Submodules** {OriFormat.Subscript($"({subs.Count()})")}:");
                    sb.AppendJoin("\n", subs
                        .Select(x => $"{GetSeverityIcon(x.Reports?.Count ?? 0)}**{x.Name}** {OriFormat.Subscript($"(+{x.CommandCount})")}"));
                }
            }
            else
            {
                sb.AppendLine($"{(info.Aliases.Count > 0 ? $"[**{info.Name}**, {string.Join(", ", info.Aliases.OrderByDescending(x => x.Length))}]" : $"**{info.Name}**")}");
                sb.AppendJoin("\n", info.Commands.Select(x => WriteCommandOverloads(x)));
            }
            return sb.ToString();
        }

        public static string WriteCommandOverloads(CommandNode info)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Join("\n", info.Overloads.Select(x =>
            {
                StringBuilder ob = new StringBuilder();
                // List<string> name = x.Name.Split(' ').ToList();
                // string.Join(' ', name.Select(y => name.IndexOf(y) == (name.Count - 1) ? $"**{y}**" : y)
                string fullName = x.Name;
                if (Checks.NotNull(x.Group))
                {
                    fullName = info.Group;
                    if (!string.IsNullOrWhiteSpace(x.Name))
                        fullName += $" {x.Name}";
                }
                List<string> name = fullName.Split(' ').ToList();
                ob.Append($"> {string.Join(' ', name.Select(y => name.IndexOf(y) == (name.Count - 1) ? $"**{y}**" : y))}");
                ob.Append($"{(info.Overloads.Count > 1 ? OriFormat.Subscript($"+{x.Index}") : "")}({string.Join(", ", x.Parameters.Select(y => y.Syntax))})");
                return ob.ToString();
            })));
            return sb.ToString();
        }

        /*
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

        // TODO: Use depth to group up modules and commands to properly get its ID.
        public static string ConcatFamilyTree(List<ContextValue> family, InfoType infoType)
        {
            if (!Checks.NotNull(family))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < family.Count; i++)
            {
                sb.Append(family[i].Name);

                if (i == family.Count - 1) // what this does on the last match.
                    break;

                if (family[i + 1].Type == InfoType.Parameter) // what todo when the next value is a parameter.
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
                InfoType.Module => ".",
                InfoType.Group => " ",
                _ => string.Empty
            };
        }
        */
    }
}
