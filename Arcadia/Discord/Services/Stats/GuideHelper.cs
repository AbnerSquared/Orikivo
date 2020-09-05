using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia
{
    public class Guide
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public List<string> Pages { get; set; } = new List<string>();
    }

    public static class GuideHelper
    {
        public static readonly List<Guide> Guides = new List<Guide>
        {
            new Guide
            {
                Id = "beginner",
                Icon = "🏳️",
                Title = "Beginning Your Journey",
                Summary = "Learn how to get started with **Orikivo Arcade**.",
                Pages = new List<string>
                {
                    $"> **Getting Started**\nWelcome to **Orikivo Arcade**! This is a bot that aims to provide a wide variety of unique ways to collect, game, and more! To initialize an account, simply call any basic command that would require one (eg. `card`, `stats`, `balance`), and your account will be automatically initialized!\n\n> **Earning Money**\nAs of now, there isn't too many ways to earn money with **Orikivo Arcade**, but the current 100% safe methods are:\n\n- Using `daily`, which rewards {Icons.Balance} **15** per day, with streak rewards\n- Using `assign` to be given new objectives, from which reward a random amount of {Icons.Balance} **Orite** based on its difficulty\n- Unlocking **Merits**, which are milestones based on what you've accomplished on **Orikivo Arcade** so far\n\nWhile those may be the safe methods, there are a few other riskier methods that can be done to earn more funds:\n- Using `gimi`, which can reward up to {Icons.Balance} **10** or {Icons.Debt} **10** based on chance\n\n> **What Is Debt?**\nIn order to keep the balance for funds in check, the implementation of {Icons.Debt} **Debt** was added. **Debt** is a negative form of money that automatically blocks all incoming funds until it is paid off. **Debt** is mainly received from the **Casino**, but can come as a surprise at random if you're not careful.\n\n> **Quests**\nQuests are a fun way to earn {Icons.Balance} **Orite** and {Icons.Exp} **EXP** by accomplishing random challenges that are provided to you. Each quest has a specified difficulty which determines the amount you receive.\n\nEveryone starts off with **1** quest slot, but as they keep accomplishing more and utilizing the bot, more slots become available, alongside a wider range of quests."
                }
            },

            new Guide
            {
                Id = "multiplayer",
                Icon = "⚔️",
                Title = "Multiplayer",
                Summary = "Learn how to use the multiplayer system.",
                Pages = new List<string>
                {
                    $"> {Icons.Warning} Please note that in some multiplayer games, you are required to have the option **Allow direct message from server members** enabled. This can be found in **Privacy & Safety/Server Privacy Defaults**.\n\nSo you've come here to learn about multiplayer? You're in the right place.\n\n> **Game Modes**\nAs of now, **Orikivo Arcade** has support for two game modes:\n- **Trivia** (Beta)\n- **Werewolf** (Beta)\n\n> {Icons.Warning} **Beware!**\nGames that are marked with `(Beta)` may have bugs and/or issues that could make the game unplayable. If a session becomes soft-locked, the server host can force end the session by typing `destroysession`.\n\n> **Hosting a Server**\nTo host a session, simply type `hostserver` to start up a default server. If you have a game in mind that you wish to play, type `hostserver <game_id>` instead to automatically launch the server for the specified game mode.\n\n> **Joining a Server**\nJoining a server has been made as easy as possible! To join an existing server, you can use the **Server Browser** to find a server to join (`servers [page]`), from which you can join by typing `joinserver <server_id>`. If you were invited to a server, you can view those invites by typing `invites`, and accepting the specified invite by its unique index (`acceptinvite <index>`). Likewise, if you just wish to quickly get into a game, you can type `quickjoin` or `quickjoin <game_id>` to hop into a random available server! You can also join existing servers by typing `join` (no prefix) in an existing server connection."
                }
            }
        };
        public static string View(int page = 0)
        {
            int pageSize = 5;
            int pageCount = (int)Math.Ceiling(Guides.Count / (double)pageSize) - 1;
            page = page < 0 ? 0 : page > pageCount ? pageCount : page;

            var info = new StringBuilder();
            info.AppendLine("> 📚 **Guides**");
            info.AppendLine("> Learn more about the mechanics **Orikivo Arcade** uses.");

            if (!Guides.Any())
                info.AppendLine("\nThere aren't any guides available yet. Stay tuned!");
            else
            {
                foreach (Guide guide in Guides.Skip(pageSize * page))
                {
                    info.AppendLine($"\n> `{guide.Id}`\n> {guide.Icon} **{guide.Title}** (**{guide.Pages.Count}** {Format.TryPluralize("page", guide.Pages.Count)})");
                    info.AppendLine($"> {guide.Summary}");
                }
            }

            return info.ToString();
        }

        public static string ViewGuide(string id, int page = 0)
        {
            Guide guide = Guides.FirstOrDefault(x => x.Id == id);

            if (guide == null)
                return Format.Warning("Could not find the specified guide.");

            page = page < 0 ? 0 : page > guide.Pages.Count - 1 ? guide.Pages.Count - 1 : page;

            var info = new StringBuilder();
            info.AppendLine($"> {guide.Icon ?? "📚"} **Guides: {guide.Title}** (Page **{page + 1}**/{guide.Pages.Count})\n");
            info.Append(guide.Pages[page]);

            return info.ToString();
        }
    }
}