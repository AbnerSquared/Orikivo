using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Orikivo;

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

        [JsonProperty("merge")]
        [Description("If you merged two **Palette** values, this will determine the direction at which they transition.")]
        public PaletteDirection? Direction { get; internal set; }

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

                string value = property.GetValue(this, null)?.ToString();
                if (Check.NotNull(value))
                    panel.Append(value);
                else
                    panel.Append("null");
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