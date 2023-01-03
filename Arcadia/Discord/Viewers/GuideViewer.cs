using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Orikivo;
using Orikivo.Text;
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
            return GetGuidePage(guide, page);
        }

        public static string GetGuidePage(Guide guide, int index = 0)
        {
            if (guide == null)
                return Format.Warning("Could not find the specified guide.");

            if (!Check.NotNullOrEmpty(guide.Pages))
                throw new Exception("Expected the specified guide to have at least a single page");

            index = Math.Clamp(index, 0, guide.Pages.Count - 1);

            var info = new StringBuilder();
            info.AppendLine($"{GetGuideContentPrefix(guide)} ({Format.PageCount(index + 1, guide.Pages.Count)})\n");
            info.Append(guide.Pages[index]);

            return info.ToString();
        }

        public static Guide TryFindGuide(string content)
        {
            if (!Check.NotNull(content)) return null;

            foreach (Guide guide in Assets.Guides)
            {
                if (content.StartsWith(GetGuideContentPrefix(guide)))
                    return guide;
            }

            return null;
        }

        public static int GetGuideContentIndex(string content)
        {
            foreach (Guide guide in Assets.Guides)
            {
                string contentPrefix = GetGuideContentPrefix(guide);

                if (content.StartsWith(contentPrefix))
                {
                    string indexPrefix = " (Page **";

                    string search = content[(contentPrefix.Length + indexPrefix.Length)..];

                    var reader = new StringReader(search);
                    string rawIndex = reader.ReadUntil('*');

                    return int.Parse(rawIndex) - 1;
                }
            }

            return 0;
        }

        public static string GetGuideContentPrefix(Guide guide)
        {
            return $"> {guide.Icon ?? "📚"} **Guide: {guide.Title}**";
        }
    }
}
