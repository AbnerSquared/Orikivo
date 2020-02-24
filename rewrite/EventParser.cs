using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Handles <see cref="EventContext"/> parsing.
    /// </summary>
    public static class EventParser
    {
        public static Dictionary<string, EventMarker> Markers => new Dictionary<string, EventMarker>
        {
            ["user"] = new EventMarker
            {
                Name = "user",
                Aliases = new List<string>
                {
                    "u"
                },
                Writer = ctx => ctx.User.Mention
            },
            ["name"] = new EventMarker
            {
                Name = "name",
                Aliases = new List<string>
                {
                    "n"
                },
                Writer = ctx => ctx.User.Username
            },
            ["user_id"] = new EventMarker
            {
                Name = "user_id",
                Aliases = new List<string>
                {
                    "uid"
                },
                Writer = ctx => ctx.User.Id.ToString()
            },
            ["position"] = new EventMarker
            {
                Name = "position",
                Aliases = new List<string>
                {
                    "pos"
                },
                Writer = ctx => OriFormat.Position(ctx.Guild.Users.OrderBy(x => x.JoinedAt.Value).ToList().IndexOf(ctx.User) + 1)
            },
            ["guild"] = new EventMarker
            {
                Name = "guild",
                Aliases = new List<string>
                {
                    "g"
                },
                Writer = ctx => ctx.Guild.Name
            },
            ["guild_id"] = new EventMarker
            {
                Name = "guild_id",
                Aliases = new List<string>
                {
                    "gid"
                },
                Writer = ctx => ctx.Guild.Id.ToString()
            },
            ["owner"] = new EventMarker
            {
                Name = "owner",
                Aliases = new List<string>
                {
                    "o"
                },
                Writer = ctx => ctx.Guild.Owner.Username
            },
            ["date"] = new EventMarker
            {
                Name = "date",
                Aliases = new List<string>
                {
                    "d"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("M/d/yyyy")
            },
            ["full_date"] = new EventMarker
            {
                Name = "full_date",
                Aliases = new List<string>
                {
                    "D"
                },
                Writer = ctx => ctx.ReceivedAt.ToString($"MMMM d, yyyy")
            },
            ["time"] = new EventMarker
            {
                Name = "time",
                Aliases = new List<string>
                {
                    "t"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("h:mm tt")
            },
            ["time_24"] = new EventMarker
            {
                Name = "time_24",
                Aliases = new List<string>
                {
                    "T"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("HH:mm tt")
            },
            ["year"] = new EventMarker
            {
                Name = "year",
                Aliases = new List<string>
                {
                    "y"
                },
                Writer = ctx => ctx.ReceivedAt.Year.ToString()
            },
            ["month"] = new EventMarker
            {
                Name = "month",
                Aliases = new List<string>
                {
                    "m"
                },
                Writer = ctx => ctx.ReceivedAt.Month.ToString()
            },
            ["month_name"] = new EventMarker
            {
                Name = "month_name",
                Aliases = new List<string>
                {
                    "M"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("MMMM")
            },
            ["day"] = new EventMarker
            {
                Name = "day",
                Aliases = new List<string>
                {
                    "dd"
                },
                Writer = ctx => ctx.ReceivedAt.Day.ToString()
            },

            ["day_name"] = new EventMarker
            {
                Name = "day_name",
                Aliases = new List<string>
                {
                    "DD"
                },
                Writer = ctx => ctx.ReceivedAt.DayOfWeek.ToString()
            },
            ["hour"] = new EventMarker
            {
                Name = "hour",
                Aliases = new List<string>
                {
                    "h"
                },
                Writer = ctx => ctx.ReceivedAt.Hour.ToString()
            },
            ["hour_24"] = new EventMarker
            {
                Name = "hour_24",
                Aliases = new List<string>
                {
                    "H"
                },
                Writer = ctx => ctx.ReceivedAt.ToString("hh")
            },
            ["minute"] = new EventMarker
            {
                Name = "minute",
                Aliases = new List<string>
                {
                    "mm"
                },
                Writer = ctx => ctx.ReceivedAt.Minute.ToString()
            },
            ["second"] = new EventMarker
            {
                Name = "second",
                Aliases = new List<string>
                {
                    "s"
                },
                Writer = ctx => ctx.ReceivedAt.Second.ToString()
            },
            ["millisecond"] = new EventMarker
            {
                Name = "millisecond",
                Aliases = new List<string>
                {
                    "ms"
                },
                Writer = ctx => ctx.ReceivedAt.Millisecond.ToString()
            },
            ["period"] = new EventMarker
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
            StringBuilder result = new StringBuilder();
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

    // loot is stuff like:
    // - sockets, which enhance your digital features
    // - new backpacks to store more stuff at a time
    // - transportation devices, which speed up travel time between places
    // - collectables in the real world, which can be traded for valuables in the digital
    // - CardTech, which enhances and modifies how your card is displayed
    //   CardTech items: // By default, your card color scheme is bound to your interface color scheme, unless you explicitly modify it
    //   - Color Schemes (Bind IDs to GammaColorMap)
    //   - Font faces (Bind IDs to FontFace)
    //   - Avatar animator (Simply a flag)
    //   - New layout templates, which change how your card is organized. (you can unlock an advanced card layout concept, which allows you to set your card to literally anything
    //   - Avatar canvas, which allows you to instead draw your avatar
    //   - Avatar size regulator, which allows you to modify the avatar appearance size on your card.
    //   - Status Templates, which change how your status is displayed color-wise
    //   - Level Templates, which change how your level is displayed
    //   - Exp Templates, which change how your current experience is displayed (exp template can be set referencing another template)
    //   - Username templates, which change how your name is displayed
    //   - Font Face Modifiers, which change how the font is written
    //   - Backgrounds, which are displayed behind a card.
    //   - Merit Slots, which allow you to display the icon of a merit onto a card
    // - Interface Enhancements, which modifies how Orikivo visually displays content to you
}
