using System.Reflection;
using System.Text;
using Arcadia.Graphics;
using Newtonsoft.Json;
using Orikivo;
using PaletteType = Arcadia.Graphics.PaletteType;

namespace Arcadia
{
    public class CardConfig
    {
        public CardConfig() {}

        [JsonConstructor]
        internal CardConfig(ColorPalette palette, FontType font, LayoutType layout)
        {
            Palette = palette ?? new ColorPalette(PaletteType.Default);
            Font = font;
            Font = font;
            Layout = layout;
        }

        [ReadOnly]
        [JsonProperty("layout")]
        [Description("Defines the base structure of your **Card**.")]
        public LayoutType Layout { get; internal set; } = LayoutType.Default;

        [ReadOnly]
        [JsonProperty("palette")]
        [Description("Defines the **Palette** currently equipped to your **Card**.")]
        public ColorPalette Palette { get; internal set; }

        [ReadOnly]
        [JsonProperty("font")]
        [Description("Defines the **Font** used for the name on your **Card**.")]
        public FontType Font { get; internal set; } = FontType.Orikos;

        public string Display()
        {
            var panel = new StringBuilder();

            panel.AppendLine($"> **Card Properties**");

            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                panel.Append("> **");
                panel.Append(property.Name);
                panel.Append("** • `");

                var value = property.GetValue(this, null)?.ToString();

                panel.Append(Check.NotNull(value) ? value : "null");
                panel.AppendLine("`");

                string subtitle = property.GetCustomAttribute<DescriptionAttribute>()?.Content;

                if (Check.NotNull(subtitle))
                    panel.AppendLine($"> {subtitle}");

                panel.AppendLine();
            }

            return panel.ToString();
        }
    }
}