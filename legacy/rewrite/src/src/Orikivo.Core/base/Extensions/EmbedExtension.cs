using Discord;
using Orikivo.Systems.Presets;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class EmbedExtension
    {
        public static EmbedBuilder WithLocalImageUrl(this EmbedBuilder e, string url)
        {
            return e.WithImageUrl($"attachment://{Path.GetFileName(url)}");
        }

        public static async Task SendLocalImageAsync()
        {

        }


        //,,,,,,,,,,,,,,,

        public static void AddPage(this List<EmbedBuilder> embeds, EmbedBuilder embed, StringBuilder sb)
        {
            sb.Debug("wth");
            embed.WithDescription(sb.ToString());
            embed.Description.Debug("embed");
            embeds.Add(embed);
            sb.Clear();
        }

        public static void Reset(this EmbedBuilder e, EmbedBuilder b)
        {
            e = b;
        }

        public static void Seal(this List<EmbedBuilder> embeds, EmbedBuilder embed, StringBuilder sb)
        {
            if (!(sb.Length > 0))
            {
                return;
            }

            embeds.AddPage(embed, sb);
        }
    }
}