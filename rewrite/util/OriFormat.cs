using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// A formatter tool used to help format simple sentences.
    /// </summary>
    public static class OriFormat
    {
        public static readonly string DebugFormat = "[Debug] -- {0} --";
        public static readonly string VoiceChannelUrlFormat = "https://discordapp.com/channels/{0}/{1}";
        // ᵃᵇᶜᵈᵉᶠᵍʰᶤʲᵏˡᵐᶯᵒᵖʳˢᵗᵘᵛʷˣʸᶻ (superscript)
        // ₀₁₂₃₄₅₆₇₈₉₊₋₌₍₎ (subscript)
        private static readonly Dictionary<char, char> _subscriptMap = new Dictionary<char, char>
        {
            {'0', '₀'},
            {'1', '₁'},
            {'2', '₂'},
            {'3', '₃'},
            {'4', '₄'},
            {'5', '₅'},
            {'6', '₆'},
            {'7', '₇'},
            {'8', '₈'},
            {'9', '₉'},
            {'+', '₊'},
            {'-', '₋'},
            {'=', '₌'},
            {'(', '₍'},
            {')', '₎'}
        };

        /// <summary>
        /// Attempts to map all known characters to its subscript variant.
        /// </summary>
        public static string Subscript(string value)
            => MapChars(value, CharMap.Subscript);

        public static string HyperlinkEmote(string parsedEmote, string emoteUrl, string emoteName)
            => $"{Format.Url(parsedEmote, emoteUrl)} {emoteName}";

        public static string CodeBlock(string value, CodeType? type = null)
            => $"```{type?.ToString().ToLower()}\n{value}```";

        public static string CropGameId(string value)
            => value.Length > 8 ? value.Substring(0, 8) + "..." : value;

        public static string Position(int i)
        {
            string value = PosNotation(i, false);
            return i switch
            {
                1 => $"{value}st",
                2 => $"{value}nd",
                3 => $"{value}rd",
                _ => $"{value}th",
            };
        }

        private const string POS_NOTATION = "##,0.###";
        public static string PosNotation(int i, bool includeDecimals = true)
            => i.ToString(includeDecimals ? POS_NOTATION : POS_NOTATION.Substring(0, 5));

        public static string Error(string reaction = null, string title = null, string reason = null, string stackTrace = null, bool isEmbedded = false)
        {
            StringBuilder sb = new StringBuilder();
            // if embedded, the reaction is placed on the title.
            if (!isEmbedded)
                if (Checks.NotNull(reaction))
                    sb.AppendLine(Format.Bold(reaction));

            if (Checks.NotNull(title))
                sb.Append(title);

            if (Checks.NotNull(reason))
                sb.Append($"```{reason}```");

            if (Checks.NotNull(stackTrace))
            {
                if (Checks.NotNull(reason))
                    sb.AppendLine();

                sb.Append($"```bf\n{stackTrace}```");
            }

            return sb.ToString();
        }

        // Create a notification pop-up text.
        public static string Notify()
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }

        public static string Lobby(string name, string id, string host, string gameMode)
            => string.Format(LobbyDataFormatter, name, id, host, gameMode);

        public static readonly string LobbyDataFormatter = "> **{0}**#{1} `{2}`\n> ⇛ Playing **{3}**";

        public static readonly string ChatFormatter = "[{0}]: \"{1}\"";

        public static string GameChat(string author, string message)
            => string.Format(ChatFormatter, author, message);

        public static readonly string UserCounterFormatter = "Users (**{0}**/**{1}**):";

        public static string UserCounter(int userCount, int userLimit)
            => string.Format(UserCounterFormatter, userCount, userLimit);

        public static string Hyperlink(string url)
            => Format.Url(Path.GetFileName(url), url);

        public static string CreateVoiceChannelUrl(ulong guildId, ulong voiceChannelId)
            => string.Format(VoiceChannelUrlFormat, guildId, voiceChannelId);
        private static string MapChars(string value, CharMap mapType)
        {
            Dictionary<char, char> map = null;
            switch(mapType)
            {
                case CharMap.Subscript:
                    map = _subscriptMap;
                    break;
                default:
                    throw new Exception("The specified CharMapType was not found.");
            }
            foreach (char c in value)
                if (map.ContainsKey(c))
                    value = value.Replace(c, map[c]);
            return value;
        }

        // ({[A-Za-z._]}) <= Regex format. Regex.Matches(Pattern);
        // use Regex to single out the {} values.
        /// <summary>
        /// Parses the format of a greeting and replaces all specified keys into their known type.
        /// </summary>
        public static string ParseGreeting(string greeting, GuildEventContext context) // TODO: Create literal parser u U P I C v V T y m M d D DIM d DOY H h m s t
        {
            DateTime time = context.User.JoinedAt?.ToUniversalTime().UtcDateTime ?? DateTime.UtcNow;
            return greeting
                .Replace("{user}", context.User.Mention)
            .Replace("{user.name}", context.User.Username)
            .Replace("{user.id}", context.User.Id.ToString())
            .Replace("{user.pos}", (context.Guild.Users.OrderBy(x => x.JoinedAt.Value).ToList().IndexOf(context.User) + 1).ToString())//context.Guild.MemberCount.ToString()) // ToPosition
            .Replace("{guild}", context.Guild.Name)
            .Replace("{guild.id}", context.Guild.Id.ToString())
            .Replace("{guild.users}", context.Guild.MemberCount.ToString()) // PlaceValue
            .Replace("{date}", time.ToString(@"MM-dd-yyyy"))
            .Replace("{time}", time.ToString(@"MM-dd-yyyy HH:mm:ss tt"))
            .Replace("{time.year}", time.Year.ToString())
            .Replace("{time.month}", time.Month.ToString())
            .Replace("{time.Month}", time.ToString(@"MMMM"))
            .Replace("{time.day}", time.Day.ToString())
            .Replace("{time.daysInMonth}", DateTime.DaysInMonth(time.Year, time.Month).ToString())
            .Replace("{time.Day}", time.DayOfWeek.ToString())
            .Replace("{time.dayOfYear}", time.DayOfYear.ToString())
            .Replace("{time.hour}", (time.Hour % 12 == 0 ? 12 : time.Hour % 12).ToString()) // 12h
            .Replace("{time.Hour}", time.Hour.ToString()) // 24h
            .Replace("{time.minute}", time.Minute.ToString())
            .Replace("{time.second}", time.Second.ToString())
            .Replace("{time.millisecond}", time.Millisecond.ToString()) // fff
            .Replace("{time.post}", time.ToString(@"tt"));
        }

        private static bool IsLeapYear(int year)
            => year % 4 == 0;
        /*
            {user}
            {user.name}
            {user.id}
            {user.pos}
            {guild}
            {guild.id}
            {guild.userCount}
            {date}
            {time}
            {time.year}
            {time.month}
            {time.week}
            {time.day}
            {time.hour}
            {time.minute}
            {time.second}
            {time.post}
        */


        // Convert HTML format tags and convert them into Discord markup tags?
        public static string ConvertHtmlTags(string value)
            => throw new NotImplementedException();

        public static string GetNounForm(string word, int count)
            => $"{word}{(count > 1 || count == 0 || count < 0 ? "s" : "")}";

        public static string ReadType<T>()
            => $"**{typeof(T).Name}**";

        public static string Username(IUser user)
            => $"**{user.Username}**#{user.Discriminator}";

        public static string GetShortTime(double seconds)
        {
            char t = 's';
            double n = seconds;
            if (seconds > (60 * 60)) // if seconds is larger than 1 hour (in seconds)
            {
                n = seconds / (60 * 60);
                t = 'h';
            }
            if (seconds > 60) // if seconds is larger than 1 minute (in seconds)
            {
                n = seconds / 60;
                t = 'm';
            }
            
            return $"**{n}**{t}";
        }

        /// <summary>
        /// Returns the specified value into a JSON-friendly string.
        /// </summary>
        public static string Jsonify(string value)
        {
            StringBuilder sb = new StringBuilder();
            char? lastChar = null;
            foreach(char c in value)
            {
                if (char.IsUpper(c))
                    if (lastChar.HasValue)
                        if (char.IsLower(lastChar.Value))
                        {
                            sb.Append($"_{char.ToLower(c)}");
                            lastChar = c;
                            continue;
                        }

                sb.Append(char.ToLower(c));
                lastChar = c;
            }

            return sb.ToString();
        }

        public static string TextChannelName(string value)
        {
            const int MAX_LENGTH = 100;

            if (value.Length > MAX_LENGTH)
                value = value.Substring(0, MAX_LENGTH);

            // regex (([A-Za-z0-9-_ ])*)
            List<char> limitedChars = new List<char>
            { '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '=',
             '{', '[', '}', ']', '|', '\\', ':', ';', '\'', '"', '<', ',', '>', '.',
             '/', '?', ' ' }; // allow characters: A-Z a-z 0-9 - _ (Valid Unicode)
            List<char> separatorChars = new List<char>
            { ' ', '.', ',' };

            StringBuilder result = new StringBuilder();
            foreach (char c in value)
            {
                if (limitedChars.Contains(c))
                {
                    if (separatorChars.Contains(c))
                        result.Append(c == ' ' ? '-' : '_');
                }
                else
                    result.Append(c);
            }

            return result.ToString();
        }
    }
}
