using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo.Text;

namespace Orikivo
{
    public class GuideNode : ContentNode
    {
        public string Id { get; set; }
        
        public string Title { get; set; }

        public string Tooltip { get; set; }

        public List<GuideChapter> Chapters { get; set; } = new List<GuideChapter>();

        public string GetChapter(int page)
        {
            if (!Chapters.Any(x => x.Number == page))
                throw new ArgumentException("There were no chapters found with the specified number.");

            StringBuilder format = new StringBuilder();

            format.AppendLine($"> {Title}");

            foreach (GuideChapter chapter in Chapters.Where(x => x.Number < page))
                format.AppendLine($"**Part {chapter.Number}**: {chapter.Title}");

            if (Chapters.Any(x => x.Number < page))
                format.AppendLine();

            GuideChapter main = Chapters.First(x => x.Number == page);
            format.AppendLine($"**Part {main.Number}**: {main.Title}");
            format.AppendLine(main.Content);

            if (Chapters.Any(x => x.Number > page))
                format.AppendLine();

            foreach (GuideChapter chapter in Chapters.Where(x => x.Number > page))
                format.AppendLine($"**Part {chapter.Number}**: {chapter.Title}");

            return format.ToString();
        }


        protected override string Formatting
        {
            get
            {
                StringBuilder format = new StringBuilder();

                format.AppendLine($"> {Title}");

                foreach (GuideChapter chapter in Chapters)
                {
                    format.AppendLine($"**Part {chapter.Number}**: {chapter.Title}");
                    format.AppendLine(chapter.Content);
                }

                return format.ToString();
            }
        }
    }
}
