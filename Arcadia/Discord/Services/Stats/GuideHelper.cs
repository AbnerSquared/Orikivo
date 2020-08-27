using System.Text;

namespace Arcadia
{
    public static class GuideHelper
    {
        public static string View(int page = 0)
        {
            var info = new StringBuilder();
            info.AppendLine("> 📚 **Guides**");
            info.AppendLine("> Learn more about the mechanics **Orikivo Arcade** uses.");

            info.AppendLine("\nThere aren't any guides available yet. Stay tuned!");

            return info.ToString();
        }
    }
}