using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Orikivo;
using PaletteType = Arcadia.Graphics.PaletteType;

namespace Arcadia
{
    public class CardConfig
    {
        public CardConfig() {}

        [JsonConstructor]
        internal CardConfig(PaletteType palette)
        {
            Palette = palette;
        }


        [JsonProperty("palette")]
        [Description("Defines the **Palette** currently equipped to your **Card**.")]
        public PaletteType Palette { get; internal set; }

        public string Display()
        {
            var panel = new StringBuilder();

            panel.AppendLine($"> **Card Config**");

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