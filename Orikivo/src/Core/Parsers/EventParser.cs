using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Desync
{
    /// <summary>
    /// Handles <see cref="EventContext"/> parsing.
    /// </summary>
    public static class EventParser
    {
        // TODO: Find a better way to handle these custom markers.
        public static Dictionary<string, EventTemplate> Markers => new Dictionary<string, EventTemplate>
        {
            ["user"] = new EventTemplate
            {
                Name = "user",
                Aliases = new List<string>
                {
                    "u"
                },
                Writer = ctx => ctx.User.Mention
            },
            ["name"] = new EventTemplate
            {
                Name = "name",
                Aliases = new List<string>
                {
                    "n"
                },
                Writer = ctx => ctx.User.Username
            },
            ["user_id"] = new EventTemplate
            {
                Name = "user_id",
                Aliases = new List<string>
                {
                    "uid"
                },
                Writer = ctx => ctx.User.Id.ToString()
            },
            ["position"] = new EventTemplate
            {
                Name = "position",
                Aliases = new List<string>
                {
                    "pos"
                },
                Writer = ctx => Format.Ordinal(ctx.Guild.Users.OrderBy(x => x.JoinedAt.Value).ToList().IndexOf(ctx.User) + 1)
            },
            ["guild"] = new EventTemplate
            {
                Name = "guild",
                Aliases = new List<string>
                {
                    "g"
                },
                Writer = ctx => ctx.Guild.Name
            },
            ["guild_id"] = new EventTemplate
            {
                Name = "guild_id",
                Aliases = new List<string>
                {
                    "gid"
                },
                Writer = ctx => ctx.Guild.Id.ToString()
            },
            ["owner"] = new EventTemplate
            {
                Name = "owner",
                Aliases = new List<string>
                {
                    "o"
                },
                Writer = ctx => ctx.Guild.Owner.Username
            },
            ["date"] = new EventTemplate
            {
                Name = "date",
                Aliases = new List<string>
                {
                    "d"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("M/d/yyyy")
            },
            ["full_date"] = new EventTemplate
            {
                Name = "full_date",
                Aliases = new List<string>
                {
                    "D"
                },
                Writer = ctx => ctx.ReceivedAt.ToString($"MMMM d, yyyy")
            },
            ["time"] = new EventTemplate
            {
                Name = "time",
                Aliases = new List<string>
                {
                    "t"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("h:mm tt")
            },
            ["time_24"] = new EventTemplate
            {
                Name = "time_24",
                Aliases = new List<string>
                {
                    "T"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("HH:mm tt")
            },
            ["year"] = new EventTemplate
            {
                Name = "year",
                Aliases = new List<string>
                {
                    "y"
                },
                Writer = ctx => ctx.ReceivedAt.Year.ToString()
            },
            ["month"] = new EventTemplate
            {
                Name = "month",
                Aliases = new List<string>
                {
                    "m"
                },
                Writer = ctx => ctx.ReceivedAt.Month.ToString()
            },
            ["month_name"] = new EventTemplate
            {
                Name = "month_name",
                Aliases = new List<string>
                {
                    "M"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("MMMM")
            },
            ["day"] = new EventTemplate
            {
                Name = "day",
                Aliases = new List<string>
                {
                    "dd"
                },
                Writer = ctx => ctx.ReceivedAt.Day.ToString()
            },

            ["day_name"] = new EventTemplate
            {
                Name = "day_name",
                Aliases = new List<string>
                {
                    "DD"
                },
                Writer = ctx => ctx.ReceivedAt.DayOfWeek.ToString()
            },
            ["hour"] = new EventTemplate
            {
                Name = "hour",
                Aliases = new List<string>
                {
                    "h"
                },
                Writer = ctx => ctx.ReceivedAt.Hour.ToString()
            },
            ["hour_24"] = new EventTemplate
            {
                Name = "hour_24",
                Aliases = new List<string>
                {
                    "H"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("hh")
            },
            ["minute"] = new EventTemplate
            {
                Name = "minute",
                Aliases = new List<string>
                {
                    "mm"
                },
                Writer = ctx => ctx.ReceivedAt.Minute.ToString()
            },
            ["second"] = new EventTemplate
            {
                Name = "second",
                Aliases = new List<string>
                {
                    "s"
                },
                Writer = ctx => ctx.ReceivedAt.Second.ToString()
            },
            ["millisecond"] = new EventTemplate
            {
                Name = "millisecond",
                Aliases = new List<string>
                {
                    "ms"
                },
                Writer = ctx => ctx.ReceivedAt.Millisecond.ToString()
            },
            ["period"] = new EventTemplate
            {
                Name = "period",
                Aliases = new List<string>
                {
                    "p"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("tt")
            }
        };

        public static string Parse(string content, EventContext context)
        {
            var result = new StringBuilder();
            bool marker = false;
            string id = "";
            foreach (char c in content)
            {
                if (c == '{' && !marker)
                {
                    marker = true;
                    continue;
                }

                if (c == '}' && marker)
                {
                    marker = false;
                    result.Append(TryParse(id, context));
                    id = "";
                    continue;
                }

                if (marker)
                    id += c;
                else
                    result.Append(c);
            }

            return result.ToString();
        }

        private static string TryParse(string id, EventContext context)
        {
            if (Markers.Values.Any(x => x.Name == id || x.Aliases.Contains(id)))
                return Markers.Values.First(x => x.Name == id || x.Aliases.Contains(id)).Writer.Invoke(context);

            // return markers if invalid
            return "{" + id + "}";
        }
    }
}
