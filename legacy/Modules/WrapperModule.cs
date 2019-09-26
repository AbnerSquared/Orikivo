using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orikivo.Static;
using Orikivo.Systems.Presets;
using Orikivo.Systems.Services;

using Orikivo.Systems.Wrappers.Steam.Services;

using Orikivo.Utility;
using Orikivo.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orikivo.Modules
{
    [Name("API")]
    [Summary("Programs made portable as a collection of commands.")]
    [DontAutoLoad]
    public class WrapperModule : ModuleBase<OrikivoCommandContext>
    {
        /*
        [Command("overwatch"), Alias("ow")]
        [Summary("Get basic information from a user's profile using the Overwatch database.")]
        public async Task GetProfileAsync(string battleTag, string platformType = "pc")
        {
            OverwatchPlatforms platformChoice;
            switch (platformType)
            {
                case "pc":
                    platformChoice = OverwatchPlatforms.Pc;
                    break;
                case "xbl":
                    platformChoice = OverwatchPlatforms.Xbl;
                    break;
                case "psn":
                    platformChoice = OverwatchPlatforms.Psn;
                    break;
                default:
                    platformChoice = OverwatchPlatforms.Pc;
                    break;
            }

            var Overwatch = new OverwatchWrapper();

            var baseData = Overwatch.GetProfile(new OverwatchProfile() { BattleTag = battleTag, Region = OverwatchRegions.Us, Platform = platformChoice }).Result;
            var quickPlayData = baseData.QuickPlayStats;
            var quickPlayAwardData = quickPlayData.Awards;
            var quickPlayGameData = quickPlayData.Games;
            var competitiveData = baseData.CompetitiveStats;
            var competitiveAwardData = competitiveData.Awards;
            var competitiveGameData = competitiveData.Games;
            var rating = "";
            var ratingName = "";
            var ratingIcon = "";

            if (baseData.Rating == "")
            {
                ratingName = "Unranked";
                ratingIcon = baseData.PrestigeIcon;
            }
            else
            {
                ratingIcon = baseData.RatingIcon;
                ratingName = $"{baseData.RatingName}";
                rating = baseData.Rating + " SR ";
            }


            var embedProfile = new EmbedBuilder();
            var embedProfileFooter = new EmbedFooterBuilder();

            embedProfile.WithThumbnailUrl(ratingIcon);
            embedProfile.WithColor(new Color(249, 158, 26));
            embedProfile.WithDescription(
                $"**General**\n>> Username: `{baseData.Name}`\n>> Rating: `{rating}({ratingName})`\n>> Level: `{baseData.Level}`\n>> Prestige: `{baseData.Prestige}`\n\n" +
                $"**Quick Play**\n>> Games Won: `{quickPlayGameData.Won}`\n>> Medals Earned: `{quickPlayAwardData.Medals}`\n>> Bronze Medals: `{quickPlayAwardData.MedalsBronze}`\n>> Silver Medals: `{quickPlayAwardData.MedalsSilver}` \n>> Gold Medals: `{quickPlayAwardData.MedalsGold}`\n\n" +
                $"**Competitive**\n>> Games Won: `{competitiveGameData.Won}` \n>> Medals Earned: `{competitiveAwardData.Medals}` \n>> Bronze Medals: `{competitiveAwardData.MedalsBronze}` \n>> Silver Medals: `{competitiveAwardData.MedalsSilver}` \n>> Gold Medals: `{competitiveAwardData.MedalsGold}`");

            embedProfileFooter.WithIconUrl(baseData.Icon);
            embedProfileFooter.WithText(baseData.Name + $" ({platformType.ToUpper()})");
            await ReplyAsync("", false, embedProfile.Build());
        }
        */

        [Group("dbli"), Name("DiscordBotList")]
        public class DiscordBotListGroup : ModuleBase<OrikivoCommandContext>
        {
            private DiscordSocketClient _client;
            private CommandService _service;
            private IConfigurationRoot _config;
            private LockedDblWrapper _dbl;

            private const ulong _mainBotId = 433079994164576268;
            private const ulong _ownerId = 181605794159001601;
            private const string _mainBotIdString = "433079994164576268";
            private const string _ownerIdString = "181605794159001601";

            public DiscordBotListGroup(DiscordSocketClient client, CommandService service, IConfigurationRoot config)
            {
                _client = client;
                _service = service;
                _config = config;
            }

            public void TryLoadDblWrapper()
            {
                if (!_dbl.Exists())
                {
                    _dbl = new LockedDblWrapper(_client.CurrentUser.Id, _config["api:dbl"]);
                }
            }

            // SMALL
            // botId
            // noavatar
            // avatarbg
            // leftcolor
            // lefttextcolor
            // rightcolor
            // righttextcolor

            // LARGE
            // botid
            // topcolor
            // middlecolor
            // usernamecolor
            // certifiedcolor
            // datacolor
            // labelcolor
            // highlightcolor

            [Command("largewidget")]
            public async Task GetLargeWidgetAsync(ulong botId = _mainBotId, string topColor = "",
                string middleColor = "", string usernameColor = "", string certifiedColor = "",
                string dataColor = "", string labelColor = "", string highlightColor = "")
            {
                await Context.Channel.SendEmbedAsync(Embedder.DefaultEmbed.WithImageUrl(new LargeWidgetOptions(topColor, middleColor, usernameColor,
                    certifiedColor, dataColor, labelColor, highlightColor).WithImageFormat(WidgetImageFormat.Png).WithBotId(botId).Read()).Build());
            }
            [Command("smallwidget")]
            public async Task GetSmallWidgetAsync(WidgetType type = WidgetType.Status, ulong botId = _mainBotId,
                string avatarBg = "", string leftColor = "",string rightColor = "", string leftTextColor = "", string rightTextColor = "")
            {
                await Context.Channel.SendEmbedAsync(Embedder.DefaultEmbed.WithImageUrl(new SmallWidgetOptions(avatarBg, leftColor, leftTextColor,
                    rightColor, rightTextColor).WithImageFormat(WidgetImageFormat.Png).WithType(type).WithBotId(botId).Read()).Build());
            }
            
            [Command("voted")]
            public async Task CheckUserVoteResponseAsync(SocketUser u = null)
            {
                await ModuleManager.TryExecute(Context.Channel, CheckUserVoteAsync(u));
            }

            public async Task CheckUserVoteAsync(SocketUser u = null)
            {
                u = u ?? Context.User;

                TryLoadDblWrapper();
                await ReplyAsync(embed: BuildDblVotedEmbed(u));
            }

            [Command("user")]
            public async Task CheckUserAsync([Remainder]string user = "")
            {
                TryLoadDblWrapper();
                SocketGuildUser u = Context.GetGuildUser();
                if (!string.IsNullOrWhiteSpace(user))
                {
                    if (!Context.Guild.TryGetUser(user, out u))
                    {
                        if (!ulong.TryParse(user, out ulong id))
                        {
                            await ReplyAsync(embed: EmbedData.Throw(Context, "Could not parse ulong.", "Make sure you use the ID of the user you wish to search for if they are outside of this guild."));
                            return;
                        }

                        await ReplyAsync(embed: ReadDblUser(id));
                        return;
                    }
                }

                await ReplyAsync(embed: ReadDblUser(u));
            }

            [Command("bot")]
            public async Task CheckBotAsync([Remainder]string bot = "")
            {
                TryLoadDblWrapper();
                SocketGuildUser b = Context.Guild.GetUser(_mainBotId);
                if (!string.IsNullOrWhiteSpace(bot))
                {
                    if (!Context.Guild.TryGetUser(bot, out b))
                    {
                        if (!ulong.TryParse(bot, out ulong id))
                        {
                            await ReplyAsync(embed: EmbedData.Throw(Context, "Could not parse ulong.", "Make sure you use the ID of the bot you wish to search for if they are outside of this guild."));
                            return;
                        }

                        await ReplyAsync(embed: ReadDblBot(id));
                        return;
                    }
                }

                await ReplyAsync(embed: ReadDblBot(b));
            }

            [Command("tags")]
            [Summary("Get all of the current tags in use on Discord Bot List.")]
            public async Task GetTagsAsync()
            {
                await Embedder.SendEmbedAsync(Context.Channel, GetDblTags());
            }

            public Embed GetDblTags()
            {
                List<string> genericTags = new List<string>
                {
                    "Music", "Moderation", "Fun", "Economy", "Meme", "Social",
                    "Leveling", "Roleplay", "Role Management", "Logging", "Web Dashboard",
                    "Stream", "Crypto", "Media", "Turkish", "Soundboard", "Utility",
                    "Customizable Behavior"
                };
                List<string> gameTags = new List<string>
                {
                    "Game", "PUBG", "Rocket League", "Fortnite", "Overwatch", "CSGO",
                    "OSU!", "Minecraft", "League of Legends", "Warframe", "Diablo III",
                    "Rust", "DOTA 2", "Starcraft 2", "Factorio", "Runescape", "Apex Legends",
                };
                EmbedBuilder eb = EmbedData.DefaultEmbed;
                eb.WithTitle("**Tags**");
                eb.WithDescription(genericTags.Enumerate(x => Format.Code(x)).Conjoin(" "));
                eb.WithFooter($"{EmojiIndex.Counter.Format(IconFormat.Escaped)}{genericTags.Count().ToPlaceValue()}");
                return eb.Build();
            }

            public Embed ReadDblUser(ulong id)
            {
                TryLoadDblWrapper();
                IDblUser user = _dbl.GetUserAsync(id).Result;
                if (!user.Exists())
                    user = _dbl.GetUserAsync(_ownerId).Result;
                EmbedBuilder eb = EmbedData.DefaultEmbed;
                if (!user.Exists())
                {
                    eb.WithColor(EmbedData.GetColor("error"));
                    eb.WithDescription($"{id} does not have an account on **Discord Bot List**.");
                    return eb.Build();
                }
                eb = BuildDblUserEmbed(user, eb);
                return eb.Build();
            }

            public Embed ReadDblUser(SocketUser u)
            {
                TryLoadDblWrapper();
                IDblUser user = _dbl.GetUserAsync(u.Id).Result;
                if (!user.Exists())
                    user = _dbl.GetUserAsync(_ownerId).Result;

                EmbedBuilder eb = EmbedData.DefaultEmbed;
                if (!user.Exists())
                {
                    eb.WithColor(EmbedData.GetColor("error"));
                    eb.WithDescription($"{u.Username} does not have an account on **Discord Bot List**.");
                    return eb.Build();
                }
                eb = BuildDblUserEmbed(user, eb);
                return eb.Build();
            }

            public Embed ReadDblBot(SocketUser u)
            {
                TryLoadDblWrapper();
                EmbedBuilder eb = EmbedData.DefaultEmbed;
                if (!u.IsBot)
                {
                    eb.WithColor(EmbedData.GetColor("error"));
                    eb.WithDescription($"{u.Username} is not a bot.");
                    return eb.Build();
                }
                IDblBot bot = _dbl.GetBotAsync(u.Id).Result;
                
                if (!bot.Exists())
                {
                    eb.WithColor(EmbedData.GetColor("error"));
                    eb.WithDescription($"{u.Username} is not associated with **Discord Bot List**.");
                    return eb.Build();
                }
                eb = BuildDblBotEmbed(bot, eb);
                return eb.Build();
            }

            public Embed ReadDblBot(ulong id)
            {
                TryLoadDblWrapper();
                IDblBot bot = _dbl.GetBotAsync(id).Result;
                EmbedBuilder eb = EmbedData.DefaultEmbed;
                if (!bot.Exists())
                {
                    eb.WithColor(EmbedData.GetColor("error"));
                    eb.WithDescription($"{id} does not lead to any valid bot on **Discord Bot List**.");
                    return eb.Build();
                }
                eb = BuildDblBotEmbed(bot, eb);
                return eb.Build();
            }

            public Embed BuildDblVotedEmbed(SocketUser user)
            {
                
                EmbedBuilder eb = Embedder.DefaultEmbed;
                bool isSelf = user == Context.User;
                string userName = isSelf ? "You" : $"**{user.Username}**#{user.Discriminator}";

                if (user.IsBot)
                {
                    eb.WithColor(EmbedData.GetColor("error"));
                    return eb.WithTitle($"`?` {userName} {(isSelf ? "are" : "is")} a bot, and cannot vote.").Build();
                }

                bool voted = _dbl.HasVotedAsync(user.Id).Result;
                if (voted)
                    return eb.WithTitle($"`✔` {userName} ha{(isSelf ? "ve" : "s")} voted for **{_client.CurrentUser.Username}**.").Build();
                eb.WithColor(EmbedData.GetColor("error"));
                return eb.WithTitle($"`❌` {userName} ha{(isSelf ? "ve" : "s")} not voted for **{_client.CurrentUser.Username}**.").Build();
            }

            public EmbedBuilder BuildDblUserEmbed(IDblUser user, EmbedBuilder eb = null)
            {
                eb = eb ?? EmbedData.DefaultEmbed;

                eb.WithTitle($"**{user.Username}**#{user.Discriminator}");
                StringBuilder str = new StringBuilder();
                //str.AppendLine($"**ID**: `{user.Id}`");

                List<string> ranks = new List<string>();
                if (user.IsAdmin)
                    ranks.Add("Admin");
                if (user.IsMod)
                    ranks.Add("Moderator");
                if (user.IsWebMod)
                    ranks.Add("Web Moderator");
                if (user.IsCertified)
                    ranks.Add("Certified");
                if (user.IsSupporter)
                    ranks.Add("Supporter");

                if (ranks.Funct())
                {
                    str.AppendLine($"**\\*** {ranks.Enumerate(x => Format.Code(x)).Conjoin(" ")}");
                }
                //str.Append("");
                str.Append($"```{user.Biography}```");
                List<string> urls = new List<string>();

                urls.Add(Format.Url("Page", user.VanityUrl));
                if (user.Social.Exists())
                {
                    if (!string.IsNullOrWhiteSpace(user.Social.GithubUsername))
                        urls.Add(Format.Url("Github", user.Social.GithubUrl));
                    if (!string.IsNullOrWhiteSpace(user.Social.InstagramUsername))
                        urls.Add(Format.Url("Instagram", user.Social.InstagramUrl));
                    if (!string.IsNullOrWhiteSpace(user.Social.RedditUsername))
                        urls.Add(Format.Url("Reddit", user.Social.RedditUrl));
                    if (!string.IsNullOrWhiteSpace(user.Social.YoutubeId))
                        urls.Add(Format.Url("YouTube", user.Social.YoutubeUrl));
                } // [0] => #
                if (urls.Funct())
                {
                    str.AppendLine(urls.Conjoin(" • ").MarkdownBold());
                }

                if (!string.IsNullOrWhiteSpace(user.ColorHex))
                    if (user.ColorHex.Length != 1)
                        eb.WithColor(System.Drawing.ColorTranslator.FromHtml(user.ColorHex).ToDiscordColor());
                eb.WithDescription(str.ToString());
                eb.WithFooter($"🆔 {user.Id}{(user.ColorHex.Length == 1 ? "" : $" | {EmojiIndex.Hex} {user.ColorHex}")}");
                if (user.BannerUrl.Exists())
                    eb.WithImageUrl(user.BannerUrl);
                return eb;
            }

            public EmbedBuilder BuildDblBotEmbed(IDblBot bot, EmbedBuilder eb = null)
            {
                eb = eb ?? EmbedData.DefaultEmbed;
                eb.WithTitle($"{(bot.IsCertified ? "☑ ":"")}{bot.Username.MarkdownBold()}#{bot.Discriminator}");
                StringBuilder str = new StringBuilder();
               
                if (!string.IsNullOrWhiteSpace(bot.FlavorText))
                    str.Append($"```{bot.FlavorText}```");
                if (bot.Tags.Funct())
                    str.Append($"{"#".MarkdownBold()} {bot.Tags.Enumerate(x => Format.Code(x)).Conjoin(" ")}");
                if (!string.IsNullOrWhiteSpace(bot.FlavorText))
                    str.Append("\n");
                Utility.Debugger.Write("-- Retrieving stats... --");
                IDblBotStats stats = bot.GetStatsAsync().Result;
                Utility.Debugger.Write("-- Stats retrieved... --");
                str.AppendLine($"{EmojiIndex.FromHours(bot.ApprovedAt.Hour)} Approved {bot.ApprovedAt.ToFullOriTime()}"); // dont forget bold
                if (stats.Exists())
                {
                    if (stats.GuildCount > 0)
                    {
                        str.Append($"🔹Serving **{stats.GuildCount.ToPlaceValue()}** Guild{(stats.GuildCount == 1 ? "" : "s")}");
                        if (stats.ShardCount.Exists())
                            if (stats.ShardCount > 0)
                                str.Append($" [Across {stats.ShardCount.ToPlaceValue().MarkdownBold()} Shard{(stats.ShardCount > 1 ? "s" : "")}]");
                        str.Append("\n");
                    }
                }
                str.AppendLine($"🔺**{bot.Upvotes.ToPlaceValue()}** Upvotes{(bot.MonthlyUpvotes > 0 ? $" [**{bot.MonthlyUpvotes.ToPlaceValue()}** This Month]":"")}");

                List<string> links = new List<string>();
                if (!string.IsNullOrWhiteSpace(bot.VanityUrl))
                    links.Add($"[Page]({bot.VanityUrl})");
                if (!string.IsNullOrWhiteSpace(bot.InviteUrl))
                    links.Add($"[Invite]({bot.InviteUrl})");
                if (!string.IsNullOrWhiteSpace(bot.SupportUrl))
                    links.Add($"[Support]({bot.SupportUrl})");
                if (!string.IsNullOrWhiteSpace(bot.WebsiteUrl))
                    links.Add($"[Website]({bot.WebsiteUrl})");
                if (!string.IsNullOrWhiteSpace(bot.GithubUrl))
                    links.Add($"[Github]({bot.GithubUrl})");

                if (links.Funct())
                    str.Append($"{links.Conjoin(" • ").MarkdownBold()}");

                //eb.WithImageUrl(bot.AvatarUrl);
                eb.WithDescription(str.ToString());
                string mainOwnerName = $"{bot.Owners[0]}";
                SocketUser mainOwner = _client.GetUser(bot.Owners[0]);
                if (mainOwner.Exists())
                    mainOwnerName = $"{mainOwner.Username}#{mainOwner.Discriminator}";
                eb.WithFooter($"{EmojiIndex.Prefix}{bot.Prefix} | {EmojiIndex.Identifier} {bot.Id} | {EmojiIndex.Owner} {mainOwnerName} | 📚 {bot.Library}");
                return eb;
            }
        }
        /*
        [Group("steam"), Name("Steam")]
        [Summary("An entire system dedicated for using the Steam API.")]
        public class SteamCommandGroup : ModuleBase<OrikivoCommandContext>
        {
            private readonly CommandService _service;
            private readonly DiscordSocketClient _socket;
            private readonly IConfigurationRoot _config;

            public SteamCommandGroup(CommandService service, IConfigurationRoot config, DiscordSocketClient socket)
            {
                _service = service;
                _config = config;
                _socket = socket;
            }

            [Group("application"), Name("Steam.Applications"), Alias("app")]
            [Summary("An entire system dedicated to using Steam methods. In the works.")]
            public class SteamDisplayApps : ModuleBase<OrikivoCommandContext>
            {
                private readonly CommandService _service;
                private readonly DiscordSocketClient _socket;
                private readonly IConfigurationRoot _config;

                public SteamDisplayApps(CommandService service, IConfigurationRoot config, DiscordSocketClient socket)
                {
                    _service = service;
                    _config = config;
                    _socket = socket;
                }



                [Command("")]
                [Summary("Get detailed information about the available Steam application.")]
                public async Task GrabAppInformation(int appId, [Remainder]string detailType = null)
                {
                    var repair = new TextRepair();
                    var steam = new SteamAppInformation();

                    var appBase = steam.GetAppInfo($"{appId}").Result.Values.ToList()[0];
                    var appInfo = appBase.ApplicationData;
                    var appCorrectlyStored = appBase.ApplicationSuccess;

                    var color = EmbedData.GetColor("steam");
                    var errcolor = EmbedData.GetColor("steamerror");

                    if (appCorrectlyStored == false)
                    {
                        var eAppNullInfo = new EmbedBuilder();
                        var eAppInfoNullFooter = new EmbedFooterBuilder();
                        eAppInfoNullFooter.WithText($"Application ID [{appId}]");
                        eAppNullInfo.WithTitle("Application ID Error!");
                        eAppNullInfo.WithDescription("The application ID you have called for does not exist in the database, or may have been removed.");
                        eAppNullInfo.WithColor(errcolor);
                        eAppNullInfo.WithCurrentTimestamp();
                        await ReplyAsync(null, false, eAppNullInfo.Build());
                        return;
                    }

                    var primarySummary = Regex.Unescape(repair.CleanHtml(appInfo.Summary));
                    if (primarySummary.Length > 1024)
                    {
                        if (primarySummary.Substring(0, 950).EndsWith(' '))
                        {
                            primarySummary = primarySummary.Substring(0, 950).TrimEnd(' ') + "... \n[(Read more)](https://store.steampowered.com/app/" + $"{appId})";
                        }
                        else
                        {
                            primarySummary = primarySummary.Substring(0, 950) + "... \n[(Read more)](https://store.steampowered.com/app/" + $"{appId})"; ;
                        }
                    }

                    var priceDisplay = "";
                    if (!appInfo.FreeCheck && !appInfo.ReleaseDate.Unreleased)
                    {
                        var price = appInfo.Price.Initial;

                        if (price.Length > 6)
                        {
                            priceDisplay = $"${price.Substring(0, 2)},{price.Substring(2, 3)}.{price.Substring(5)}";
                        }
                        else if (price.Length > 5)
                        {
                            priceDisplay = $"${price.Substring(0, 1)},{price.Substring(1, 3)}.{price.Substring(4)}";
                        }
                        else if (price.Length > 4)
                        {
                            priceDisplay = $"${price.Substring(0, 3)}.{price.Substring(3)}";
                        }
                        else if (price.Length > 3)
                        {
                            priceDisplay = $"${price.Substring(0, 2)}.{price.Substring(2)}";
                        }
                        else if (price.Length > 2)
                        {
                            priceDisplay = $"${price.Substring(0, 1)}.{price.Substring(1)}";
                        }
                        else if (price.Length > 1)
                        {
                            priceDisplay = $"$0.{price}";
                        }
                    }
                    else
                    {
                        if (appInfo.ReleaseDate.Unreleased)
                        {
                            priceDisplay = $"Coming Soon ({appInfo.ReleaseDate.Date})";
                        }
                        else
                        {
                            priceDisplay = "Free-to-Play";
                        }


                    }

                    var siteLink = "";
                    if (appInfo.Website != null)
                    {
                        if (appInfo.Website.EndsWith('/'))
                        {
                            siteLink = appInfo.Website.TrimEnd('/');
                            siteLink = $"[{siteLink}]";
                        }
                    }



                    var eAppInfo = new EmbedBuilder();
                    var eAppInfoFooter = new EmbedFooterBuilder();
                    eAppInfo.WithFooter(eAppInfoFooter);
                    eAppInfoFooter.WithText($"{priceDisplay} {siteLink}");
                    eAppInfo.WithTitle($"{appInfo.Name} : A {appInfo.Type} made by {appInfo.Developers[0]}");
                    eAppInfo.WithDescription($"**Primary Genre**\n{appInfo.Genres[0].Description}");
                    eAppInfo.WithColor(0, 174, 239);
                    eAppInfo.WithImageUrl($"{appInfo.HeaderImage}");
                    eAppInfo.AddField(x => {
                        x.Name = "Summary";
                        x.Value = $"{primarySummary}";
                        x.IsInline = false;
                    });

                    if (detailType == null)
                    {
                        await ReplyAsync(null, false, eAppInfo.Build());
                    }
                    else
                    {
                        var detailSpec = detailType.Replace("scr", "screenshots");


                        if (detailSpec.StartsWith("screenshots"))
                        {
                            if (detailSpec.EndsWith("screenshots"))
                            {
                                detailSpec = "screenshots #";
                            }

                            var splitDetails = detailSpec.Split(' ');
                            var scrNumCheck = int.TryParse(splitDetails[1], out int screenshotNumber);
                            var eAppScreenshotDisplay = new EmbedBuilder();
                            var eAppScreenshotFooter = new EmbedFooterBuilder();
                            if (scrNumCheck)
                            {

                                var scrPos = screenshotNumber - 1;
                                var scrCount = appInfo.Screenshots.Length;
                                eAppScreenshotFooter.WithText($"{appInfo.Name} [Screenshot {screenshotNumber} of {scrCount}]");
                                eAppScreenshotDisplay.WithImageUrl(appInfo.Screenshots[scrPos].Url);
                                eAppScreenshotDisplay.WithFooter(eAppScreenshotFooter);
                                await ReplyAsync(null, false, eAppScreenshotDisplay.Build());
                            }
                            else
                            {
                                var scrCount = appInfo.Screenshots.Length;
                                eAppScreenshotFooter.WithText($"{appInfo.Name} [Screenshot {1} of {scrCount}]");
                                eAppScreenshotDisplay.WithImageUrl(appInfo.Screenshots[0].Url);
                                eAppScreenshotDisplay.WithFooter(eAppScreenshotFooter);
                                await ReplyAsync(null, false, eAppScreenshotDisplay.Build());
                            }
                        }
                        else
                        {
                            var eAppNullDetails = new EmbedBuilder();
                            eAppNullDetails.WithTitle("Detail Method Error!");
                            eAppNullDetails.WithDescription("The specification of the application ID you attempted to use was incorrect.");
                            eAppNullDetails.WithColor(186, 5, 97);
                            eAppNullDetails.WithCurrentTimestamp();
                            await ReplyAsync(null, false, eAppNullDetails.Build());
                        }
                    }


                }

                [Command("list")]
                [Summary("Returns the complete list of all available Steam applications.")]
                public async Task GrabAppList(int page = 0)
                {
                    var usePage = page;
                    if (page <= 0)
                    {
                        usePage = 1;
                    }

                    //var steam = new SteamApps(_config["api:steam"]);
                    var appList = steam.GetAppList(2).Result.Apps.AppInformation;
                    var eAppList = new EmbedBuilder();
                    eAppList.WithColor(0, 174, 239);
                    var fieldDictionary = new Dictionary<string, string>();
                    var fieldCount = 0;
                    var field = "";
                    var pageResultMax = 100 * usePage;
                    var sendResults = 100 * (usePage - 1);
                    var startingPoint = sendResults;
                    var sendSectionCount = 0;
                    var appCount = 0;

                    foreach (var app in appList)
                    {
                        var name = app.ApplicationName;
                        var id = app.ApplicationID;
                        if (startingPoint == 0)
                        {
                            if (sendResults < pageResultMax || field != "")
                            {
                                if (sendSectionCount < 5)
                                {
                                    var descLimit = field.Length + name.Length + id.ToString().Length;
                                    if (descLimit < 1024)
                                    {
                                        var section = $"`[{id}] {name}`\n";
                                        field += section;
                                        Console.WriteLine($"Added to [f{fieldCount}D].");
                                    }
                                    sendSectionCount += 1;
                                    sendResults += 1;
                                    Console.WriteLine($"[f{fieldCount}D] => [{sendSectionCount}/5] [{sendResults}/{pageResultMax}]");
                                }
                                else
                                {
                                    sendSectionCount = 0;
                                    fieldDictionary.Add($"f{fieldCount + 1}D", field);
                                    Console.WriteLine($"Out of room! Sending field information to the dictionary as f{fieldCount + 1}D.");
                                    field = "";
                                    fieldCount += 1;
                                    Console.WriteLine($"[f{fieldCount}D] => [{sendSectionCount}/5] [{sendResults}/{pageResultMax}]");
                                }
                            }
                        }
                        else
                        {
                            startingPoint -= 1;
                        }
                        appCount += 1;
                    }
                    var eAppListFooter = new EmbedFooterBuilder();
                    eAppListFooter.WithText($"There are currently {appCount} available games on Steam.");
                    eAppList.WithFooter(eAppListFooter);
                    eAppList.WithTitle($"[{sendResults - 99}-{pageResultMax}/{appCount}] Page {usePage} of {appCount / 100}");
                    foreach (var fPage in fieldDictionary)
                    {
                        eAppList.AddField(x =>
                        {
                            x.Name = $"Segment [{fieldDictionary.ToList().IndexOf(fPage) + 1 + (20 * (usePage - 1))}]";
                            x.Value = fPage.Value;
                            Console.WriteLine($"Added [{fPage.Key}] into a new field.");
                            x.IsInline = true;
                        });
                    }
                    if (usePage > appCount / 100)
                    {
                        var eAppPageOverflow = new EmbedBuilder();
                        eAppPageOverflow.WithFooter(eAppListFooter);
                        eAppPageOverflow.WithColor(186, 5, 97);
                        eAppPageOverflow.WithTitle("Page Overflow!");
                        eAppPageOverflow.WithDescription($"The current page you called for went over the current limit. [{appCount / 100}]");
                        await ReplyAsync(null, false, eAppPageOverflow.Build());
                        return;
                    }
                    await ReplyAsync(null, false, eAppList.Build());
                }
            }
        }
        */

        /*
        [Group("giphy"), Name("Giphy"), Alias("g")]
        [Summary("A group that contains a system built for Giphy.")]
        public class Giphy : ModuleBase<OrikivoCommandContext>
        {
            private readonly CommandService _service;
            private readonly DiscordSocketClient _socket;
            private readonly IConfigurationRoot _config;

            public Giphy(CommandService service,
                DiscordSocketClient socket,
                IConfigurationRoot config)
            {
                _service = service;
                _socket = socket;
                _config = config;
            }

            [Command("getid"), Alias("id")]
            [Summary("A command that gets a gif with the provided id.")]
            public async Task GetGiphyId([Remainder]string giphyId)
            {
                var giphy = new GiphyWrapper(_config["api:giphy"]);
                var searchReturn = await giphy.GetGifByIdType(giphyId);
                var embedGifId = new EmbedBuilder
                {
                    ImageUrl = searchReturn.Data.Images.Original.Url
                };
                await ReplyAsync("", false, embedGifId.Build());
            }

            [Command("search"), Alias("s")]
            [Summary("A command that searches for a gif on Giphy with the provided tag, tag position, and limit.")]
            public async Task SearchGiphy(string giphyTag, int tagCount, [Remainder]int searchLimit)
            {
                var defaultRating = GiphyRatings.Pg13;
                if (searchLimit == 0)
                {
                    searchLimit = 25;
                }
                var giphy = new GiphyWrapper(_config["api:giphy"]);
                var searchReturn = await giphy.SearchGif(new GiphySearch()
                {
                    Query = giphyTag,
                    Limit = searchLimit,
                    Rating = defaultRating
                });
                var embedGifId = new EmbedBuilder
                {
                    ImageUrl = searchReturn.Data[tagCount].Images.Original.Url
                };
                await ReplyAsync("", false, embedGifId.Build());
            }

            [Command("translate"), Alias("tl")]
            [Summary("A command that translates the term provided into a gif.")]
            public async Task TranslateGiphy([Remainder]string giphyTerm)
            {
                var defaultRating = GiphyRatings.Pg13;
                var giphy = new GiphyWrapper(_config["api:giphy"]);
                var translateReturn = await giphy.TranslateGif(new GiphyTranslation()
                {
                    Phrase = giphyTerm,
                    Rating = defaultRating
                });
                var embedGifId = new EmbedBuilder
                {
                    ImageUrl = translateReturn.Data.Images.Original.Url
                };
                await ReplyAsync("", false, embedGifId.Build());
            }

            [Command("trending"), Alias("tr")]
            [Summary("A command that searches for a gif on Giphy with the provided tag.")]
            public async Task TrendingGiphy(int trendNumber, [Remainder]int trendLimit)
            {
                if (trendLimit == 0)
                {
                    trendLimit = 25;
                }
                var defaultRating = GiphyRatings.Pg;
                var giphy = new GiphyWrapper(_config["api:giphy"]);
                var trendingReturn = await giphy.GetGifByTrendingTypes(new GiphyTrending()
                {
                    Limit = trendLimit,
                    Rating = defaultRating
                });
                var embedGif = new EmbedBuilder
                {
                    ImageUrl = trendingReturn.Data[trendNumber].Images.Original.Url
                };
                await ReplyAsync("", false, embedGif.Build());
            }

            [Command("random"), Alias("r")]
            [Summary("A command that gets a random gif from Giphy using a term.")]
            public async Task GetRandom([Remainder]string term)
            {
                var defaultRating = GiphyRatings.Pg13;
                var giphy = new GiphyWrapper(_config["api:giphy"]);
                var randomReturn = await giphy.GetGifRandomly(new GiphyRandom() { Tag = term, Rating = defaultRating });
                var embedGif = new EmbedBuilder
                {
                    ImageUrl = randomReturn.Data.ImageOriginalUrl
                };
                await ReplyAsync("", false, embedGif.Build());
            }
        }
        */
        /*
        [Group("e926"), Name("E926")]
        [Summary("An API service dedicated to e926.")]
        public class E926 : ModuleBase<OrikivoCommandContext>
        {
            private readonly CommandService _service;
            private readonly DiscordSocketClient _socket;
            private readonly IConfigurationRoot _config;

            public E926(CommandService service,
                DiscordSocketClient socket,
                IConfigurationRoot config)
            {
                _service = service;
                _socket = socket;
                _config = config;
            }

            [Command("search"), Alias("s")]
            [Summary("Searches on the E926 database for a specified tag.")]
            public async Task SearchForE926(string searchTag, int page = 1, int limit = -1)
            {
                var e = EmbedData.SetPostEmbed(EmbedData.EmbedCollection.Default);
                var e926 = new E926Wrapper();
                searchTag = searchTag.Replace(' ', '_');
                var isId = ulong.TryParse(searchTag, out ulong identity);

                if (isId)
                {
                    try
                    {
                        var r = await e926.GetPost(identity);
                        var p = r.Posts[0];
                        var artist = p.Artist[0];
                        var description = p.Description;
                        var url = p.FileUrl;
                        var footer = $"#{p.Id}";

                        e.WithTitle(artist);
                        e.WithDescription(description);
                        e.WithImageUrl(url);

                        await ReplyAsync(null, false, e.Build());
                        return;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        var errUrl = "https://i.imgur.com/WEXBzFs.gif";
                        e.WithColor(EmbedData.GetColor("error"));
                        e.WithImageUrl(errUrl);
                        await ReplyAsync(null, false, e.Build());
                        return;
                    }
                }

                var result = await e926.GetPostCollection(searchTag, limit);
                var totalCount = result.Posts.Length;
                var index = page - 1;

                var pDisp = $"{page} of {totalCount}";


                if (index > totalCount)
                {
                    index = totalCount - 1;
                    pDisp = $"{totalCount} of {totalCount}";
                }


                Console.WriteLine(result.Posts[index]);
                try
                {
                    e.WithTitle(result.Posts[index].Artist[0]);
                    e.WithDescription(result.Posts[index].Description);
                    var f = new EmbedFooterBuilder();
                    f.WithText(pDisp);
                    e.WithFooter(f);
                    e.WithImageUrl(result.Posts[index].FileUrl);
                }
                catch (IndexOutOfRangeException)
                {
                    var errUrl = "https://i.imgur.com/WEXBzFs.gif";
                    e.WithColor(EmbedData.GetColor("error"));
                    e.WithImageUrl(errUrl);
                }

                await ReplyAsync(null, false, e.Build());
            }

            [Command("random"), Alias("r")]
            [Summary("Searches on the E926 database for a specified tag.")]
            public async Task SearchRandomlyE926([Remainder]string searchTag)
            {
                var e = EmbedData.SetPostEmbed(EmbedData.EmbedCollection.Default);
                var e926 = new E926Wrapper();
                searchTag = searchTag.Replace(' ', '_');
                var result = await e926.GetPostCollection(searchTag, -1);
                var rng = new Random();
                var postCount = result.Posts.Length;
                var index = rng.Next(1, postCount) - 1;

                try
                {
                    e.WithTitle(result.Posts[index].Artist[0]);
                    e.WithDescription(result.Posts[index].Description);
                    e.WithImageUrl(result.Posts[index].FileUrl);
                }
                catch (IndexOutOfRangeException)
                {
                    var errUrl = "https://i.imgur.com/WEXBzFs.gif";
                    e.WithColor(EmbedData.GetColor("error"));
                    e.WithImageUrl(errUrl);
                }

                await ReplyAsync(null, false, e.Build());
            }

            [Command("searchlist"), Alias("sl")]
            [Summary("Searches on the E926 database for a specified tag, and returns a list of images dedicated to it.")]
            public async Task SearchForE926List(string searchTag, int page = 1)
            {
                searchTag = searchTag.ToLower();
                var e = EmbedData.SetPostEmbed(EmbedData.EmbedCollection.Default);
                var e926 = new E926Wrapper();
                searchTag = searchTag.Replace(' ', '_');
                var result = await e926.GetPostCollection(searchTag, 10);
                var rCount = result.Posts.Length;
                var pageDisplay = "";
                var limit = 1024;
                Console.WriteLine(result.Posts[0]);
                var pos = 0;

                var embeds = new List<EmbedBuilder>();
                var eCounter = embeds.Count;
                var index = page - 1;

                try
                {
                    foreach (var post in result.Posts)
                    {
                        var artist = result.Posts[pos].Artist[0].Replace("_(artist)", string.Empty);
                        var url = result.Posts[pos].FileUrl;
                        var score = result.Posts[pos].Score;
                        var id = result.Posts[pos].Id;
                        var sentence = $"        ↳ {artist} [`#{id}`]({url})";

                        var size = pageDisplay.Length + sentence.Length;
                        if (size > limit)
                        {

                            e.WithTitle($"[e926]\n    ↳ {rCount.ToString("#,#")} Result{(rCount > 1 ? "s" : "")}: {searchTag}");
                            e.WithDescription(pageDisplay);
                            embeds.Add(e);
                            e = EmbedData.SetPostEmbed(EmbedData.EmbedCollection.Default);
                            pageDisplay = "";
                        }
                        else
                        {
                            pageDisplay += $"{sentence}\n";
                        }

                        pos += 1;
                    }

                    if (pageDisplay != "")
                    {
                        e.WithTitle($"[e926]\n    ↳ {rCount.ToString("#,#")} Result{(rCount > 1 ? "s" : "")}: {searchTag}");
                        e.WithDescription(pageDisplay);
                        embeds.Add(e);
                        e = EmbedData.SetPostEmbed(EmbedData.EmbedCollection.Default);
                        pageDisplay = "";
                    }

                    eCounter = embeds.Count;
                    var c = 1;
                    foreach (var em in embeds)
                    {

                        var f = new EmbedFooterBuilder();
                        f.WithText($"Page {c} of {eCounter}");
                        em.WithFooter(f);
                        c += 1;
                    }

                }
                catch (IndexOutOfRangeException)
                {
                    var errUrl = "https://i.imgur.com/WEXBzFs.gif";
                    e.WithColor(EmbedData.GetColor("error"));
                    e.WithImageUrl(errUrl);
                }

                if (page > eCounter)
                {
                    index = eCounter - 1;
                }
                else if (page < 1)
                {
                    index = 0;
                }

                Console.WriteLine($"index({index})\nembed-count({eCounter})");
                await ReplyAsync(null, false, embeds[index].Build());
            }
        }*/
        /*
        [Group("osu"), Name("osu!")]
        [Summary("An API system for osu!.")]
        public class Osu : ModuleBase<OrikivoCommandContext>
        {
            private readonly CommandService _service;
            private readonly DiscordSocketClient _socket;
            private readonly IConfigurationRoot _config;

            public Osu(CommandService service,
                DiscordSocketClient socket,
                IConfigurationRoot config)
            {
                _service = service;
                _socket = socket;
                _config = config;
            }

            [Command("beatmap"), Alias("b")]
            [Summary("Gets a beatmap or a collection of beatmaps.")]
            public async Task GetBeatmapAsync
                (
                int index = 1,
                string since = null,
                string beatmapSetId = null,
                string beatmapId = null,
                string user = null,
                string type = null,
                string mode = null,
                string includeConvertedBeatmaps = null,
                string beatmapHash = null,
                string limit = null
                )
            {


                index = index - 1;
                if (since != null) since = since.Equals("[d]") ? null : since;
                if (beatmapSetId != null) beatmapSetId = beatmapSetId.Equals("[d]") ? null : beatmapSetId;
                if (beatmapId != null) beatmapId = beatmapId.Equals("[d]") ? null : beatmapId;
                if (type != null) type = type.Equals("[d]") ? null : type;
                if (mode != null) mode = mode.Equals("[d]") ? null : mode;
                if (includeConvertedBeatmaps != null) includeConvertedBeatmaps = includeConvertedBeatmaps.Equals("[d]") ? null : includeConvertedBeatmaps;
                if (beatmapHash != null) beatmapHash = beatmapHash.Equals("[d]") ? null : beatmapHash;
                if (limit != null) limit = limit.Equals("[d]") ? "50" : limit;
                var osu = new OsuWrapper(_config["api:osu"]);
                var param = new OsuBeatmapParameters()
                {
                    Since = since,
                    BeatmapSetId = beatmapSetId,
                    BeatmapId = beatmapId,
                    User = user,
                    Type = type,
                    Mode = mode,
                    IncludeConvertedBeatmaps = includeConvertedBeatmaps,
                    BeatmapHash = beatmapHash,
                    Limit = limit ?? "50"
                };
                var result = await osu.GetBeatmaps(param);
                var r = result.Beatmaps[index];
                var text =
                    $"```" +
                    $"\n(Beatmap {index + 1} of {param.Limit})" +
                    $"\nApproved: {r.Approved}" +
                    $"\nApprove Date: {r.ApprovedDate}" +
                    $"\nLast Updated: {r.LastUpdate}" +
                    $"\nArtist: {r.Artist}" +
                    $"\nBeatmap ID: {r.BeatmapId}" +
                    $"\nBeatmap Set ID: {r.BeatmapSetId}" +
                    $"\nBPM: {r.Bpm}" +
                    $"\nCreator: {r.Creator}" +
                    $"\nCreator ID: {r.CreatorId}" +
                    $"\nDifficulty Rating: {r.DifficultyRating}" +
                    $"\nCircle Size: {r.DiffSize}" +
                    $"\nOverall Difficulty: {r.DiffOverall}" +
                    $"\nApproach Rate: {r.DiffApproach}" +
                    $"\nHealth Drain: {r.DiffDrain}" +
                    $"\nHit Length: {r.HitLength}" +
                    $"\nSource: {r.Source}" +
                    $"\nGenre ID: {r.GenreId}" +
                    $"\nLanguage ID: {r.LanguageId}" +
                    $"\nTitle: {r.Title}" +
                    $"\nTotal Length: {r.TotalLength}" +
                    $"\nVersion: {r.Version}" +
                    $"\nMD5 Hash: {r.FileMd5}" +
                    $"\nMode: {r.Mode}" +
                    $"\nTags: {r.Tags}" +
                    $"\nFavorite Count: {r.FavoriteCount}" +
                    $"\nPlay Count: {r.PlayCount}" +
                    $"\nPass Count: {r.PassCount}" +
                    $"\nMax Combo: {r.MaxCombo}" +
                    $"```";
                await ReplyAsync(text);
            }

            [Command("user"), Alias("u")]
            [Summary("Gets a user specified.")]
            public async Task GetUserAsync
                (
                string user,
                string mode = null,
                string type = null,
                string eventDays = null
                )
            {

                if (mode != null) type = type.Equals("[d]") ? null : type;
                if (type != null) mode = mode.Equals("[d]") ? null : mode;
                if (eventDays != null) eventDays = eventDays.Equals("[d]") ? null : eventDays;

                var osu = new OsuWrapper(_config["api:osu"]);
                var param = new OsuUserParameters()
                {
                    User = user,
                    Mode = mode,
                    Type = type,
                    EventDays = eventDays
                };
                var result = await osu.GetUser(param);
                var r = result.Users[0];
                var e = r.Events.Length > 0 ? r.Events[0] : null;
                string eventText = "";

                if (e != null)
                {
                    eventText =
                        $"\n_______________" +
                        $"\nEvent I:" +
                        $"\n    HTML: [{e.DisplayHtml.Substring(0, 32)}...]" +
                        $"\n    Beatmap ID: {e.BeatmapId}" +
                        $"\n    Beatmap Set ID: {e.BeatmapSetId}" +
                        $"\n    Date: {e.Date}" +
                        $"\n    Epic Factor: {e.EpicFactor}";
                }
                var text =
                    $"```" +
                    $"\nUser ID: {r.UserId}" +
                    $"\nUsername: {r.Username}" +
                    $"\n300 Count: {r.Count300}" +
                    $"\n100 Count: {r.Count100}" +
                    $"\n50 Count: {r.Count50}" +
                    $"\nPlay Count: {r.PlayCount}" +
                    $"\nRanked Score: {r.RankedScore}" +
                    $"\nTotal Score: {r.TotalScore}" +
                    $"\nLevel: {r.Level}" +
                    $"\nPP (Ranked): {r.PpRank}" +
                    $"\nPP (Raw): {r.PpRaw}" +
                    $"\nPP (Country Rank): {r.PpCountryRank}" +
                    $"\nAccuracy: {r.Accuracy}" +
                    $"\nSS Rank Count: {r.CountRankSs}" +
                    $"\nSSH Rank Count: {r.CountRankSsH}" +
                    $"\nS Rank Count: {r.CountRankS}" +
                    $"\nSH Rank Count: {r.CountRankSH}" +
                    $"\nA Rank Count: {r.CountRankA}" +
                    $"\nCountry: {r.Country}" +
                    $"\nTotal Seconds Played: {r.TotalSecondsPlayed}" +
                    $"{eventText}" +
                    $"```";
                await ReplyAsync(text);
            }

            [Command("scores"), Alias("s")]
            [Summary("Gets the scores of a mentioned beatmap.")]
            public async Task GetScoreAsync
                (
                string beatmapId,
                int index = 1,
                string user = null,
                string type = null,
                string mode = null,
                string mods = null,
                string limit = null
                )
            {
                index = index - 1;
                if (type != null) type = type.Equals("[d]") ? null : type;
                if (mode != null) mode = mode.Equals("[d]") ? null : mode;
                if (mods != null) mods = mods.Equals("[d]") ? null : mods;
                if (limit != null) limit = limit.Equals("[d]") ? null : limit;
                var osu = new OsuWrapper(_config["api:osu"]);
                var param = new OsuScoreParameters()
                {
                    BeatmapId = beatmapId,
                    User = user,
                    Type = type,
                    Mode = mode,
                    Mods = mods,
                    Limit = limit
                };
                var result = await osu.GetScores(param);
                var r = result.Scores[index];
                var text =
                    $"```" +
                    $"\nScore ID: {r.ScoreId ?? "null"}" +
                    $"\nScore: {r.Score ?? "null"}" +
                    $"\nUsername: {r.Username ?? "null"}" +
                    $"\nMax Combo: {r.MaxCombo ?? "null"}" +
                    $"\n300 Count: {r.Count300 ?? "null"}" +
                    $"\n100 Count: {r.Count100 ?? "null"}" +
                    $"\n50 Count: {r.Count50 ?? "null"}" +
                    $"\nMiss Count: {r.CountMiss ?? "null"}" +
                    $"\nKatu Count: {r.CountKatu ?? "null"}" +
                    $"\nGeki Count: {r.CountGeki ?? "null"}" +
                    $"\nPerfect: {r.Perfect ?? "null"}" +
                    $"\nEnabled Mods: {r.EnabledMods ?? "null"}" +
                    $"\nUser ID: {r.UserId ?? "null"}" +
                    $"\nDate: {r.Date ?? "null"}" +
                    $"\nRank: {r.Rank ?? "null"}" +
                    $"\nPP: {r.Pp ?? "null"}" +
                    $"\nReplay Available: {r.ReplayAvailable ?? "null"}" +
                    $"```";
                await ReplyAsync(text);
            }

            [Command("userrecent"), Alias("ur")]
            [Summary("Gets the user's recent matches.")]
            public async Task GetUserRecentAsync
                (
                string user,
                int index = 1,
                string type = null,
                string mode = null,
                string limit = null
                )
            {
                index = index - 1;
                if (type != null) type = type.Equals("[d]") ? null : type;
                if (mode != null) mode = mode.Equals("[d]") ? null : mode;
                if (limit != null) limit = limit.Equals("[d]") ? null : limit;
                var osu = new OsuWrapper(_config["api:osu"]);
                var param = new OsuUserRecentParameters()
                {
                    User = user,
                    Type = type,
                    Mode = mode,
                    Limit = limit
                };
                var result = await osu.GetUserRecentBeatmaps(param);
                var r = result.UserRecentBeatmaps[index];
                var text =
                    $"```" +
                    $"\nBeatmap ID: {r.BeatmapId}" +
                    $"\nScore: {r.Score}" +
                    $"\nMax Combo: {r.MaxCombo}" +
                    $"\n300 Count: {r.Count300}" +
                    $"\n100 Count: {r.Count100}" +
                    $"\n50 Count: {r.Count50}" +
                    $"\nMiss Count: {r.CountMiss}" +
                    $"\nKatu Count: {r.CountKatu}" +
                    $"\nGeki Count: {r.CountGeki}" +
                    $"\nPerfect: {r.Perfect}" +
                    $"\nEnabled Mods: {r.EnabledMods}" +
                    $"\nUser ID: {r.UserId}" +
                    $"\nDate: {r.Date}" +
                    $"\nRank: {r.Rank}" +
                    $"```";
                await ReplyAsync(text);
            }

            [Command("userbest"), Alias("ub")]
            [Summary("Gets the user's top scores.")]
            public async Task GetUserBestAsync
                (
                string user,
                int index = 1,
                string type = null,
                string mode = null,
                string limit = null
                )
            {
                index = index - 1;
                if (type != null) type = type.Equals("[d]") ? null : type;
                if (mode != null) mode = mode.Equals("[d]") ? null : mode;
                if (limit != null) limit = limit.Equals("[d]") ? null : limit;
                var osu = new OsuWrapper(_config["api:osu"]);
                var param = new OsuUserBestParameters()
                {
                    User = user,
                    Type = type,
                    Mode = mode,
                    Limit = limit
                };
                var result = await osu.GetUserBestScores(param);
                var r = result.UserBestScores[index];
                var text =
                    $"```" +
                    $"\nBeatmap ID: {r.BeatmapId}" +
                    $"\nScore: {r.Score}" +
                    $"\nMax Combo: {r.MaxCombo}" +
                    $"\n300 Count: {r.Count300}" +
                    $"\n100 Count: {r.Count100}" +
                    $"\n50 Count: {r.Count50}" +
                    $"\nMiss Count: {r.CountMiss}" +
                    $"\nKatu Count: {r.CountKatu}" +
                    $"\nGeki Count: {r.CountGeki}" +
                    $"\nPerfect: {r.Perfect}" +
                    $"\nEnabled Mods: {r.EnabledMods}" +
                    $"\nUser ID: {r.UserId}" +
                    $"\nDate: {r.Date}" +
                    $"\nRank: {r.Rank}" +
                    $"\nPP: {r.Pp}" +
                    $"```";
                await ReplyAsync(text);
            }

            [Command("match"), Alias("m")]
            [Summary("Gets information about a current match.")]
            public async Task GetMatchAsync
                (
                string matchId
                )
            {
                var osu = new OsuWrapper(_config["api:osu"]);
                var param = new OsuMatchParameters()
                {
                    MatchId = matchId
                };
                var result = await osu.GetMatch(param);
                var r = result.Matches[0];
                var m = r.Match;
                var g = r.Games[0];
                var s = g.Scores[0];
                var text =
                    $"```" +
                    $"\nMatch ID: {m.MatchId}" +
                    $"\nName: {m.Name}" +
                    $"\nStart Time: {m.StartTime}" +
                    $"\nEnd Time: {m.EndTime}" +
                    $"\n_______________" +
                    $"\nGame I:" +
                    $"\n    Game ID: {g.GameId}" +
                    $"\n    Start Time: {g.StartTime}" +
                    $"\n    End Time: {g.EndTime}" +
                    $"\n    Beatmap ID: {g.BeatmapId}" +
                    $"\n    Play Mode: {g.PlayMode}" +
                    $"\n    Match Type: {g.MatchType}" +
                    $"\n    Scoring Type: {g.ScoringType}" +
                    $"\n    Team Type: {g.TeamType}" +
                    $"\n    Mods: {g.Mods}" +
                    $"\n    _______________" +
                    $"\n    Score Slot I:" +
                    $"\n        Slot: {s.Slot}" +
                    $"\n        Team: {s.Team}" +
                    $"\n        User ID: {s.UserId}" +
                    $"\n        Score: {s.Score}" +
                    $"\n        Max Combo: {s.MaxCombo}" +
                    $"\n        Rank: {s.Rank}" +
                    $"\n        50 Count: {s.Count50}" +
                    $"\n        100 Count: {s.Count100}" +
                    $"\n        300 Count: {s.Count300}" +
                    $"\n        Miss Count: {s.CountMiss}" +
                    $"\n        Geki Count: {s.CountGeki}" +
                    $"\n        Katu Count: {s.CountKatu}" +
                    $"\n        Perfect: {s.Perfect}" +
                    $"\n        Pass: {s.Pass}" +
                    $"```";
                await ReplyAsync(text);
            }

            [RequireOwner]
            [Command("replay"), Alias("r")]
            [Summary("Gets a replay session, encoded in Base64.")]
            public async Task GetReplayAsync
                (
                string mode,
                string beatmapId,
                string user
                )
            {
                var osu = new OsuWrapper(_config["api:osu"]);
                var param = new OsuReplayParameters()
                {
                    Mode = mode,
                    BeatmapId = beatmapId,
                    User = user
                };
                var result = await osu.GetReplay(param);
                var r = result.Replay;
                var text =
                    $"```" +
                    $"\nContent: {r.Content.Substring(0, 32)}..." +
                    $"\nEncoding: {r.Content}" +
                    $"```";
                await ReplyAsync(text);
            }


        }*/
        /*
        [Group("tenor"), Name("Tenor"), Alias("t")]
        [Summary("A group that contains a system for Tenor.")]
        public class Tenor : ModuleBase<OrikivoCommandContext>
        {
            private readonly CommandService _service;
            private readonly DiscordSocketClient _socket;
            private readonly IConfigurationRoot _config;

            public Tenor(CommandService service,
                DiscordSocketClient socket,
                IConfigurationRoot config)
            {
                _service = service;
                _socket = socket;
                _config = config;
            }

            [Command("random"), Alias("r")]
            [Summary("A command that searches for a gif on Tenor with the provided tag.")]
            public async Task GrabTenorImage([Remainder]string tenorTag)
            {
                var embedGif = new EmbedBuilder();
                var tenor = new TenorWrapper(_config["api:tenor"]);
                var defaultSafeSearch = TenorRating.Disabled;
                try
                {

                }
                catch () { }
                var randomReturn = await tenor.GetGifRandomly(new TenorRandom()
                {
                    Q = tenorTag,
                    SafeSearch = defaultSafeSearch
                });
                Console.WriteLine(randomReturn.Results.ToString());

                embedGif.WithColor(EmbedData.GetColor("correct"));
                try
                {
                    embedGif.WithImageUrl(randomReturn.Results[0].Media[0].Gif.Url);
                }
                catch (IndexOutOfRangeException)
                {
                    var finUrl = "https://i.imgur.com/WEXBzFs.gif";
                    var rnd = new Random();
                    if (rnd.Next(0, 1) == 1)
                    {
                        finUrl = "http://gifimage.net/wp-content/uploads/2017/11/error-gif-13.gif";
                    }
                    embedGif.WithColor(EmbedData.GetColor("error"));
                    embedGif.WithImageUrl(finUrl);
                }

                await ReplyAsync("", false, embedGif.Build());
            }
        }*/
    }
}