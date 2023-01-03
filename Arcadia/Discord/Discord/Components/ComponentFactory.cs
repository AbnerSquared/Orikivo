using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Arcadia
{
    public static class ComponentFactory
    {
        public static ComponentBuilder BuildQuestViewComponents(int index = 0)
        {
            var components = new ComponentBuilder();
            var nextPage = new ButtonBuilder("Next quest", "quest_next", ButtonStyle.Secondary);
            var previousPage = new ButtonBuilder("Previous page", "quest_previous", ButtonStyle.Secondary);

            var row = new ActionRowBuilder().WithButton(previousPage).WithButton(nextPage);

            components.AddRow(row);
            return components;
        }

        public static ComponentBuilder BuildGuideComponents(Guide guide, int index = 0)
        {
            var guideComponents = new ComponentBuilder();

            int count = guide.Pages.Count;

            var nextPage = new ButtonBuilder("Next page", "guide_next", ButtonStyle.Secondary, isDisabled: index + 1 == count);
            var previousPage = new ButtonBuilder("Previous page", "guide_previous", ButtonStyle.Secondary, isDisabled: index == 0);
            var firstPage = new ButtonBuilder("First page", "guide_first", ButtonStyle.Secondary, isDisabled: count <= 2);
            var lastPage = new ButtonBuilder("Last page", "guide_last", ButtonStyle.Secondary, isDisabled: count <= 2);
            // var jumpTo = new ButtonBuilder("Jump to", "guide_jump", ButtonStyle.Secondary, isDisabled: count <= 3);

            var row = new ActionRowBuilder().WithButton(firstPage).WithButton(previousPage).WithButton(nextPage).WithButton(lastPage);

            guideComponents.AddRow(row);
            return guideComponents;
        }

        public static async Task GuideButtonHandler(SocketMessageComponent component)
        {
            if (!component.Data.CustomId.StartsWith("guide_")) return;

            string content = component.Message.Content;
            Guide guide = GuideViewer.TryFindGuide(content);

            if (guide == null)
                throw new Exception("Unable to find a matching guide for the specified content");

            int index = GuideViewer.GetGuideContentIndex(content);
            int oldIndex = index;

            switch (component.Data.CustomId)
            {
                case "guide_previous":
                    index--;
                    break;
                case "guide_next":
                    index++;
                    break;
                case "guide_first":
                    index = 0;
                    break;
                case "guide_last":
                    index = guide.Pages.Count - 1;
                    break;
                case "guide_jump": // idk how to handle this yet
                    return;
                default:
                    return;
            }

            index = Math.Clamp(index, 0, guide.Pages.Count - 1);

            if (index == oldIndex) // ignore if we're on the same page lol
                return;

            var guideComponents = BuildGuideComponents(guide, index);
            var page = GuideViewer.GetGuidePage(guide, index);

            await component.UpdateAsync(x =>
            {
                x.Content = page;
                x.Components = guideComponents.Build();
            });

        }
    }
}
