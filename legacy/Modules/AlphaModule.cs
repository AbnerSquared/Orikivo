using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orikivo.Systems.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orikivo.Static;
using Orikivo.Utility;
using Discord;
using Orikivo.Systems.Services;
using System.IO;
using System.Drawing;
using Color = Discord.Color;
using Orikivo.Providers;
using System.Text;
using Orikivo.Systems.Dependencies;
using System.Drawing.Imaging;
using SysImageFormat = System.Drawing.Imaging.ImageFormat;
using ImageFormat = Discord.ImageFormat;
using Orikivo.Wrappers;
using Orikivo.Tests;

namespace Orikivo.Modules
{
    [Name("Alpha")]
    [Summary("Contains a collection of incomplete commands.")]
    [DontAutoLoad]
    public class AlphaModule : ModuleBase<OrikivoCommandContext>
    {
        // import single-launch services here.
        private readonly CommandService _service;
        private readonly DiscordSocketClient _client;
        private readonly IConfigurationRoot _config;
        private Random _rng;

        public AlphaModule
        (
            CommandService service,
            DiscordSocketClient client,
            IConfigurationRoot config,
            Random rng
        )
        {
            _service = service;
            _client = client;
            _config = config;
            _rng = rng;
        }

        //[Command("solvetest")]
        public async Task SolveDemoAsync()
        {
            CalculatorTest.Solve();
            await ReplyAsync("Demo complete. Refer to console.");
        }

        //[Command("hashtest")]
        public async Task HashDemoAsync()
        {
            HashBuilderTest.GetHash();
            await ReplyAsync("Demo complete. Refer to console.");
        }

        //[Command("keytest")]
        public async Task KeyDemoAsync(int iterations = 100, int size = KeyBuilder.DefaultKeyLength)
        {
            iterations = iterations.InRange(1, int.MaxValue);
            size = size.InRange(1, KeyBuilder.DefaultKeyLength);
            KeyBuilderTest.Regenerate(iterations, size);
            await ReplyAsync("Demo complete. Refer to console.");
        }

        [Command("time")]
        [Summary("Read the current time derived from the UTC standard on Orikivo.")]
        public async Task ReadTimeAsync()
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            eb.WithTitle(DateTime.Now.ToFullOriTime());
            await Context.Channel.SendEmbedAsync(eb.Build());
        }

        // Complete.
        [Command("compose")]
        [Summary("Construct a preset letter, and relay the data back as a message.")]
        public async Task DemoLetterAsync(
            [Name("url"), Summary("The path of the attachment you wish to send. A file attachment automatically overrides a URL. Incorrect formatting is ignored.")]string url,
            [Name("subject"), Summary("The main topic of the letter.")]string subject,
            [Name("message"), Summary("The description of the summary specified. Is not required.")][Remainder]string message = null)
            => await ModuleManager.TryExecute(Context.Channel, AlphaService.DemoLetterAsync(Context, subject, message, url));


        // add vote streak
        [Command("vote"), Alias("v")]
        [Summary("Learn about or verify votes.")]
        public async Task VoteAsync()
        {
            LockedDblWrapper dbl = new LockedDblWrapper(Context.Client.CurrentUser.Id, _config["api:dbl"]);
            bool voted = dbl.HasVotedAsync(Context.User.Id).Result;
            EmbedBuilder e = EmbedData.DefaultEmbed;
            if (voted)
            {
                e.WithTitle("Thanks for voting!");
                e.WithDescription("Your hard work has been noted.");
            }
            else
            {
                e.WithTitle("It looks like you haven't voted yet!");
                StringBuilder str = new StringBuilder();
                str.AppendLine("As of now, voting doesn't bring perks aside from a genuine smile.");
                str.AppendLine("But that doesn't mean it won't help in the long run!".MarkdownBold());
                str.Append("When the time is right, voting will provide fun bonuses and merits for all who participate. ");
                str.AppendLine($"It also helps us out. In the end, it's your call :)");
                str.Append($"You can vote [**here!**]({OldGlobal.VotingUrl})");
                e.WithDescription(str.ToString());
            }

            await ReplyAsync(embed: e.Build());
        }

        // enhance react features
        [Command("react"), Alias("rct")]
        [Summary("Allows the user to react with Orikivo Mini. | Reactions: owo")]
        public async Task React([Remainder]string reaction)
        {
            var embedReaction = new EmbedBuilder
            {
                Color = EmbedData.GetColor(reaction),
                ImageUrl = EmbedData.GetReaction(reaction)
            };
            await ReplyAsync("", false, embedReaction.Build());
        }
        /*
        [Command("notifications")]
        [Summary("View all notifications.")]
        public async Task NotificationResponseAsync(int notifier = 1)
        {
            await ModuleManager.TryExecute(Context.Channel, NotificationAsync(notifier));
        }

        public async Task NotificationAsync(int notifier = 1)
        {
            AccountConfig cfg = Context.Account.Config;
            string m1 = "(1) **Inbound Mail** - " + cfg.InboundMail.ToToggleString();
            string m2 = $"(2) **Updates** - {cfg.InboundUpdates.ToToggleString()}";
            await ReplyAsync(m1);
        }

        [Command("notify")]
        [Summary("Toggle a notifier using its id.")]
        public async Task ToggleNotifierAsync(int id)
        {
            await ModuleManager.TryExecute(Context.Channel, SetNotifierAsync(id));
        }

        public async Task SetNotifierAsync(int id)
        {
            AccountConfig cfg = Context.Account.Config;
            if (id == 1)
            {
                cfg.ToggleMail();
                if (cfg.InboundMail)
                {
                    await ReplyAsync("Notifications will now be sent to your DMs upon getting new mail.");
                }
                else
                {
                    await ReplyAsync("You will no longer have notifications when a mail is sent to you.");
                }
                return;
            }
            if (id == 2)
            {
                cfg.ToggleUpdates();
                if (cfg.InboundUpdates)
                {
                    await ReplyAsync("Notifications will now be sent when Orikivo is updated.");
                }
                else
                {
                    await ReplyAsync("You will no longer have notifications when Orikivo is updated.");
                }
                return;
            }
            await ThrowNotifierErrorAsync(id);
        }

        public async Task ThrowNotifierErrorAsync(int id)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("error"));
            e.Title = $"There is no notifier option under {id}.";
            await ReplyAsync(embed: e.Build());
        }

        public async Task TossMailsAsync(int[] ids)
        {
            int truecount = ids.Length;

            foreach (int id in ids)
            {
                if (!Context.Account.Mailbox.TryGetMail(id, out OldMail m))
                {
                    truecount -= 1;
                    continue;
                }
                if (m.Locked)
                {
                    truecount -= 1;
                    continue;
                }
            }

            if (!(truecount > 0))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "All of the letters that were specified were invalid."));
                return;
            }

            Context.Account.Mailbox.Toss(ids);
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithColor(EmbedData.GetColor("error"));
            e.WithDescription($"You have tossed {truecount} letter{(truecount > 1 ? "s" : "")}.");
            await ReplyAsync(embed: e.Build());
        }

        public async Task TossMailAsync(OldMail m)
        {
            Context.Account.Mailbox.Toss(m);
        }

        [Command("toss"), Priority(1)]
        public async Task TossMailResponseAsync(int id)
        {
            if (!Context.Account.Mailbox.TryGetMail(id, out OldMail m))
            {
                await ThrowMailErrorAsync(id);
                return;
            }
            if (m.Locked)
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "This mail is locked, and cannot be tossed."));
                return;
            }

            await TossMailAsync(m);

            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithColor(EmbedData.GetColor("error"));
            e.WithDescription($"You have tossed mail-{id}.");
            await ReplyAsync(embed: e.Build());
        }

        [Command("toss"), Priority(0)]
        public async Task TossMailResponseAsync(params int[] ids)
        {
            await TossMailsAsync(ids);
        }

        [Command("send")]
        [Summary("Compose a mail to send to somebody's inbox.")]
        public async Task SendMailAsync(SocketUser user, string subject, [Remainder]string content = null)
        {
            const int MAX_SUBJECT = 32;
            const int MAX_CONTENT = EmbedBuilder.MaxDescriptionLength;
            if (subject.Length > MAX_SUBJECT)
            {
                await Context.Channel.ThrowAsync("Max subject limit reached.",
                    $"Ensure that the subject you write is under {MAX_SUBJECT} characters.");
                return;
            }
            if (content.Exists())
            {
                if (content.Length > MAX_CONTENT)
                {
                    await Context.Channel.ThrowAsync("Max content limit reached.",
                       $"Ensure that the content you write is under {MAX_CONTENT} characters.");
                    return;
                }
            }

            OldAccount a = Context.Data.GetOrAddAccount(user);
            OldMail m = new OldMail(Context.Account, subject, content.Exists() ? new CompactMessage(content) : null);
            await m.SendAsync(a, Context.Client);
            Context.Data.Update(a);
            await ReplyAsync($"Your mail has been sent to {a.GetName().MarkdownBold()}.");
        }

        [Command("read")]
        [Summary("Reads a mail from an index of the mailbox.")]
        public async Task GetMailAsync(int id)
        {
            if (!Context.Account.Mailbox.TryGetMail(id, out OldMail m))
            {
                await ThrowMailErrorAsync(id);
                return;
            }

            await ReplyAsync(embed: ReadMail(m).Build());

            if (m.Message.Embeds.Exists())
                if (m.Message.Embeds.Count > 0)
                    await ReplyAsync(embed: m.Message.Embeds[0]);
        }

        public async Task ThrowMailErrorAsync(int id)
        {
            await ReplyAsync(embed: EmbedData.Throw(Context, $"`mail-{id}` does not exist in your mailbox."));
        }

        public Embed GetMailLockState(OldMail m, int id)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithColor(m.Locked ? EmbedData.GetColor("error") : EmbedData.GetColor("yield"));
            e.WithDescription($"`mail-{id}` has been {(m.Locked ? "locked.\nIt will no longer be erased." : "unlocked.\nIt can now be erased.")}");
            return e.Build();
        }

        public async Task LockMailAsync(OldMail m, int id)
        {
            m.ToggleLock();
            await ReplyAsync(embed: GetMailLockState(m, id));
            Context.Data.Update(Context.Account);
        }

        [Command("lock")]
        [Summary("prevents a mail from being deleted.")]
        public async Task LockMailAsync(int id)
        {
            if (!Context.Account.Mailbox.TryGetMail(id, out OldMail m))
            {
                await ThrowMailErrorAsync(id);
                return;
            }

            await LockMailAsync(m, id);
        }

        public async Task EmptyMailAsync()
        {
            if (!Context.Account.Mailbox.HasMail())
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Your mailbox is already empty."));
                return;
            }

            Context.Account.Mailbox.Empty();
            await ReplyAsync(embed: OnEmptyMail());
        }

        public Embed OnEmptyMail()
        {
            return EmbedData.DefaultEmbed.WithDescription($"{EmojiIndex.EmptyMailbox} You have emptied your mailbox.").Build();
        }

        [Command("clearmail")]
        [Summary("Clears all of your mail in the mailbox.")]
        public async Task ClearMailAsync()
        {
            await EmptyMailAsync();
        }

        [Command("mail")]
        [Summary("View all of your current mail.")]
        public async Task CheckMailAsync(int page = 1)
        {
            if (!Context.Account.Mailbox.HasMail())
            {
                await ReplyAsync(embed: EmptyMail());
                return;
            }

            Embed mail = GetMailList(Context.Account.Mailbox, page);
            await ReplyAsync(embed: mail);
        }

        public Embed EmptyMail(EmbedBuilder e = null)
        {
            e = e ?? EmbedData.DefaultEmbed;
            e.WithDescription($"{EmojiIndex.EmptyMailbox} You have no mail.");
            return e.Build();
        }

        public EmbedBuilder ReadMail(OldMail m)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(m.Read ? EmbedData.GetColor("origreen") : EmbedData.GetColor("owo"));
            e.WithTitle(GetMailTitle(m));
            if (m.Message.Exists())
                e.WithDescription(m.Message.Content);
            e.WithTimestamp(m.Date);
            m.MarkAsRead();
            return e;
        }

        public string GetMailTitle(OldMail m)
            => $"{m.LockString(Context.Account, " ")}{m.Author.Name.MarkdownBold()}\nSubject: {m.Subject}";

        public Embed GetMailList(AccountMailbox mailbox, int page = 1)
        {
            EmbedBuilder b = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText($"{EmojiIndex.Mail} Inbox");
            b.WithFooter(f);

            List<string> strings = new List<string>();

            foreach (OldMail m in mailbox.GetAllMail())
            {
                strings.Add(GetLinearInfo(m, mailbox.Mail.IndexOf(m)));
            }

            return EmbedData.GenerateEmbedList(strings, page, b);
        }

        public string GetLinearInfo(OldMail m, int index)
            => $"({(index + 1)}) {m.ReadString(Context.Account)}{m.Author.Name.MarkdownBold()} - {m.LockString(Context.Account, " ")}{m.Subject}";
            */
        [Command("captcha")]
        [Summary("Generate a new captcha. (Rendering Demo)")]
        public async Task BuildVerifier()
        {
            CaptchaBuilder v = new CaptchaBuilder();
            string url = ".//data//captcha.png";
            v.Captcha.SaveBitmap(url, SysImageFormat.Png);

            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithImageUrl($"attachment://{Path.GetFileName(url)}");
            e.WithColor(EmbedData.GetColor("error"));

            await Context.Channel.SendFileAsync(url);
            //_client.MessageReceived += OnVerificationDetection;
        }

