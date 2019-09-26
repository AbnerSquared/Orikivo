using Discord.Commands;
using Orikivo.Modules;
using Orikivo.Utility;
using Orikivo.Wrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class WidgetHelper
    {
        public static bool IsValidHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return false;
            Regex regex = new Regex(@"#?[0-9A-Fa-f]{3,6}");
            @"#?[0-9A-Fa-f]{3|6}".Debug("Pattern");
            Debugger.Write($"-- {hex} | Specified Hex... --");
            if (regex.IsMatch(hex))
                return true;
            Debugger.Write("-- Match failed... --");
            return false;

        }
        public static bool TryParseColorHex(string hex, out Color c)
        {
            c = Color.Empty;
            Color color = ColorTranslator.FromHtml(hex);
            if (color.Exists())
            {
                c = color;
                return true;
            }
            return false;
        }
        public static bool TryParseWidgetType(string s, out WidgetType wt)
        {
            wt = WidgetType.Status;
            Type type = typeof(WidgetType);
            foreach (string name in type.GetEnumNames())
            {
                name.Debug();
                if (name.Matches(s, MatchHandling.Match, MatchValueHandling.StartsWith))
                {
                    wt = (WidgetType)type.GetField(name).GetRawConstantValue();
                    return true;
                }
            }

            return false;
        }
        public static bool TryParseWidgetType(int i, out WidgetType ao)
        {
            if (!Enum.TryParse($"{i}", out ao))
                return false;
            return true;
        }
    }

    public class WidgetTypeTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            WidgetType result;
            if (int.TryParse(input, out int id))
            {
                if (WidgetHelper.TryParseWidgetType(id, out result))
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(result));
                }
            }
            if (WidgetHelper.TryParseWidgetType(input, out result))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as an WidgetType."));
        }
    }
}
