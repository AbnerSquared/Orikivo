using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia
{
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
                    "> **Getting Started**\nWelcome to **Orikivo Arcade**! This is a bot that aims to provide a wide variety of unique ways to collect, game, and more! To initialize an account, simply call any basic command that would require one (eg. `card`, `stats`, `balance`), and your account will be automatically initialized!\n\n> **Gaining XP**\nExperience is gained through playing multiplayer games, unlocking merits, completing quests, and through various other activities. You can view your current level at anytime by typing `level`. Raising you level provides access to additional mechanics and/or features that would otherwise be hidden.\n\n> :warning: **Notice**\nAscending is not currently implemented.\n\nWhen you reach level 100, you can ascend. Ascending resets your level back to 0, but raises your ascent by 1. The feature of this mechanics is yet to be explained.\n\n> **Earning Money**\nAs of now, there isn't too many ways to earn money with **Orikivo Arcade**, but the current 100% safe methods are:\n\n- Using `daily`, which rewards :money_with_wings: **15** per day, with streak rewards\n- Using `assign` to be given new objectives, from which reward a random amount of :money_with_wings: based on its difficulty\n- Unlocking **Merits**, which are milestones based on what you've accomplished on **Orikivo Arcade** so far\n\nWhile those may be the safe methods, there are a few other riskier methods that can be done to earn more funds:\n- Using `gimi`, which can reward up to :money_with_wings: **10** or :page_with_curl: **10** based on chance\n\n> **What Is Debt?**\nIn order to keep the balance for funds in check, the implementation of :page_with_curl: **Debt** was added. **Debt** is a negative form of money that automatically blocks all incoming funds until it is paid off. **Debt** is mainly received from the **Casino**, but can come as a surprise at random if you're not careful.",
                    "> **Merits**\nMerits are milestone achievements that can be unlocked by meeting certain criteria (usually provided as a hint for the merit's quote). Some merits reward :money_with_wings: **Orite**, while some can even reward items! As you unlock more merits, your **Merit Score** will increase.\n\nKeep in mind that there are more merits out there than meets the eye. Mess around and find hidden accomplishments!\n\n> **Boosters**\nBoosters are a good way to increase the amount of money you earn based on the activities you execute. Some can decrease the amount of :page_with_curl: **Debt** you receive, while others can boost the amount of :money_with_wings: **Orite** you earn! You can view your income rate at anytime by typing `boosters`.\n\nHowever, note that while some boosters can provide great benefits, others can bind you with a nasty inhibitor that may increase the amount of :page_with_curl: **Debt** you receive!\n\n> **Crafting**\nCertain items can be crafted once a collection of items have been found. Crafting can be used to create brand-new collectibles from existing ones, as well as increase the strength of certain base items!\n\n> **Card Design**\nA **Card** represents an overall summary of your account on **Orikivo Arcade**, and can be customized to your own desire. Change the color of your **Card**, set the font face to use, and set up new layouts! The ability to customize your card is planned to expand based on your progress. To view your card, type `card`. To view other cards, type `card <user>`.",
                    "> **Quests**\nQuests are a fun way to earn :money_with_wings: **Orite** and :small_red_triangle: **EXP** by accomplishing random challenges that are provided to you. Each quest has a specified difficulty which determines the amount you receive.\n\nEveryone starts off with **1** quest slot, but as they keep accomplishing more and utilizing the bot, more slots become available, alongside a wider range of quests.\n\n> **Profiles**\nProfiles are another alternative to a **Card**, and they are always available to view. You can view your profile by typing `profile`. You are also able to view another user's profile by typing `profile <user>`.\n\n> **Gifting and Trading**\nIf you wish to be a kind-hearted soul, you are allowed to gift items of any kind (provided they are tradable). To gift an item to other users, type `gift <slot_id | unique_id | item_id>`, and the best match for the selected item will be chosen.\n\nLikewise, if you would prefer to trade with other users instead, you can initialize a live trade by typing `trade <user>`. Once a trade has been opened, they will be notified, from which they are given the option to accept or decline the invitation. If a trade has been accepted, the trade menu is opened.\n\nIn the trade menu, the available commands (without **prefix**) are:\n- `ready`: Marks (or unmarks if already ready) the player that executed as ready\n- `inventory`: Opens up the player's inventory so that they may select items to trade\n- `cancel`: Cancels the current trade\n\nWhen an inventory is open in a trade, the player who has their inventory open may add an item by typing `<item_id> [amount]`, from which the default amount is 1 if unspecified. All untradable and locked items are hidden from the trade menu.\n\nOnce both players are marked as ready, the trade will go through and display the contents that both players have provided, successfully ending the trade.",
                    "> **Inventory Management**\nEach user has an inventory that can store a wide range of collectibles. Some items can be used to escape :page_with_curl: **Debt**, while others can change or alter how your **License** (or **Card**) is shown. Items can have unique use cases and properties that can alter your passive income, provide access to hidden commands and games, and more! You can view your inventory by typing `inventory [page]`.\n\n**Trade Offers**\nIf a player wishes to quickly send an offer instead, they may do so by typing `offer <user> <input>`, from which an offer is built as `<inbound> for <outbound>`. If you wish to only request items, you may do so by typing `offer <user> for <outbound>`. Otherwise, you may use `offer <user> <inbound> for <outbound>`. To view a list of possible offers, simply type `offers [page]`, where the default page is **1**.\n\nTrade offers are parsed by typing the item's ID alongside a specified amount, typed as `<item_id>,<amount>`. An example is `su_pl,1`, which translates to **Summon: Pocket Lawyer** (x**1**). If the amount is omitted, it is always set to **1** by default.\nAn example offer is `su_pl,2 p_gg for p_gl p_wu`, which translates to:\n\nYou will lose:\n- **Summon: Pocket Lawyer** (x**1**)\n- **Card Palette: Gamma Green** (x**1**)\n\nYou will gain:\n+ **Card Palette: Glass** (x**1**)\n+ **Card Palette: Wumpite** (x**1**)\n\nIf you find an offer that you wish to accept, you may do so by typing `acceptoffer <offer_id>`, from which the offer ID is shown in `offers [page]`.\n\n> :warning: **Keep In Mind**\nYou may only have up to **5** inbound and outbound offers at a time. Trade offers expire in **24**h, and are saved per session. If **Orikivo Arcade** were to suddenly go offline, the trade offers would not be stored.",
                    "> **Shops**\n**Orikivo Arcade** has an ever-so expanding range of shops available to the user! As you progress, more shops will become available, while other shops may expand their inventory!\n\nAll shops refresh their contents on the start of a new day (in **UTC** time). Certain shops may provide the ability to sell specific items, with the downside of certain deductions as tax. Most shops are designated to a specific set of items, while others can simply be a wild card. Each shop has a limited stock, so once you purchase everything in the store, you may not be able to purchase anything again until the shop is refreshed. As you fulfill certain requirements or milestones, shops will increase with what they tend to offer, most notably by adding more unique items in their item pool.\n\n> **Casino**\nThe **Casino** is a risky, but possible way to gain riches in this world! To get started, convert some of your :money_with_wings: **Orite** into :jigsaw: **Chips** (the official gambling currency) by typing `getchips <amount>`. For more information on how the :jigsaw: **Chip** rate conversion works, type `getchips` to read more details.\n\nAs of now, the **Casino** offers the following games:\n- **Doubler**, a game mode with plenty of risk and a very high possible reward\n\n> **Leaderboards**\nClimb to the top of the leaderboards that fall into several categories:\n- `money`, which ranks users based on the amount of :money_with_wings: **Orite** they have\n- `debt`, which ranks users based on the amount of :page_with_curl: **Debt** they have\n- `chips`, which ranks users based on the amount of :jigsaw: **Chips** they have\n- `merits`, which ranks users based on their **Merit Score** (total sum of achieved merits)\n\nIf you wish to view the leaderboard for a specific stat, you may do so by typing the ID of a stat in place of the query (eg. `leaderboard gimi:times_played`)!",
                    "> **Stats**\nTrack and keep note of all of your statistics for every game mode and activity in **Orikivo Arcade**! To view a list of all of your stats, type `stats [page]` to get started. To view a group of stats instead, you can type `statsof <stat_group>`, where the group is the first part of a stat.\n\n> **Configuration**\nDue to how complex **Orikivo Arcade** gets at times, there is a configuration panel that allows you to changes various settings. These include:\n- Tooltips\n- Notifications\n- Error Handling\n- Prefix\n\nTooltips are handy little tips that appear in most commands that you use to help guide you towards anything that you might want to do. To disable them, You can type `config tooltips false`, which will hide all of them. Keep in mind that this will hide all tips that appear, which can be confusing at times if you don't know how to use **Orikivo Arcade** just yet.\n\nNotifications are used to help notify a user about specific things. This can be fine-tuned for each user, so that they won't be overwhelmed with notifications on every command they execute.\n\nError handling determines how an error is shown to a user. It can be simplified to hide developer information to keep the focus solely on the bot. However, in order for the developers to figure out problems that might occur, it is recommended to at least leave it on **Simple**, so that the direct cause of the issue that occured can quickly be found.\n\nYour prefix is by default either set to the server's default, or the global default, which is `[`. If you wish to override this prefix with your own, you can type `config prefix <value>` to do so. Keep in mind that the limit for how long a prefix can be is 16 characters.\n\n> **Closing**\nWhile this doesn't exactly cover everything, this should hopefully provide a good kickstart on how to use **Orikivo Arcade** to the best of your extent. Get out there and have some fun!"
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
                    $"> {Icons.Warning} Please note that in some multiplayer games, you are required to have the option **Allow direct message from server members** enabled. This can be found in **Privacy & Safety/Server Privacy Defaults**.\n\nSo you've come here to learn about multiplayer? You're in the right place.\n\n> **Game Modes**\nYou can view all of the games **Orikivo Arcade** currently offers by typing `games`.\n\n> {Icons.Warning} **Beware!**\nGames that are marked with `(Beta)` may have bugs and/or issues that could make the game unplayable. If a session becomes soft-locked, the server host can force end the session by typing `destroysession`.\n\n> **Hosting a Server**\nTo host a session, simply type `hostserver` to start up a default server. If you have a game in mind that you wish to play, type `hostserver <game_id>` instead to automatically launch the server for the specified game mode.\n\n> **Joining a Server**\nJoining a server has been made as easy as possible! To join an existing server, you can use the **Server Browser** to find a server to join (`servers [page]`), from which you can join by typing `joinserver <server_id>`. If you were invited to a server, you can view those invites by typing `invites`, and accepting the specified invite by its unique index (`acceptinvite <index>`). Likewise, if you just wish to quickly get into a game, you can type `quickjoin` or `quickjoin <game_id>` to hop into a random available server! You can also join existing servers by typing `join` (no prefix) in an existing server connection.",
                }
            }
        };

        private static readonly int _pageSize = 5;

        public static string View(int page = 0)
        {
            var info = new StringBuilder();
            info.AppendLine("> 📚 **Guides**");
            info.AppendLine("> Learn more about the mechanics **Orikivo Arcade** uses.");

            if (!Guides.Any())
                info.AppendLine("\nThere aren't any guides available yet. Stay tuned!");
            else
            {
                foreach (Guide guide in Paginate.GroupAt(Guides, page, _pageSize))
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