        /*
        public async Task OnVerificationDetection(SocketMessage message)
        {
            if (message.Author.Exists())
            {
                // if the author matches the captcha caller author
                // if what they typed matches the captcha key
                // give this user the default role.
            }
        }
        */

        [RequireOwner]
        [Command("activity"), Priority(2)]
        [Summary("Sets the type of activity alongside the activity name itself.")]
        public async Task ChangeActivityNameAsync(ActivityType type, [Remainder]string name)
        {
            var e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");
            name = name ?? "literally nothing.";
            Context.Data.Global.SetActivity(name, type);
            await _client.UpdateActivity(Context.Data.Global.Activity);
            await ReplyAsync(embed: $"The activity name is now `{name}`".ToEmbedDescription(e).Build());
        }

        [RequireOwner]
        [Command("activity"), Priority(1)]
        [Summary("Places a new activity name the value written.")]
        public async Task ChangeActivityNameAsync([Remainder]string name)
        {
            var e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");
            name = name ?? "literally nothing.";
            Context.Data.Global.SetActivity(name);
            await _client.UpdateActivity(Context.Data.Global.Activity);
            await ReplyAsync(embed: $"The activity name is now `{name}`".ToEmbedDescription(e).Build());
        }

        [Command("activity"), Priority(0)]
        [Summary("Returns the activity format Orikivo is using.")]
        public async Task ViewGlobalDataAsync()
        {
            var e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("owo");
            await ReplyAsync(embed: $"{Context.Data.Global.Activity.Type.ToTypeString().MarkdownBold() + " " + Context.Data.Global.Activity.Name}".ToEmbedDescription(e).Build());
        }

        [Command("shop")]
        [Summary("Take a look at the shop, and see what you like! (Provided you can buy anything)")]
        public async Task ViewShopAsync()
        {
            StoreManager tmp = new StoreManager();
            List<string> items = new List<string>();
            items.Add("**Schemes**");
            foreach (OldCardColorScheme scheme in tmp.ColorSchemes)
            {
                items.Add($"{scheme.Name} | {EmojiIndex.Balance}{scheme.Cost.ToPlaceValue().MarkdownBold()}");
            }
            items.Add("**Consumables**");
            foreach (ActionItem item in tmp.Consumables)
            {
                items.Add($"{item.Name} | {EmojiIndex.Balance}{item.Cost.ToPlaceValue().MarkdownBold()}");
            }

            var e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = "Ori's Corner Store";
            e.Footer = new EmbedFooterBuilder().WithText($"{Context.Server.Config.GetPrefix(Context)}inspect <name>");
            await ReplyAsync(embed: string.Join('\n', items).ToEmbedDescription(e).Build());
        }

        [Command("inspect"), Alias("view")]
        [Summary("Look at an item in the shop.")]
        public async Task InspectItemAsync([Remainder]string name)
        {
            try
            {
                StoreManager tmp = new StoreManager();
                ActionItem item = tmp.Consumables.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
                if (item == null)
                {
                    Debugger.Write("yeah, this is fine.");
                    await ReplyAsync(embed: EmbedData.Throw(Context, "The item you specified may not exist or have a description."));
                    return;
                }
                var f = new EmbedFooterBuilder();

                f.WithText($"{(Context.Server.Config.UsePrefixes ? Context.Server.Config.Prefix : Context.Client.CurrentUser.Mention)}buy {item.Name.ToLower()}");


                var e = new EmbedBuilder();
                e.WithColor(EmbedData.GetColor("origreen"));
                e.WithTitle($"{item.Name} {EmojiIndex.Balance}{item.Cost.ToPlaceValue().MarkdownBold()}");
                e.WithDescription(item.Description);
                e.WithFooter(f);

                await ReplyAsync(embed: e.Build());
            }
            catch (ArgumentNullException)
            {
                await ReplyAsync("That item either doesn't exist, or may not have a description.");
            }
        }

