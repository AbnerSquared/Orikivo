using System.Collections.Generic;
using System.Text;
using Orikivo;

namespace Arcadia.Modules
{
    public static class AboutViewer
    {

        public static readonly Dictionary<string, string> Pages = new Dictionary<string, string>
        {
            ["research"] = $""
        };

        public static string View()
        {
            var result = new StringBuilder();

            result.AppendLine($"> **Arcadia** (v**{ArcadeData.Version}**)");
            result.AppendLine("\n> **Q**: Wait, I thought this was **Orikivo Arcade**?\n> **A**: They're both the same!\n");
            result.AppendLine("> **Summary**");
            result.AppendLine("**Arcadia** is a alternative version of **Orikivo** that branched off to focus more on the multiplayer side. A greater emphasis was made for an economy-like structure that players can work together to climb the tops of, while also giving a slight peek into the story-like structure that **Orikivo** is planned to have.\n");

            // Statistics

            result.AppendLine("> **Web Portal**");
            result.AppendLine("• **Website** (**Beta**): <https://abnersquared.github.io/Orikivo.Web/>");
            result.AppendLine("• **Invite URL**: <https://abnersquared.github.io/Orikivo.Web/invites/arcadia>");

            // result.AppendLine();
            // result.AppendLine("> **Acknowledgements**");
            // Acknowledgements

            return result.ToString();
        }

        public static string ViewFor()
        {
            var result = new StringBuilder();
            return result.ToString();
        }
    }
}