using Discord;
using Orikivo.Static;

namespace Orikivo
{
    

    public static class EmojiExtension
    {
        /// <summary>
        /// Returns an emoji displayed as a specified IconFormat.
        /// </summary>
        public static string Format(this Emoji e, IconFormat f)
        {
            switch (f)
            {
                case IconFormat.Hidden:
                    return string.Empty;
                case IconFormat.Packed:
                    return e.Name.DiscordLine();
                case IconFormat.Escaped:
                    return e.ToString();
                default:
                    return e.Name;
            }
        }

        public static string Escape(this Emoji e) =>
            $"\\{e}";

        public static OriReportPriorityType GetFlagType(this Emoji e)
        {
            if (e == EmojiIndex.PriorityFlag)
            {
                return OriReportPriorityType.Critical;
            }
            if (e == EmojiIndex.ExceptionFlag)
            {
                return OriReportPriorityType.Warning;
            }
            if (e == EmojiIndex.SpeedFlag)
            {
                return OriReportPriorityType.Speed;
            }
            if (e == EmojiIndex.VisualFlag)
            {
                return OriReportPriorityType.Visual;
            }

            return OriReportPriorityType.Unknown;
        }

        public static Emoji Icon(this OriReportPriorityType e)
        {
            switch (e)
            {
                case OriReportPriorityType.Critical:
                    return EmojiIndex.PriorityFlag;
                case OriReportPriorityType.Warning:
                    return EmojiIndex.ExceptionFlag;
                case OriReportPriorityType.Speed:
                    return EmojiIndex.SpeedFlag;
                case OriReportPriorityType.Visual:
                    return EmojiIndex.VisualFlag;
                default:
                    return EmojiIndex.SuggestFlag;
            }
        }

        public static string GetName(this OriReportPriorityType e)
        {
            switch (e)
            {
                case OriReportPriorityType.Critical:
                    return "Priority Report";
                case OriReportPriorityType.Warning:
                    return "Exception Report";
                case OriReportPriorityType.Speed:
                    return "Runtime Report";
                case OriReportPriorityType.Visual:
                    return "Visual Report";
                default:
                    return "Suggestion";
            }
        }

        public static string GetDefaultSummary(this OriReportPriorityType e)
        {
            switch (e)
            {
                case OriReportPriorityType.Critical:
                    return "A major error may exist within this context.";
                case OriReportPriorityType.Warning:
                    return "A typical runtime error may occur on this command.";
                case OriReportPriorityType.Speed:
                    return "This command may run slower than intended.";
                case OriReportPriorityType.Visual:
                    return "This command may have a misspell.";
                default:
                    return "Nothing was specified.";
            }
        }
    }
}