        [Command("buy")]
        [Summary("Buy an item.")]
        public async Task BuyItemAsync([Remainder]string name)
        {
            var e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");

            StoreManager tmp = new StoreManager();
            OldCardColorScheme purchase2 = null;
            tmp.BuyConsumable(Context.Account, name, out ActionItem purchase);
            if (purchase == null)
            {
                tmp.BuyScheme(Context.Account, name, out purchase2);
            }
            string s = "";
            if (purchase == null && purchase2 == null)
            {
                e.WithColor(EmbedData.GetColor("error"));
                s = "You lack funds."; // be able to grab the cost of what you wanted.
            }
            else
            {
                if (purchase == null)
                {
                    s = $"Bought `{purchase2.Name}` for {EmojiIndex.Balance}{purchase2.Cost.ToPlaceValue().MarkdownBold()}.";
                }

                s = $"Bought `{purchase.Name}` for {EmojiIndex.Balance}{purchase.Cost.ToPlaceValue().MarkdownBold()}.";
            }
            await ReplyAsync(embed: s.ToEmbedTitle(e).Build());
        }
        /*
        [Command("scheme")]
        [Summary("Set a color scheme for card rendering. (default is origreen)")]
        public async Task SetSchemeAsync(string name = null)
        {
            var a = Context.Account;
            var def = new SchemeIndex().OriGreen;
            if (name == null)
            {
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithColor(a.Card.Schema.Palette[0].R, a.Card.Schema.Palette[0].G, a.Card.Schema.Palette[0].B);
                e.WithTitle($"You are currently using **{a.Card.Schema.Name}**.");
                await ReplyAsync(embed: e.Build());
                return;
            }
            if (name.ToLower() == def.Name.ToLower())
            {
                a.Card.ClearSchema();
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithColor(def.Palette[0].R, def.Palette[0].G, def.Palette[0].B);
                e.WithTitle("Your card will now render with **OriGreen**.");
                await ReplyAsync(embed: e.Build());
                Context.Data.Update(Context.Account);
                return;
            }
            else
            {
                name = name.ToLower();
                StoreManager tmp = new StoreManager();
                OldCardColorScheme item = tmp.ColorSchemes.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                item.Name.Debug();
                foreach (OldCardColorScheme itm in a.Inventory.Items)
                {
                    itm.Name.Debug();
                }
                if (!item.Exists())
                {
                    await ReplyAsync("This scheme doesn't exist at all. What are you trying to get at?");
                    return;
                }

                if (a.Inventory.HasItem(item))
                {
                    a.Card.SetSchema(item);
                    EmbedBuilder e = EmbedData.DefaultEmbed;
                    e.WithColor(a.Card.Schema.Palette[0].R, a.Card.Schema.Palette[0].G, a.Card.Schema.Palette[0].B);
                    e.WithTitle($"Your card will now render with **{a.Card.Schema.Name}**.");
                    await ReplyAsync(embed: e.Build());
                    return;
                }
                else
                {
                    var balance = a.Balance;
                    var cost = item.Cost;
                    EmbedBuilder e = EmbedData.DefaultEmbed;
                    e.WithColor(balance >= cost ? EmbedData.GetColor("origreen") : EmbedData.GetColor("error"));
                    string extra = (balance >= cost) ? $"However, you have enough. Why not buy it?" :
                        $"You require {EmojiIndex.Balance}{(cost - balance).ToPlaceValue().MarkdownBold()}.";
                    e.WithDescription($"You do not have **{item.Name}**.\n{extra}");
                    await ReplyAsync();
                }
            }
        }

        [Command("buyscheme")]
        [Summary("Try to buy a scheme.")]
        public async Task BuySchemeAsync([Remainder]string name)
        {
            var e = EmbedData.DefaultEmbed;

            StoreManager tmp = new StoreManager();
            tmp.BuyScheme(Context.Account, name, out OldCardColorScheme purchase);
            string s = "";
            if (purchase == null)
            {
                e.WithColor(EmbedData.GetColor("error"));
                s = "You lack funds."; // be able to grab the cost of what you wanted.
            }
            else
            {
                s = $"Bought **{purchase.Name}** for {EmojiIndex.Balance}{purchase.Cost.ToPlaceValue().MarkdownBold()}.";
            }
            await ReplyAsync(embed: s.ToEmbedTitle(e).Build());
            Context.Data.Update(Context.Account);
        }

        [Command("use")]
        [Summary("Use an available consumable.")]
        public async Task UseItemAsync(int id)
        {
            List<ActionItem> items = Context.Account.Inventory.Consumables;
            ActionItem item;

            try
            {
                item = items.ElementAt(id - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Invalid index.", "The location you specified for an item doesn't exist.", false));
                return;
            }

            Context.Account.Use(item, Context);
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithDescription($"You have used **{item.Name}**.");
            await ReplyAsync(embed: e.Build());
        }

        [Command("inventory"), Alias("inv")]
        [Summary("View your current item storage.")]
        public async Task GetInventoryAsync([Remainder]SocketUser user = null)
        {
            var e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("yield");

            user = user ?? Context.User;
            OldAccount a = Context.Data.GetOrAddAccount(user);
            List<string> items = new List<string>();
            List<OldCardColorScheme> tmp = a.Inventory.Items;
            List<ActionItem> tmp2 = a.Inventory.Consumables;

            foreach (OldCardColorScheme item in tmp)
            {
                double value = (item.Cost / 100) * 70; // items are worth 70% atm.
                items.Add($"[{(tmp.IndexOf(item) + 1).ToPlaceValue().MarkdownBold()}] {item.Name} | {EmojiIndex.Balance}{value.ToPlaceValue().MarkdownBold()}");
            }
            foreach (ActionItem item in tmp2)
            {
                double value = (item.Cost / 100) * 70;
                items.Add($"[{(tmp2.IndexOf(item) + 1).ToPlaceValue().MarkdownBold()}] {item.Name} | {EmojiIndex.Balance}{value.ToPlaceValue().MarkdownBold()}");
            }
            string r = items.Count > 0 ? string.Join('\n', items) : "You don't have any items.".DiscordBlock();
            await ReplyAsync(embed: r.ToEmbedDescription(e).Build());
        }
        */
        [Command("flags")]
        [Summary("View reporting flag types, with each one representing a specific priority level.")]
        public async Task ViewFlagTypeAsync()
        {
            var e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("steamerror"));
            e.WithTitle("Report Guide - Priority Flags");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.PriorityFlag} | **P0 (Fundamental)**");
            sb.AppendLine($"These are for situations in which the function executed can be exploited to a point of considerably damaging Orikivo's data.");

            sb.AppendLine($"\n{EmojiIndex.ExceptionFlag} | **P1 (Runtime)**");
            sb.AppendLine($"In the case that a command fails to even work, it may most likely be due to an exception. In most cases, you should see an exception box, from which you can directly send as a report.");

            sb.AppendLine($"\n{EmojiIndex.SpeedFlag} | **P2 (Blocking)**");
            sb.AppendLine($"These are for situations in which a command you execute takes much longer than the average execution speed. They tend to block gateways, and harm the process speed of Orikivo.");

            sb.AppendLine($"\n{EmojiIndex.VisualFlag} | **P3 (Visual)**");
            sb.AppendLine($"This falls under grammatical mishaps, visual bugs, and just plain incorrect information.");

            sb.AppendLine($"\n{EmojiIndex.SuggestFlag} | **P4 (Improvement)**");
            sb.AppendLine($"Use this flag if you wish to add input or suggestions to an existing command to simplify and speed up execution.");

            e.WithDescription(sb.ToString());

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText("Tip: You may also type the priority number when reporting, rather than the emoji variant.");

            await ReplyAsync(embed: e.Build());
        }

        [Command("server"), Priority(0)]
        [Summary("View basic server information about the current guild.")]
        public async Task ViewServerAsync()
        {
            await ReplyAsync(embed: GenerateServerBox(Context.Server));
        }

        [Command("server"), Priority(1)]
        [Summary("View basic server information about another guild.")]
        public async Task ViewServerAsync([Remainder]string guild)
        {
            if (!Context.TryGetGuild(guild, out SocketGuild g))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "The guild you specified does not exist."));
                return;
            }

            Server tmp = Context.Data.GetOrAddServer(g);
            await ReplyAsync(embed: GenerateServerBox(tmp));
        }

        // try uploading a font, and automatically crop each letter as a demo!
        /*
        [Command("font")]
        [Summary("View information about a font face.")]
        public async Task ViewFont()
        {
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            ulong f = Context.Account.Card.FontId;
            FontFace font = FontManager.FontMap.GetFont(f);

            if (!font.Exists())
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Invalid font identifier.", "Your current font was invalidated and has been reset.", false));
                Context.Account.Card.FontId = 0;
                Context.Data.Update(Context.Account);
                return;
            }

            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(font.Name);

            //e.AddField("Overhang", "", false);
            //e.AddField("Padding", "", false);
            //e.AddField("Styles", "", false);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.Identifier.Pack(a)} {font.Id}");
            sb.AppendLine($"{EmojiIndex.Size.Pack(a)} {font.Ppu.ToString()}");

            sb.AppendLine($"**Overhang** | {font.Overhang}px");
            //sb.AppendLine($"{EmojiIndex.Permission.Pack()}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} Overhang refers to the vertical downward pixel push height for lowercase hanging letters. (y, g, q)");

            sb.AppendLine($"**Padding** | {font.Padding}px");
            //sb.AppendLine($"{EmojiIndex.Permission.Pack()} {font.Padding}px");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} Padding is term for the pixel spacing after each letter has been placed in a generation.");

            sb.AppendLine($"**Styles** | {GetFontStyles(font)}");
            //sb.AppendLine($"{EmojiIndex.Permission.Pack()} {GetFontStyles(font)}");
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} Styles are the overrides that a font face has. This includes bold, italics, and anything that fits into such criteria.");

            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());

        }

        [Command("font")]
        [Summary("Update your font face to a new one using its id.")]
        public async Task SetFont(ulong id)
        {
            if (!FontManager.FontMap.Fonts.Any(x => x.Id == id))
            {
                await ReplyAsync("no fonts have that id.");
                return;
            }

            FontFace font = FontManager.FontMap.GetFont(id);

            Context.Account.Card.FontId = id;
            await ReplyAsync($"Font set to {font.Name}.");
            Context.Data.Update(Context.Account);
        }

        [Command("fonts")]
        public async Task ViewFonts(int page = 1)
        {
            List<FontFace> fonts = FontManager.FontMap.Fonts.OrderBy(x => x.Id).ToList();
            List<string> info = new List<string>();
            foreach (FontFace font in fonts)
            {
                info.Add(GetFontSummary(font));
            }
            EmbedBuilder b = EmbedData.DefaultEmbed;
            b.WithTitle($"**Orikivo** - Fonts");
            Embed e = EmbedData.GenerateEmbedList(info, page);
            await ReplyAsync(embed: e);
        }

        [Command("pixelate"), Alias("pxl")]
        [Summary("Renders a string of text using a scalable Bitmap.")]
        public async Task TestRenderAsync([Remainder] string content)
        {
            int padding = 2; // 2px
            content = content.Sanitize();
            if (string.IsNullOrWhiteSpace(content))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Invalid input string.", "Make sure that you at type at least one validated character. (\\ is an invalid character)"));
                return;
            }

            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            Bitmap bmp = RenderText(content, a, padding);

            string file = $"{a.Id}_render.png";
            string path = $"{GetAccountDirectory(a)}{file}";

            SaveRender(bmp, path);

            await SendRenderedString(a, Context.Channel, bmp, path);
        }
        */
        [Command("merits")]
        [Summary("See all merits available.")]
        public async Task MeritListAsync(int page = 1)
        {
            List<string> merits = new List<string>();
            // sort by group
            CacheIndex.Merits.Merits.ForEach(x => merits.Add($"{Format.Bold(x.Name)}#{x.IdValue}"));
            Embed e = Embedder.Paginate(merits, page);
            await Context.Channel.SendEmbedAsync(e);
        }
        /*
        [Command("merit")]
        [Summary("Merit Test")]
        public async Task MeritAsync(string id = "")
        {
            Merit merit = CacheIndex.Merits.Merits.Where(x => x.IdValue == id).FirstOrDefault();
            System.Drawing.Color color = Context.Account.Card.Schema.Palette[0];
            EmbedBuilder eb = Embedder.DefaultEmbed.WithColor(color.ToDiscordColor());
            eb.WithTitle(Format.Bold(merit.Name) + $"\n#{merit.IdValue}");//$" [{Format.Code($"{merit.GroupId.ToString("00")}")} {Format.Code($"{merit.Id.ToString("000")}")} {Format.Code($"{merit.RankId.ToString("00")}")}]");
            if (!string.IsNullOrWhiteSpace(merit.IconUrl))
                eb.WithLocalImageUrl(merit.IconUrl);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(merit.Description);

            if (merit.Criteria.Funct())
            {
                sb.AppendLine($"\n**Objectives**:");
                if (merit.Classified)
                    sb.AppendLine("`[CLASSIFIED]`");
                else
                {
                    foreach (MeritCriterion criterion in merit.Criteria)
                    {
                        string goal = "";
                        string value = "";
                        Emoji em = EmojiIndex.Balance;
                        switch (criterion.Goal)
                        {
                            case MeritGoalType.Own:
                                goal = "Own a total of";
                                if (criterion.Type == null)
                                    throw new Exception("MeritValueType must have an assigned value.");
                                switch (criterion.Type.Value)
                                {
                                    case MeritValueType.Balance:
                                        em = EmojiIndex.Balance;
                                        break;
                                    case MeritValueType.Debt:
                                        em = EmojiIndex.Debt;
                                        break;
                                }
                                value = $"{em}**{criterion.Bound.ToPlaceValue()}**.";
                                break;
                            case MeritGoalType.Collect:
                                goal = "Collect";
                                if (criterion.Type == null)
                                    throw new Exception("MeritValueType must have an assigned value.");
                                em = EmojiIndex.Balance;
                                switch (criterion.Type.Value)
                                {
                                    case MeritValueType.Balance:
                                        em = EmojiIndex.Balance;
                                        break;
                                    case MeritValueType.Debt:
                                        em = EmojiIndex.Debt;
                                        break;
                                }
                                value = $"{em}**{criterion.Bound.ToPlaceValue()}**.";
                                break;
                            case MeritGoalType.Event:
                                goal = "Connect with";
                                if (criterion.Event == null)
                                    throw new Exception("MeritEventType must have an assigned value.");
                                em = EmojiIndex.Balance;
                                switch (criterion.Event.Value)
                                {
                                    case EventType.Daily:
                                        value = $"the daily headmaster **{criterion.Bound.ToPlaceValue()}** time{(criterion.Bound > 1 ? "s" : "")}.";
                                        break;
                                    case EventType.Vote:
                                        value = $"the vote marker **{criterion.Bound.ToPlaceValue()}** time{(criterion.Bound > 1 ? "s" : "")}.";
                                        break;
                                    case EventType.Midas:
                                        value = $"the Midas Touch **{criterion.Bound.ToPlaceValue()}** time{(criterion.Bound > 1 ? "s" : "")}.";
                                        break;
                                }
                                break;
                        }

                        string crit = $"{goal} {value}";
                        sb.AppendLine($"• {crit}");
                    }
                }
            }
            if (merit.Rewards.Funct())
            {
                sb.AppendLine($"\n**Rewards**:");
                foreach (Reward reward in merit.Rewards)
                {
                    string value = "";
                    switch (reward.Type)
                    {
                        case RewardType.Item:
                            if (reward.Item == null)
                                throw new Exception("The OriItem required to reward is missing.");
                            value = $"**{reward.Item.Name}**{(reward.Amount > 1 ? $" x{reward.Amount.ToPlaceValue()}" : "")}";
                            break;
                        case RewardType.Money:
                            value = $"{EmojiIndex.Balance}**{reward.Amount}**";
                            break;
                    }

                    sb.AppendLine($"• {value}");
                }
            }

            eb.WithDescription(sb.ToString());

            //if (merit.IconUrl != null)
            //    await Context.Channel.SendFileAsync(merit.IconUrl, embed: eb.Build());
            //else
            await Context.Channel.SendEmbedAsync(eb.Build());
        }

        [Command("scalepixelate"), Alias("spxl")]
        [Summary("Renders a string of text using the new text generation system, resized to a specified scale.")]
        public async Task TestRenderAsync(int scale, [Remainder] string content)
        {
            int padding = 2;
            scale = PixelScale.Set(scale);
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            Bitmap bmp = RenderText(content, a, padding).Resize(scale);

            string file = $"{a.Id}_render.png";
            string path = $"{GetAccountDirectory(a)}{file}";

            SaveRender(bmp, path);

            await SendRenderedString(a, Context.Channel, bmp, path);
        }
        */
        [Command("nickname"), Alias("nick")]
        [Summary("Set your account's nickname. (Use \\null to clear.)")]
        public async Task SetNicknameAsync([Remainder]string name = null)
        {
            var e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");

            if (name == null)
            {
                e.Color = EmbedData.GetColor("owo");
                await ReplyAsync(embed: (Context.Account.Config.Nickname ?? "You currently lack a nickname.").ToEmbedTitle(e).Build());
                return;
            }

            if (name == "\\null")
            {
                Context.Account.Config.SetNickname(null);
                await ReplyAsync(embed: "Your nickname has been cleared.".ToEmbedTitle(e).Build());
                return;
            }

            int maxLetters = 32;
            if (name.Length > maxLetters)
            {
                Embed z = EmbedData.Throw(Context, "Invalid nickname.", "Nicknames cannot be higher than 32 characters.", false);
                await ReplyAsync(embed: z);
                return;
            }
            else if (name.Length < 2)
            {
                Embed z = EmbedData.Throw(Context, "Invalid nickname.", "You need at least 2 characters for a nickname.", false);
                await ReplyAsync(embed: z);
                return;
            }

            Context.Account.Config.SetNickname(name);
            await ReplyAsync(embed: $"Your nickname has been set to `{Context.Account.Config.Nickname}`".ToEmbedTitle(e).Build());
        }

        public Embed GenerateServerBox(Server s)
        {
            var e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("error");

            SocketGuild g = s.Guild(Context.Client);
            string inbound = $"{s.Config.InboundChannel}";

            if (g.TextChannels.Any(x => x.Id == s.Config.InboundChannel))
            {


                SocketTextChannel t = g.TextChannels.Where(x => x.Id == s.Config.InboundChannel).First();
                inbound = $"(Bound to {t.Mention})";

            }
            else
            {
                if (Context.TryGetPrimaryChatChannel(g, out SocketTextChannel ch))
                {
                    inbound = $"| {ch.Mention} **(Unbounded)**";
                }
                else
                {
                    inbound = $"| Invalidated **(Empty Usable Channels)**";
                }
            }

            e.WithTitle(s.Name);
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.Identifier.Pack(a)} {s.Id}");
            sb.AppendLine($"{EmojiIndex.Text.Pack(a)} **CrossChat** {(s.Config.CrossChat ? inbound : "| Disabled")}");
            //sb.AppendLine($"{EmojiIndex.Experience.Pack()} **Experience** Global");
            sb.AppendLine($"{EmojiIndex.ExceptionFlag.Pack(a)} **Exceptions** | {(s.Config.Throw ? "Verbose" : "Quiet")}");
            sb.AppendLine($"{EmojiIndex.Prefix.Pack(a)} **Prefix** | {(s.Config.UsePrefixes ? $"{s.Config.Prefix}example" : $"**Prefixes are disabled.** | {Context.Client.CurrentUser.Mention} example")}");

            e.WithDescription(sb.ToString());
            return e.Build();
        }

        // redo command statistics

        [Command("daily")]
        [Summary("Receive your daily funding.")]
        public async Task EarnDailyAsync()
        {
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);

            ulong reward = 15;
            EmbedBuilder e = EmbedData.DefaultEmbed;
            if (a.UsedDaily.Exists())
            {
                TimeSpan span = (DateTime.UtcNow - a.UsedDaily);

                if (span.TotalHours < 24)
                {
                    TimeSpan day = TimeSpan.FromHours(24);
                    TimeSpan cooldown = day - span;

                    e.WithColor(EmbedData.GetColor("error"));
                    e.WithTitle($"{EmojiIndex.FromHours(DateTime.UtcNow.Hour)} {cooldown.ToString(@"hh\:mm\:ss").MarkdownBold()}");
                    e.WithDescription(GetFlavorText());
                    await ReplyAsync(embed: e.Build());
                    return;
                }
                if (span.TotalHours > 48)
                {
                    if (a.DailyStreak > 1)
                    {

                        e.WithColor(EmbedData.GetColor("yield"));
                        e.WithTitle($"+ {EmojiIndex.Balance}{reward.ToPlaceValue().MarkdownBold()} (null)");
                        e.WithDescription(GetDailyResetText());
                        a.DailyStreak = 0;
                    }
                }
            }

            a.UsedDaily = DateTime.UtcNow;
            a.TickDaily();

            if (a.DailyStreak >= 5)
            {
                ulong bonus = 50;
                e.WithColor(EmbedData.GetColor("owo"));
                e.WithTitle($"+ {EmojiIndex.Balance}{reward.ToPlaceValue().MarkdownBold()} + {bonus.ToPlaceValue().MarkdownBold()} (x{a.DailyStreak})");
                e.WithDescription("You madman!");
                reward += bonus;
                a.DailyStreak = 0;
            }
            else
            {
                e.WithTitle($"+ {EmojiIndex.Balance}{reward.ToPlaceValue().MarkdownBold()} (x{a.DailyStreak})");
                e.WithDescription(GetFlavorDailyText());
            }

            a.Give(reward);
            await ReplyAsync(embed: e.Build());
        }

        //[Command("pay")]
        [Summary("Send your funds to another user.")]
        public async Task PayAccountAsync(ulong amount, [Remainder]SocketUser user)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");
            OldAccount payor = Context.Account;
            OldAccount payee = Context.Data.GetOrAddAccount(user);
            string s = "";
            if (amount == 0)
            {
                e.WithColor(EmbedData.GetColor("error"));
                s = "It'd probably be good to pay with money.";
                amount = 0;
            }
            if (payor.Id == payee.Id)
            {
                e.Color = EmbedData.GetColor("error");
                s = $"You cannot send money to yourself."; // Sending money to yourself is a no-no.
                amount = 0;
            }
            if (payor.Balance - (double)amount < 0) // doubles are used, since - ulong makes it positive.
            {
                if (!payor.Config.Overflow)
                {
                    e.Color = EmbedData.GetColor("error");
                    s = $"You don't have the funds to carry this out!";
                    amount = 0;
                }
                else
                {
                    s = $"Paid out {Formatting.ToBalance(payor.Balance)} to {user.Username} with remaining funds.";
                    amount = payor.Balance;
                }
            }
            else
                s = $"Sent {Formatting.ToBalance(amount)} to {user.Username}.";

            if (amount > 0)
            {
                payor.Donate(payee, amount);
                Context.Data.Update(payee);
            }
            await ReplyAsync(embed: s.ToEmbedTitle(e).Build());
        }
        /*
        [Command("balance"), Alias("bal")]
        [Summary("Returns a summary of your wallet status.")]
        public async Task CheckBalanceAsync([Remainder]SocketUser user = null)
        {
            OldAccount a = Context.Data.GetOrAddAccount(user ?? Context.User);

            EmbedBuilder eb = Embedder.DefaultEmbed.WithColor(a.Card.Schema.Palette[0].ToDiscordColor()).WithTitle(OriFormat.Balance(a.Balance, a.Debt));
            if (a.Id != Context.User.Id)
                eb.WithFooter(a.GetDefaultName());

            await Context.Channel.SendEmbedAsync(eb.Build());
        }
        */
        [Command("clean")]
        [Summary("Returns a string in which any Unicode value that pertains to an alphanumeric value is reverted.")]
        public async Task CleanTextAsync([Remainder]string value) =>
            await ReplyAsync(value.FromAny());

        //[Command("events")]
        public async Task ViewEvents(int page = 1)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithColor(EmbedData.GetColor("error"));
            e.WithTitle($"{EmojiIndex.Events}");
            e.WithDescription("The event planner isn't ready yet. Please hang on. :)");

            await ReplyAsync(embed: e.Build());
        }

        [Command("hash")]
        [Summary("Hash a piece of written text.")]
        [RequireOwner]
        public async Task HashTextAsync(string value)
        {
            string hash = HashBuilder.Generate(value);
            bool extracted = HashBuilder.TryGetHash(hash, out OriHash result);

            // To display a raw hash : \nraw:\n    ↳ {hash}\niteration:\n    ↳ {hashInfo["iteration"]}

            var e = EmbedData.SetEmbed(
                EmbedData.GetColor("origreen"),
                "Hash generated.",
                $"```\nversion:\n    ↳ {result.Version}\nhash:\n    ↳ {result.Hash}```");
            await ReplyAsync(null, false, e.Build());
        }

        [Command("key"), Priority(0)]
        [Summary("Generates a random key with a length of 8.")]
        public async Task GenerateKeyAsync()
        {
            var key = KeyBuilder.Generate();
            var e = EmbedData.SetEmbed(EmbedData.GetColor("origreen"), $"{key}");
            await ReplyAsync(null, false, e.Build());
        }

        [Command("key"), Priority(1)]
        [Summary("Generates a random key using a specified length.")]
        public async Task GenerateManualKeyAsync(
            [Summary("The length of the key to generate. (256 character limit)")]int length
            )
        {
            string key = KeyBuilder.Generate(length);
            var e = EmbedData.SetEmbed(EmbedData.GetColor("origreen"));
            if (key.Length > EmbedBuilder.MaxDescriptionLength)
                e.WithDescription("StackOverflowError: A key generation must be below 2,048 characters.");
            else if (key.Length > 48)
                e.WithDescription(key);
            else
                e.WithTitle(key);

            await ReplyAsync(null, false, e.Build());
        }
        
        [Command("reprofile"), Alias("rpf")]
        [Summary("Gets your license with Poxel.GetCard().")]
        public async Task GetReProfile()
        {
            var img = new ImageConfiguration();
            SocketUser u = Context.User;
            OldAccount a = Context.Data.GetOrAddAccount(u);
            var pfp = u.GetAvatarUrl(ImageFormat.Auto, 32);
            var baseUrl = Directory.CreateDirectory($".//Data//Users//{u.Id}//").FullName;
            string avatarId = pfp.Exists() ? u.AvatarId : "null";
            string filename = $"{avatarId}_profile-display.png";
            var setUrl = pfp.Exists() ? $"{baseUrl}{avatarId}.png" : ".//Templates//avatar_null.png";
            var path = $"{baseUrl}{filename}";

            var avatar = img.TryGetAvatar(pfp, setUrl);
            PixelRenderingOptions opt = new PixelRenderingOptions(a);
            avatar = PixelEngine.Pixelate(avatar, opt);
            var profile = PixelEngine.DrawCard(avatar, a.Config.Nickname ?? u.Username, u.Status.ToString(), GetActivityStatus(u), a.Data.Level, a.Balance, a.Data.Experience, (ulong)a.Data.ExperienceToNextLevel(), opt);
            profile = ImageConfiguration.Resize(profile, profile.Width * 2, profile.Height * 2);
            var format = SysImageFormat.Png;
            img.SaveAs(profile, path, format);

            var e = new EmbedBuilder();
            e.WithColor(opt.Palette[0].R, opt.Palette[0].G, opt.Palette[0].B);
            e.WithImageUrl($"attachment://{filename}");

            await Context.Channel.SendFileAsync(path, embed: e.Build());
        }

        [Command("profile"), Alias("pf"), Priority(0)]
        [Summary("Returns your profile license.")]
        public async Task GetProfile()
        {
            var img = new ImageConfiguration();
            SocketUser u = Context.User;
            OldAccount a = Context.Data.GetOrAddAccount(u);
            var pfp = u.GetAvatarUrl(ImageFormat.Auto, 32);
            var baseUrl = Directory.CreateDirectory($".//Data//Users//{u.Id}//").FullName;
            string avatarId = pfp.Exists() ? u.AvatarId : "null";
            string filename = $"{avatarId}_profile-display.png";
            var setUrl = pfp.Exists() ? $"{baseUrl}{avatarId}.png" : ".//Templates//avatar_null.png";
            var path = $"{baseUrl}{filename}";

            var avatar = img.TryGetAvatar(pfp, setUrl);
            PixelRenderingOptions opt = new PixelRenderingOptions(a);
            avatar = PixelEngine.Pixelate(avatar, opt);
            var profile = TemplateBuilder.BuildNewProfileBaseTemplate(avatar, a.Config.Nickname ?? u.Username, u.Status.ToString(), GetActivityStatus(u), a.Data.Level, a.Balance, a.Data.Experience, (ulong)a.Data.ExperienceToNextLevel(), opt);
            profile = ImageConfiguration.Resize(profile, profile.Width * 2, profile.Height * 2);
            var format = SysImageFormat.Png;
            img.SaveAs(profile, path, format);

            var e = new EmbedBuilder();
            e.WithColor(opt.Palette[0].R, opt.Palette[0].G, opt.Palette[0].B);
            e.WithImageUrl($"attachment://{filename}");

            await Context.Channel.SendFileAsync(path, embed: e.Build());
        }

        [Command("profile"), Alias("pf"), Priority(1)]
        [Summary("Attempts to retrieve a profile by a written name.")]
        public async Task GetProfile([Remainder]string name)
        {
            var img = new ImageConfiguration();
            IEnumerable<OldAccount> matches = Context.Data.Accounts.Values.Where(x => x.Callers.Any(y => y.ToLower() == name.ToLower()));
            if (!matches.Funct())
            {
                await Context.Channel.ThrowAsync("No matches found.", "The input received resulted in no matches.");
                return;
            }
            OldAccount a = matches.First();
            a.ToString().Debug("found match");
            SocketUser u = Context.Client.GetUser(a.Id);
            if (!u.Exists())
            {
                u = Context.Client.GetUser(a.Username, $"#{a.DiscriminatorValue}");
            }
            if (!u.Exists())
            {
                await Context.Channel.ThrowAsync("No guild connection.", "The account was found, but doesn't exist as a direct connection.");
                return;
            }
            //Context.Data.GetOrAddAccount(u);
            var pfp = u.GetAvatarUrl(ImageFormat.Auto, 32);
            var baseUrl = Directory.CreateDirectory($".//Data//Users//{u.Id}//").FullName;
            string avatarId = pfp.Exists() ? u.AvatarId : "null";
            string filename = $"{avatarId}_profile-display.png";
            var setUrl = pfp.Exists() ? $"{baseUrl}{avatarId}.png" : ".//Templates//avatar_null.png";
            var path = $"{baseUrl}{filename}";

            var avatar = img.TryGetAvatar(pfp, setUrl);
            PixelRenderingOptions opt = new PixelRenderingOptions(a);
            avatar = PixelEngine.Pixelate(avatar, opt);
            var profile = TemplateBuilder.BuildNewProfileBaseTemplate(avatar, a.Config.Nickname ?? u.Username, u.Status.ToString(), GetActivityStatus(u), a.Data.Level, a.Balance, a.Data.Experience, (ulong)a.Data.ExperienceToNextLevel(), opt);
            profile = ImageConfiguration.Resize(profile, profile.Width * 2, profile.Height * 2);
            var format = SysImageFormat.Png;
            img.SaveAs(profile, path, format);

            var e = new EmbedBuilder();
            e.WithColor(opt.Palette[0].R, opt.Palette[0].G, opt.Palette[0].B);
            e.WithImageUrl($"attachment://{filename}");

            await Context.Channel.SendFileAsync(path, embed: e.Build());
        }

        [Command("profile"), Alias("pf"), Priority(2)]
        [Summary("Returns a profile license using an ID.")]
        public async Task GetProfile([Summary("The identifier of a user.")]ulong id)
        {
            var img = new ImageConfiguration();
            SocketUser u = Context.User;
            OldAccount a = Context.Data.GetOrAddAccount(u);
            if (Context.Data.Accounts.ContainsKey(id))
            {
                try
                {
                    a = Context.Data.GetOrAddAccount(id);
                    u = Context.Client.GetUser(id);
                    if (u == null)
                    {
                        await ReplyAsync("This account does not exist.");
                        return;
                    }
                }
                catch
                {
                    await ReplyAsync("This account does not exist.");
                    return;
                }
            }

            var pfp = u.GetAvatarUrl(ImageFormat.Auto, 32);
            var baseUrl = Directory.CreateDirectory($".//Data//Users//{u.Id}//").FullName;
            string avatarId = pfp.Exists() ? u.AvatarId : "null";
            string filename = $"{avatarId}_profile-display.png";
            var setUrl = pfp.Exists() ? $"{baseUrl}{avatarId}.png" : ".//Templates//avatar_null.png";
            var path = $"{baseUrl}{filename}";

            var avatar = img.TryGetAvatar(pfp, setUrl);
            PixelRenderingOptions opt = new PixelRenderingOptions(a);
            avatar = PixelEngine.Pixelate(avatar, opt);
            var profile = TemplateBuilder.BuildNewProfileBaseTemplate(avatar, a.Config.Nickname ?? u.Username, u.Status.ToString(), GetActivityStatus(u), a.Data.Level, a.Balance, a.Data.Experience, (ulong)a.Data.ExperienceToNextLevel(), opt);
            profile = ImageConfiguration.Resize(profile, profile.Width * 2, profile.Height * 2);
            var format = SysImageFormat.Png;
            img.SaveAs(profile, path, format);

            var e = new EmbedBuilder();
            e.WithColor(opt.Palette[0].R, opt.Palette[0].G, opt.Palette[0].B);
            e.WithImageUrl($"attachment://{filename}");

            await Context.Channel.SendFileAsync(path, embed: e.Build());
        }

        [Command("profile"), Alias("pf"), Priority(3)]
        [Summary("Returns a profile license using a mention.")]
        public async Task GetProfile([Remainder] SocketUser u)
        {
            var img = new ImageConfiguration();
            u = u ?? Context.User;
            OldAccount a = Context.Data.GetOrAddAccount(u);
            var pfp = u.GetAvatarUrl(ImageFormat.Auto, 32);
            var baseUrl = Directory.CreateDirectory($".//Data//Users//{u.Id}//").FullName;
            string avatarId = pfp.Exists() ? u.AvatarId : "null";
            string filename = $"{avatarId}_profile-display.png";
            var setUrl = pfp.Exists() ? $"{baseUrl}{avatarId}.png" : ".//Templates//avatar_null.png";
            var path = $"{baseUrl}{filename}";

            var avatar = img.TryGetAvatar(pfp, setUrl);
            PixelRenderingOptions opt = new PixelRenderingOptions(a);
            avatar = PixelEngine.Pixelate(avatar, opt);
            var profile = TemplateBuilder.BuildNewProfileBaseTemplate(avatar, a.Config.Nickname ?? u.Username, u.Status.ToString(), GetActivityStatus(u), a.Data.Level, a.Balance, a.Data.Experience, (ulong)a.Data.ExperienceToNextLevel(), opt);
            profile = ImageConfiguration.Resize(profile, profile.Width * 2, profile.Height * 2);
            var format = SysImageFormat.Png;
            img.SaveAs(profile, path, format);

            var e = new EmbedBuilder();
            e.WithColor(opt.Palette[0].R, opt.Palette[0].G, opt.Palette[0].B);
            e.WithImageUrl($"attachment://{filename}");

            await Context.Channel.SendFileAsync(path, embed: e.Build());
        }

        [Command("design"), Alias("d")]
        [Summary("View and simulate designs before implementation.")]
        public async Task GetDesign(string context = null)
        {
            List<string> links = new List<string>
            {
                ".//resources//tmp_profile.png",
                ".//resources//tmp_musicplayer.png",
                ".//resources//tmp_musicskip.png",
                ".//resources//tmp_musicskip2.png",
                ".//resources//tmp_levelup.png",
                ".//resources//tmp_moduleicons.png",
                ".//resources//tmp_loss.png",
                ".//resources//tmp_shops.png",
                ".//resources//tmp_itemviewer.png",
                ".//resources//tmp_levelicons.png",
                ".//resources//tmp_pagebar.png",
                ".//resources//tmp_werewolfrole.png",
                ".//resources//tmp_versionicon.png",
                ".//resources//tmp_moduleicons2.png"
            };

            List<string> names = new List<string>
            {
                "profile", "musicplayer", "musicskip", "musicskip2",
                "levelup", "modules", "loss", "shops", "itemviewer",
                "itemviewer2", "levelicons", "pagebar", "werewolfrole",
                "version", "modulesv2"
            };

            context = (context ?? "").ToLower();
            EmbedBuilder e = EmbedData.DefaultEmbed;
            string url = null;

            switch (context)
            {
                case "profile":
                    url = links[0];
                    break;
                case "musicplayer":
                    url = links[1];
                    break;
                case "musicskip":
                    url = links[2];
                    break;
                case "musicskip2":
                    url = links[3];
                    break;
                case "levelup":
                    url = links[4];
                    break;
                case "modules":
                    e.WithDescription("orikivo : modules".DiscordBlock());
                    url = links[5];
                    break;
                case "loss":
                    url = links[6];
                    break;
                case "shops":
                    url = links[7];
                    break;
                case "itemviewer":
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"**Wench Fragment** | {EmojiIndex.Balance.Pack(Context.Account)}**100**");
                    e.WithDescription(sb.ToString());
                    url = links[8];
                    e.WithFooter("One of the pieces used to form a Wench.");
                    break;
                case "itemviewer2":
                    StringBuilder sb2 = new StringBuilder();
                    sb2.Append($"Wench Fragment | {EmojiIndex.Balance}100 | 8.10");
                    url = links[8];
                    e.WithFooter(sb2.ToString());
                    break;
                case "levelicons":
                    url = links[9];
                    break;
                case "pagebar":
                    url = links[10];
                    break;
                case "werewolfrole":
                    url = links[11];
                    break;
                case "version":
                    url = links[12];
                    break;
                case "modulesv2":
                    url = links[13];
                    break;
                default:
                    break;
            }
            if (url == null)
            {
                await ReplyAsync($"**Design Templates**\n{names.Conjoin(" | ")}");
                return;
            }

            e.WithLocalImageUrl(url);
            await Context.Channel.SendFileAsync(url, embed: e.Build());
        }

        [Command("ratios")]
        [Summary("View the correct max ranges for image sizes for embeds.")]
        public async Task GetRatios()
        {
            var em = new EmbedBuilder
            {
                Color = EmbedData.GetColor("origreen")
            };
            em.AddField(x =>
            {
                x.Name = "**Image Ratios**";
                x.Value = "**16:9** | (400, 225)\n" +
                          "**4:3**  | (400, 300)\n" +
                          "**1:1**  | (300, 300)\n" +
                          "**1:2**  | (400, 200)\n" +
                          "**2:1**  | (150, 300)";
            });
            em.AddField(x =>
            {
                x.Name = "**Thumbnail Ratios**";
                x.Value = "**16:9** | (80, 45)\n" +
                          "**4:3**  | (80, 60)\n" +
                          "**1:1**  | (80, 80)\n" +
                          "**1:2**  | (80, 40)\n" +
                          "**2:1**  | (40, 80)";
            });
            await ReplyAsync("", false, em.Build());
        }

        // redo insult command

        // redo status command, to be compatible with OriUser.
        [Group("status"), Name("Status"), Alias("st")]
        [Summary("Prevent mentions from bugging you when you need it.")]
        public class Status : ModuleBase<OrikivoCommandContext>
        {
            private readonly CommandService _service;
            private readonly DiscordSocketClient _socket;
            private readonly IConfigurationRoot _config;
            private readonly StatusService _status;

            public Status(CommandService service, IConfigurationRoot config, DiscordSocketClient socket, StatusService status)
            {
                _service = service;
                _config = config;
                _socket = socket;
                _status = status;
            }

            [Command("")]
            [Summary("Displays your current status, or of the user called.")]
            public async Task StatusAsync([Remainder]SocketUser userRef = null)
            {
                var sender = userRef;
                if (userRef == null)
                {
                    sender = Context.Message.Author;
                }

                await _status.StatusRead(sender, Context);
            }

            [Command("clear")]
            [Summary("Clear your current status.")]
            public async Task StatusSet()
            {
                await _status.ClearStatus(Context);
            }

            [Command("set")]
            [Summary("Set your status with an optional message.")]
            public async Task StatusSet(string statusType, [Remainder]string message = "")
            {
                await _status.SetStatus(Context, statusType, message);
            }
        }


        [Command("leaderboard"), Alias("lb"), Priority(1)]
        [Summary("Retrieve a leaderboard with a specified value.")]
        public async Task GetLeaderboardUsers(string board, int page = 1)
        {
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            LeaderboardType type = ReadTypeContext(board);
            List<OldAccount> users = GetLeaderboard(type);
            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"**Leaderboard** | {type.ToString()}";
            List<string> names = new List<string>();
            foreach (OldAccount x in users)
                names.Add($"{x.Username.MarkdownBold()}\n{ReadDataType(x, type)}");
            e.WithFooter($"Rank: {(users.Contains(Context.Account) ? $"{(users.IndexOf(Context.Account) + 1).ToPositionValue()}" : "null")}");
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }

        [Command("boards")]
        [Summary("View the types of leaderboards that exist.")]
        public async Task ViewLeaderboardTypes()
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();

            e.WithTitle("Leaderboard Guide - Board Types");
            StringBuilder sb = new StringBuilder();

            // By money stats
            sb.AppendLine($"(balance) | **Balance** - Sorts by the current wallet value.");
            sb.AppendLine($"(most) | **Most Held** - Sorts users by the most they've held at a certain point in time.");
            sb.AppendLine($"(spent) | **Expended** - Sorts by the amount of money a user has spent.");
            sb.AppendLine($"(midas) | **Midas** - Sorts by the amount of times a user has been given the Midas touch.");
            //sb.AppendLine($"(moneywon) | **Most Money Won** - Sorts by the most money a user has won at once.");
            //sb.AppendLine($"(moneylost) | **Most Money Lost** - Sorts by the most money a user has lost at once.");
            sb.AppendLine($"(debt) | **Debt** - Sorts by the current debt value.");

            // By experience stats
            //sb.AppendLine($"(exp) | **Experience** - Sorts by current experience value.");
            //sb.AppendLine($"(mail) | **Mail Sent** - Sorts by the amount of times a mail was sent.");


            //f.WithText($"Tip: Appending + or - after the context specifies the direction!");
            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
        }

        [Command("top")]
        [Summary("View the top 10 users from a specified value.")]
        public async Task GetTopUsers()
        {
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            ICollection<OldAccount> users = Context.Data.Accounts.Values.Where(z => z.Balance > 0).OrderByDescending(n => n.Balance).ToList();

            List<EmbedBuilder> embeds = new List<EmbedBuilder>();
            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.WithColor(EmbedData.GetColor("origreen"));
            e.WithTitle($"(Balance) | Top 10");

            StringBuilder sb = new StringBuilder();

            int x = 1;
            foreach (var u in users)
            {
                if (x > 10)
                {
                    break;
                }

                string user = $"{u.GetName()} | {EmojiIndex.Balance.Pack(a)}{u.Balance.ToPlaceValue().MarkdownBold()}";

                switch (x)
                {
                    case 1:
                        sb.AppendLine($"**First Place** | {u.GetName()}\n{EmojiIndex.Balance.Pack(a)}{u.Balance.ToPlaceValue().MarkdownBold()}");
                        break;
                    case 2:
                        sb.AppendLine($"**Second Place** | {u.GetName()}\n{EmojiIndex.Balance.Pack(a)}{u.Balance.ToPlaceValue().MarkdownBold()}");
                        break;
                    case 3:
                        sb.AppendLine($"**Third Place** | {u.GetName()}\n{EmojiIndex.Balance.Pack(a)}{u.Balance.ToPlaceValue().MarkdownBold()}");
                        break;
                    default:
                        if (x == 4)
                            sb.AppendLine("\n\n**Runnerups**");
                        sb.AppendLine($"{x.ToPositionValue()} {user}");
                        break;
                }
                x += 1;
            }

            e.WithDescription(sb.ToString());

            await ReplyAsync(embed: e.Build());
        }

        [Command("shops")]
        public async Task GetShops()
        {
            string list = ShopManager.ShopMap.Shops.Enumerate(x => x.Name).Conjoin("\n");
            await ReplyAsync(string.IsNullOrWhiteSpace(list)? "err":list);
        }

        [Command("pawn")]
        public async Task Pawn()
        {
            OriShop shop = ShopManager.ShopMap.Shops.FirstOrDefault();
            EmbedBuilder eb = Embedder.DefaultEmbed;
            eb.WithTitle(shop.Name);
            StringBuilder sb = new StringBuilder();
            sb.AppendLines(shop.Vendor.Name.MarkdownBold(), shop.Vendor.Responses.OnShopEntry[RandomProvider.Instance.Next(shop.Vendor.Responses.OnShopEntry.Count - 1)]);
            sb.AppendLine($"Loot Group: `{shop.LootGroup}`");
            eb.WithDescription(sb.ToString());
            await Context.Channel.SendEmbedAsync(eb.Build());
        }

        [Command("pocket")]
        public async Task Pocket()
        {
            OriItem item = ItemManager.ItemMap.Items.FirstOrDefault();
            EmbedBuilder eb = Embedder.DefaultEmbed;
            eb.WithTitle(item.Name);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{item.Rarity} {item.Group}");
            if (item.MarketTag.Value.HasValue)
                sb.AppendLine($"{EmojiIndex.Balance}**{item.MarketTag.Value.Value}**");
            sb.AppendLine(item.FlavorText);
            eb.WithDescription(sb.ToString());
            eb.WithLocalImageUrl(item.IconPath);
            await Context.Channel.SendFileAsync(item.IconPath, embed: eb.Build());
        }



        #region tools
        public string GetDailyResetText()
        {
            List<string> l = new List<string>
            {
                "You took too long. Now your candy's gone.",
                "I know I said to be patient, but really?",
                "I haven't seen you in a orica." // orica - the time that was passed when earth died.
            };

            int x = RandomProvider.Instance.Next(1, l.Count + 1);
            return l[x - 1];
        }

        public string GetFlavorText()
        {
            List<string> l = new List<string>
            {
                "You can't request just yet. Hang tight.",
                "Be patient, young one.",
                "Impurity leads to a dim path.",
                "Your authorities have no place here."
            };

            int x = RandomProvider.Instance.Next(1, l.Count + 1);
            return l[x - 1];
        }

        public string GetFlavorDailyText()
        {
            List<string> l = new List<string>
            {
                "Enjoy the funds, bucko.",
                "Hey, it's on the house!",
                "This is probably the only safe way to earn money."
            };

            int x = RandomProvider.Instance.Next(1, l.Count + 1);
            return l[x - 1];
        }

        public string GetFontStyles(FontFace font)
        {
            StringBuilder sb = new StringBuilder();

            foreach (FontStyle style in font.Styles)
            {
                sb.Append($"**{style.Type}** ");
            }
            return sb.ToString();
        }

        public string GetFontSummary(FontFace font)
        {
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            string url = "https://www.google.com";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.Permissions.Pack(a)} {font.Name}");
            sb.AppendLine($"[{EmojiIndex.Identifier.Pack(a)}]({url}) {font.Id}");
            //sb.AppendLine($"{EmojiIndex.Size.Pack()} {font.Size.ToString()}");
            //sb.AppendLine($"**Overhang** | {font.Overhang}px"); // (Vertical push strength for lowercase hanging letters)
            //sb.AppendLine($"**Padding** | {font.Padding}px"); // (The space in between letters)
            //sb.AppendLine($"**Styles** | {font.Styles.Count}"); // (Different visual effects for a font face)

            return sb.ToString();
        }

        public Bitmap RenderText(string content, OldAccount a, int padding = 0)
        {
            return content.Render(padding, new PixelRenderingOptions(a));
        }

        /*
        public async Task SendRenderedString(OldAccount a, ISocketMessageChannel channel, Bitmap bmp, string path)
        {
            Embed e = GenerateAttachmentBox(a, bmp, path);
            await channel.SendFileAsync(path, embed: e);
        }*/

        public void SaveRender(Bitmap bmp, string path)
        {
            bmp.SaveBitmap(path, SysImageFormat.Png);
        }

        public string GetAccountDirectory(OldAccount a)
        {
            return Directory.CreateDirectory($".//data//{a.Id}//").FullName;
        }

        /*
        public Embed GenerateAttachmentBox(OldAccount a, Bitmap bmp, string path)
        {
            const string ATTACHMENT = "attachment://{0}";
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithColor(a.Card.Schema.Palette[0].ToDiscordColor());
            e.WithImageUrl(string.Format(ATTACHMENT, Path.GetFileName(path)));
            return e.Build();
        }
        */
        public Embed GenerateAttachmentBox(Bitmap bmp, string path, string file)
        {
            const string ATTACHMENT = "attachment://{0}";
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithImageUrl(string.Format(ATTACHMENT, file));
            return e.Build();
        }
        /*
        public async Task GetCommandAnalytics(OldAccount a, int page = 1)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.Color = EmbedData.GetColor("origreen");
            if (a.Analytics.Commands.Count.Equals(0))
            {
                e.Color = EmbedData.GetColor("error");
                e.WithTitle($"{a.GetName()} lacks interaction.");
                e.WithDescription("This user has not used any known commands.");
                await ReplyAsync(embed: e.Build());
                return;
            }

            List<KeyValuePair<string, CommandAnalyzer>> list = a.Analytics.Commands.OrderByDescending(x => x.Value.Counter).ToList();
            List<Embed> embeds = new List<Embed>();

            page = page.InRange(1, list.Count) - 1;
            string placement = "";
            int z = page + 1;
            if (z == 1)
            {
                placement = $"{EmojiIndex.GoldMedal}";
                e.Color = new Color(255, 215, 0);
            }
            else if (z == 2)
            {
                placement = $"{EmojiIndex.SilverMedal}";
                e.Color = new Color(192, 192, 192);
            }
            else if (z == 3)
            {
                placement = $"{EmojiIndex.BronzeMedal}";
                e.Color = new Color(205, 127, 50);
            }
            else
            {
                e.Color = EmbedData.GetColor("origreen");
                placement = $"({z.ToPositionValue()})".MarkdownBold();
            }
            KeyValuePair<string, CommandAnalyzer> pair = list[page];

            ulong counter = pair.Value.Counter;
            string index = $"x{counter.ToPlaceValue()} | {page + 1} of {list.Count}";

            e.Title = placement + " " + pair.Key;
            e.Description = $"{pair.Value.Date}";
            e.Footer = new EmbedFooterBuilder().WithText(index);
            await ReplyAsync(embed: e.Build());
        }
        */
        /*
        public async Task GetCommandAnalytics(SocketUser u, int page = 0)
        {
            OldAccount tmp = Context.Data.GetOrAddAccount(u);
            await GetCommandAnalytics(tmp, page);
        }*/

        public string GetActivityStatus(SocketUser sender)
        {
            UserStatus onlineSet = UserStatus.Online;
            UserStatus busySet = UserStatus.DoNotDisturb;
            UserStatus[] idleSet = { UserStatus.Idle, UserStatus.AFK };
            UserStatus[] offlineSet = { UserStatus.Invisible, UserStatus.Offline };

            var activityNullString = "";

            if (offlineSet.Contains(sender.Status))
            {
                activityNullString = "Offline";
            }
            else if (idleSet.Contains(sender.Status))
            {
                activityNullString = "Idling";
            }
            else if (busySet.Equals(sender.Status))
            {
                activityNullString = "Busy";
            }
            else if (onlineSet.Equals(sender.Status))
            {
                activityNullString = "Online";
            }

            var userActivity = sender.Activity != null ? $"{sender.Activity.Type} {sender.Activity.Name}" : activityNullString;

            if (sender.Activity != null)
            {
                if ($"{sender.Activity.Type}".ToLower() == "listening")
                {
                    userActivity = $"Listening to {sender.Activity.Name}";
                }
            }

            return userActivity;
        }

        public ulong GetLevel(ulong exp)
        {
            return exp / 1000;
        }

        //Build experience, currency, level, and preference table
        

        public Func<OldAccount, object> SetBoardFunc(LeaderboardType type)
        {
            switch (type)
            {
                case LeaderboardType.Midas:
                    return delegate(OldAccount a) { return a.GimmeStats.GoldenCount; };
                case LeaderboardType.MostHeld:
                    return delegate (OldAccount a) { return a.Analytics.MaxHeld; };
                case LeaderboardType.Expended:
                    return delegate (OldAccount a) { return a.Analytics.Expended; };
                case LeaderboardType.Debt:
                    return delegate (OldAccount a) { return a.Debt; };
                default:
                    return delegate (OldAccount a) { return a.Balance; };
            }
        }

        public List<OldAccount> GetLeaderboard(LeaderboardType type)
        {
            List<OldAccount> users = Context.Data.Accounts.Values.ToList();
            foreach (OldAccount a in users)
            {
                if (!a.GimmeStats.Exists())
                    a.GimmeStats = new GiveOrTakeAnalyzer();
            }
            return users.OrderByDescending(SetBoardFunc(type)).ToList();
        }

        public LeaderboardType ReadTypeContext(string ctx)
        {
            ctx = ctx.ToLower();
            switch(ctx)
            {
                case "spent": return LeaderboardType.Expended;
                case "most": return LeaderboardType.MostHeld;
                case "debt": return LeaderboardType.Debt;
                case "midas": return LeaderboardType.Midas;
                default: return LeaderboardType.Balance;
            }
        }

        public string ReadDataType(OldAccount a, LeaderboardType type)
        {
            OldAccount x = Context.Account;
            switch(type)
            {
                case LeaderboardType.Expended:
                    return $"{EmojiIndex.Expended.Pack(x)}{a.Analytics.Expended.ToPlaceValue().MarkdownBold()}";
                case LeaderboardType.MostHeld:
                    return $"{EmojiIndex.MostHeld.Pack(x)}{a.Analytics.MaxHeld.ToPlaceValue().MarkdownBold()}";
                case LeaderboardType.Debt:
                    return $"{EmojiIndex.Debt.Pack(x)}{a.Debt.ToPlaceValue().MarkdownBold()}";
                case LeaderboardType.Midas:
                    return $"{EmojiIndex.Midas.Pack(x)}{a.GimmeStats.GoldenCount.ToPlaceValue().MarkdownBold()}";
                default:
                    return $"{EmojiIndex.Balance.Pack(x)}{a.Balance.ToPlaceValue().MarkdownBold()}";
            }
        }
        /*
        [Command("baseorivatar"), Alias("obfp")]
        [Summary("Generate your avatar under your current color scheme, without pixelation. (default size is 256px)")]
        public async Task GenerateOrivatarAsync(SocketUser user = null)
        {
            user = user ?? Context.User;
            OldAccount a = Context.Data.GetOrAddAccount(user);

            ImageFormat f = ImageFormat.Png;
            string dir = Directory.CreateDirectory($".//data//{user.Id}//").FullName;
            string path = $"{dir}orivatar_render{f.GetExtensionName()}";
            string localpath = $"{dir}avatar{f.GetExtensionName()}";
            using (Bitmap bmp = AvatarManager.GetSetAvatarBitmap(user, localpath, 256, fallback: user.GetDefaultAvatarUrl()))
            {
                Bitmap avatar = bmp.ToPalette(a.Card.Schema.Palette);
                avatar.SaveBitmap(path, f);
                avatar.Dispose();

                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(a.Card.Schema.Palette[0].ToDiscordColor());
                eb.WithLocalImageUrl(path);
                await Context.Channel.SendFileAsync(path, embed: eb.Build());
            }
        }
        
        [Command("orivatar"), Alias("ofp"), Priority(1)]
        [Summary("Generate your avatar under your current color scheme. All inputs are auto-corrected to its closest counterpart. (16px Per Block. Keep even.)")]
        public async Task GenerateOrivatarAsync(int ppu = 32, int scale = 2)
        {
            // limit to 512px
            int bppu = 16;
            List<int> ppus = new List<int>
            {
                bppu,      //16px
                bppu * 2,  //32px
                bppu * 4,  //64px
                bppu * 8,  //128px
                bppu * 16 //256px
            };
            List<int[]> scales = new List<int[]>
            {
                new int[] { 1, 2, 4, 8, 16 },
                new int[] { 1, 2, 4, 8 },
                new int[] { 1, 2, 4 },
                new int[] { 1, 2 },
                new int[] { 1 }
            };

            // auto corrector
            ppu.Debug();
            ppu = ppus.OrderBy(x => Math.Abs(ppu - x)).First();
            ppu.Debug();

            scale.Debug();
            scale = scales[ppus.IndexOf(ppu)].OrderBy(x => Math.Abs(scale - x)).First();
            scale.Debug();

            OldAccount a = Context.Account;
            if (!a.Exists())
            {
                a = Context.Data.GetOrAddAccount(Context.User);
            }

            ImageFormat f = ImageFormat.Png;
            string dir = Directory.CreateDirectory($".//data//{Context.User.Id}//").FullName;
            string path = $"{dir}orivatar_render{f.GetExtensionName()}";
            string localpath = $"{dir}avatar{f.GetExtensionName()}"; // _{KeyBuilder.Generate()
            using (Bitmap bmp = AvatarManager.GetSetAvatarBitmap(Context.User, localpath, (ushort)ppu))
            {
                Bitmap avatar = bmp.ToPalette(a.Card.Schema.Palette).Resize(scale);
                avatar.SaveBitmap(path, f);
                avatar.Dispose();

                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(a.Card.Schema.Palette[0].ToDiscordColor());
                eb.WithLocalImageUrl(path);
                await Context.Channel.SendFileAsync(path, embed: eb.Build());
            }
        }

        [Command("orivatar"), Alias("ofp"), Priority(0)]
        [Summary("Generate your avatar under your current color scheme. All inputs are auto-corrected to its closest counterpart. (16px Per Block. Keep even.)")]
        public async Task GenerateOrivatarAsync(SocketUser u, int ppu = 32, int scale = 2)
        {
            // limit to 512px
            int bppu = 16;
            List<int> ppus = new List<int>
            {
                bppu,      //16px
                bppu * 2,  //32px
                bppu * 4,  //64px
                bppu * 8,  //128px
                bppu * 16 //256px
            };
            List<int[]> scales = new List<int[]>
            {
                new int[] { 1, 2, 4, 8, 16 },
                new int[] { 1, 2, 4, 8 },
                new int[] { 1, 2, 4 },
                new int[] { 1, 2 },
                new int[] { 1 }
            };

            // auto corrector
            ppu.Debug();
            ppu = ppus.OrderBy(x => Math.Abs(ppu - x)).First();
            ppu.Debug();

            scale.Debug();
            scale = scales[ppus.IndexOf(ppu)].OrderBy(x => Math.Abs(scale - x)).First();
            scale.Debug();

            OldAccount a = Context.Data.GetOrAddAccount(u);

            ImageFormat f = ImageFormat.Png;
            string dir = Directory.CreateDirectory($".//data//{u.Id}//").FullName;
            string path = $"{dir}orivatar_render{f.GetExtensionName()}";
            string localpath = $"{dir}avatar{f.GetExtensionName()}"; // _{KeyBuilder.Generate()
            using (Bitmap bmp = AvatarManager.GetSetAvatarBitmap(u, localpath, (ushort)ppu))
            {
                Bitmap avatar = bmp.ToPalette(a.Card.Schema.Palette).Resize(scale);
                avatar.SaveBitmap(path, f);
                avatar.Dispose();

                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(a.Card.Schema.Palette[0].ToDiscordColor());
                eb.WithLocalImageUrl(path);
                await Context.Channel.SendFileAsync(path, embed: eb.Build());
            }
        }
        */
        [Command("leaderboard"), Alias("lb"), Priority(0)]
        [Summary("View the complete list of accounts, sorted by balance.")]
        public async Task GetLeaderboardUsers(int page = 1)
        {
            OldAccount a = Context.Account ?? Context.Data.GetOrAddAccount(Context.User);
            List<OldAccount> users = GetLeaderboard(LeaderboardType.Balance);
            List<EmbedBuilder> embeds = new List<EmbedBuilder>();
            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            e.Color = EmbedData.GetColor("origreen");
            e.Title = $"**Leaderboard** | Balance";

            List<string> names = new List<string>();

            foreach (OldAccount u in users)
                names.Add($"{u.Username.MarkdownBold()}\n{ReadDataType(u, LeaderboardType.Balance)}");
            e.WithFooter($"Rank: {(users.Contains(Context.Account) ? $"{(users.IndexOf(Context.Account) + 1).ToPositionValue()}" : "null")}");
            Embed r = EmbedData.GenerateEmbedList(names, page, e);
            await ReplyAsync(embed: r);
        }
        #endregion
    }
}
