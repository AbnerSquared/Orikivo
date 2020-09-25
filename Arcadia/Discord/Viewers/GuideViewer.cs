using System;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public static class GuideViewer
    {
        private static readonly int _pageSize = 5;

        public static string View(int page = 0)
        {
            var info = new StringBuilder();
            info.AppendLine("> 📚 **Guides**");
            info.AppendLine("> Learn more about the mechanics **Orikivo Arcade** uses.");

            if (!Assets.Guides.Any())
                info.AppendLine("\nThere aren't any guides available yet. Stay tuned!");
            else
            {
                foreach (Guide guide in Paginate.GroupAt(Assets.Guides, page, _pageSize))
                {
                    info.AppendLine($"\n> `{guide.Id}`\n> {guide.Icon} **{guide.Title}** (**{guide.Pages.Count}** {Format.TryPluralize("page", guide.Pages.Count)})");
                    info.AppendLine($"> {guide.Summary}");
                }
            }

            return info.ToString();
        }

        public static string ViewGuide(string id, int page = 0)
        {
            Guide guide = Assets.Guides.FirstOrDefault(x => x.Id == id);

            if (guide == null)
                return Format.Warning("Could not find the specified guide.");

            if (!Check.NotNullOrEmpty(guide.Pages))
                throw new Exception("Expected the specified guide to have at least a single page");

            page = Math.Clamp(page, 0, guide.Pages.Count - 1);

            var info = new StringBuilder();
            info.AppendLine($"> {guide.Icon ?? "📚"} **Guides: {guide.Title}** ({Format.PageCount(page + 1, guide.Pages.Count)})\n");
            info.Append(guide.Pages[page]);

            return info.ToString();
        }
    }
}
