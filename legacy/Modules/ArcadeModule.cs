using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Providers;
using Orikivo.Services;
using Orikivo.Static;
using Orikivo.Systems.Presets;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Modules
{
    [Name("Arcade")]
    [Summary("Enter the new dawn of useless entertainment.")]
    [DontAutoLoad]
    public class ArcadeModule : ModuleBase<OrikivoCommandContext>
    {
        public ArcadeModule()
        {

        }

        [Command("editsession")]
        public async Task EditSessionEmbed(string title, string description = null, string footer = null)
        {
            if (Context.Server.OpenGameSessions.Funct())
            {
                Context.Server.OpenGameSessions[0].Refresh(Context.Guild, title, description, footer);
                return;
            }

            await ReplyAsync("no sessions to edit.");
        }

        [Command("newsession")]
        [Summary("Start a new session test.")]
        public async Task SessionTest()
        {
            if (Context.Server.OpenGameSessions.Funct())
            {
                await ReplyAsync("A session is already open.");
                return;
            }
            GameSession.Build(Context.Guild, Context.Server, new WerewolfGame());
            await ReplyAsync("Session built.");
            Context.Data.Update(Context.Server);
        }



        [Command("endsession")]
        [Summary("Close all sessions")]
        public async Task SessionTest2()
        {
            if (Context.Server.OpenGameSessions.Funct())
            {
                Context.Server.CloseSessions(Context.Guild);
                await ReplyAsync("sessions closed.");
                return;
            }
            await ReplyAsync("no sessions found.");
        }

        [Command("games")]
        [Summary("View a list of available games.")]
        public async Task GamesResponseAsync()
        {
            WerewolfGame g = new WerewolfGame();
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("error"));
            e.WithTitle("Ori's Arcade Zone");
            e.WithDescription($"{g.Status} **{g.Name}**\n{g.Summary}");//"The arcade is currently closed. Please hang tight while the arcade is being repaired. Sorry about that.");
            e.WithFooter("The arcade is currently closed. :(");
            await ReplyAsync(embed: e.Build());
        }

        public async Task GetTempEmbed(string name)
        {
            switch (name)
            {
                case "rolegen": await ReplyAsync(embed: WerewolfEmbedder.RoleGenerationTemplate()); return;
                case "roleassign": await ReplyAsync(embed: WerewolfEmbedder.RoleAssignTemplate()); return;
                case "night1": await ReplyAsync(embed: WerewolfEmbedder.FirstNightTemplate()); return;
                case "night": await ReplyAsync(embed: WerewolfEmbedder.NightTemplate()); return;
                case "death1": await ReplyAsync(embed: WerewolfEmbedder.FirstDeathTemplate()); return;
                case "death": await ReplyAsync(embed: WerewolfEmbedder.DeathTemplate()); return;
                case "convict": await ReplyAsync(embed: WerewolfEmbedder.SuspicionTemplate()); return;
                case "motion2": await ReplyAsync(embed: WerewolfEmbedder.SecondMotionTemplate()); return;
                case "defense": await ReplyAsync(embed: WerewolfEmbedder.DefenseTemplate()); return;
                case "vote": await ReplyAsync(embed: WerewolfEmbedder.VotingTemplate()); return;
                case "seerscan": await ReplyAsync(embed: WerewolfEmbedder.SeerScanTemplate()); return;
                case "seeroutcome": await ReplyAsync(embed: WerewolfEmbedder.SeerOutcomeTemplate()); return;
                case "wolfget": await ReplyAsync(embed: WerewolfEmbedder.WerewolfSelectionTemplate()); return;
                case "wolfbreak": await ReplyAsync(embed: WerewolfEmbedder.WerewolfBreakdownTemplate()); return;
                case "wolfoutcome": await ReplyAsync(embed: WerewolfEmbedder.WerewolfOutcomeTemplate()); return;
            }
        }

        [Command("templates"), Alias("tmp")]
        [Summary("Preview a collection of display presets for a game.")]
        public async Task GetTemplatesAsync(string template = null)
        {
            List<string> temps = new List<string>
            {
                "rolegen", "roleassign",
                "night1", "night",
                "death1", "death",
                "convict", "motion2",
                "defense", "vote",
                "seerscan", "seeroutcome",
                "wolfget","wolfbreak","wolfoutcome"
            };

            if (template.Exists())
            {
                if (template.EqualsAny(temps))
                {
                    await GetTempEmbed(template);
                    return;
                }
            }

            await ReplyAsync($"Werewolf Base Templates: {string.Join(" || ", temps)}");
            return;

        }
    }

    [Name("Gambling")]
    [Summary("Game modes that focus on the core concept of risk and reward.")]
    public class GamblingModule : ModuleBase<OrikivoCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;

        public GamblingModule(DiscordSocketClient client, CommandService service)
        {
            _client = client;
            _service = service;
        }

        // a conversion service to allow betting as this.
        // 1 credit is 5
        // 3 credits is 15
        // 5 credits is 25

        [Command("doubleornothingall"), Alias("dbla")]
        [Summary("Attempt to double all of your money to a specified tick count.")]
        public async Task SimpleGambleAsync(int times = 1)
            => await DoubleOrNothingAsync(Context.Account, times);

        [Command("doubleornothing"), Alias("dbl")]
        [Summary("Attempt to double a specified value to a tick count.")]
        public async Task SimpleGambleAsync(ulong wager, int times = 1)
        => await DoubleOrNothingAsync(Context.Account, wager, times);

        [Command("giveortake"), Alias("gimme", "got")]
        [Summary("Play the game of luck, and see if I give you money, or just steal it like the sad bum I am.")]
        public async Task GetOrTakeResponseAsync()
            => await GetOrTakeWalletAsync(Context.Account);

        [Command("betroll"), Alias("br"), Priority(0)]
        [Summary("Wager on a D6, and choose the specific sides you think it will land on.")]
        public async Task DiceWagerResponseAsync(int wager, params int[] sides)
            => await WagerDiceRollAsync(Context.Account, wager, sides);

        [Command("betanyroll"), Alias("bnr"), Priority(1)]
        [Summary("Wager on a DN, where N is the amount of sides, and choose the specific sides you think it will land on.")]
        public async Task DiceWagerResponseAsync(int wager, int size, params int[] sides)
            => await WagerDiceRollAsync(Context.Account, wager, size, sides);

        [Command("betranged"), Alias("brg"), Priority(1)]
        [Summary("Wager on a D6, and choose the direction that the roll will land in at a specified midpoint.")]
        public async Task RangedDiceWagerResponseAsync(int wager, int midpoint, bool above = false)
            => await WagerDiceRollRangeAsync(Context.Account, wager, midpoint, above);

        [Command("betranged"), Alias("brg"), Priority(0)]
        [Summary("Wager on a DN, where N is the amount of sides, and choose the direction that the roll will land in at a specified midpoint.")]
        public async Task RangedDiceWagerResponseAsync(int wager, int size, int midpoint, bool above = false)
            => await WagerDiceRollRangeAsync(Context.Account, wager, size, midpoint, above);

        [Command("betflip"), Alias("bf")]
        [Summary("Wager on a coin flip. The face is tails by default.")]
        public async Task CoinWagerResponseAsync(ulong wager, bool face = false)
            => await WagerCoinFlipAsync(Context.Account, wager, face);

        // for double or nothing.
        public async Task DoubleOrNothingAsync(OldAccount a, int times)
            => await DoubleOrNothingAsync(a, a.Balance, times);

        public async Task DoubleOrNothingAsync(OldAccount a, ulong wager, int times)
        {
            if (times <= 0)
            {
                string err = "You need to set the tick counter for the machine to land on.";
                await ReplyAsync(embed: EmbedData.Throw(Context, "Invalid tick counter.", err, false));
                return;
            }

            CasinoResult outcome = CasinoService.DoubleOrNothing(a, wager, times);
            await ReplyAsync(embed: outcome.Generate());
        }

        public string GoldenReply()
        {
            string[] s =
            {
                "Holy canola oil, where did this even come from?",
                "Calm before the storm.",
                "Hey, maybe you finally got out debt!",
                "Call me Midas, because I just dipped you in liquid gold!",
                "All that truly glitters is gold."

            };
            int x = RandomProvider.Instance.Next(1, s.Length + 1);
            return s[x - 1];
        }

        public string TakenReply(double d)
        {
            string[] s =
            {
                "Sorry, but I can't host myself for free.",
                "How else can I give out all this money?",
                "Maybe it's time to get a job?",
                "This audit shows you've been a big bad.",
                "While I giveth, I also taketh.",
                $"Account.Take({d})",
                "Experience oblivion.",
                "They can't all be winners."

            };
            int x = RandomProvider.Instance.Next(1, s.Length + 1);
            return s[x - 1];
        }

        public string DebtReply()
        {
            string[] s =
            {
                "Jeez, I can't even take money?",
                "Reap what you sow.",
                "Why do you keep trying?",
                "Mr. Pocket can't help you this time.",
                "Unable to compute negative funds.",
                "Please stop. You're hurting yourself.",
                "I can't be your only source of income.",
                "You need LifeAlert at the rate you fall at.",
                "Two negatives don't make a positive.",
                "ORS isn't pleased. I'm worried for you.",
                "It is drawing near...",
                "This isn't a sustainable method of income...",
                "It's fine. Feel free to pay me back later. :')"

            };
            int x = RandomProvider.Instance.Next(1, s.Length + 1);
            return s[x - 1];
        }

        public string GivenReply()
        {
            string[] s =
            {
                "You got lucky I found money to begin with.",
                "Alright, here. :)",
                "The sea has opened a new wave of funds.",
                "Whatever makes you happy, pal.",
                "A little bit of money can go far.",
                "While I taketh, I also giveth.",
                "My mercy prevails over my wrath.",
                "A bucko for my bucko.",
                "If all you wanted was a bill, just stop by a lake."

            };
            int x = RandomProvider.Instance.Next(1, s.Length + 1);
            return s[x - 1];
        }

        // for gimme command.
        public async Task GetOrTakeWalletAsync(OldAccount a)
        {
            EmbedBuilder e = new EmbedBuilder();
            Color give = EmbedData.GetColor("origreen");
            Color golden = EmbedData.GetColor("yield");
            Color take = EmbedData.GetColor("error");
            Color pity = EmbedData.GetColor("steamerror");
            int x = RandomProvider.Instance.Next(0, 1000 + 1);
            bool win = x >= 500;
            bool goldwin = x == 999;
            double reward = goldwin ? 999 : RandomProvider.Instance.Next(1, 10 + 1);
            string balance = $"{EmojiIndex.Balance}{reward.ToPlaceValue().MarkdownBold()}";
            string debt = $"{EmojiIndex.Debt}{reward.ToPlaceValue().MarkdownBold()}";
            string onGiveTitle = $"+ {balance}";
            string onTakeTitle = $"- {balance}";
            string onPityTitle = $"+ {debt}";
            if (!a.TracksGimme)
            {
                a.GimmeStats = new GiveOrTakeAnalyzer();
            }
            a.GimmeStats.Track(win, goldwin, (long)reward);
            StringBuilder f = new StringBuilder();
            f.Append($"{EmojiIndex.TellerMachine} {x}.{reward} | ");
            if (win)
            {
                if (goldwin)
                {
                    e.WithColor(golden);
                    e.WithTitle(onGiveTitle);
                    e.WithDescription(GoldenReply());
                    f.Append($"{a.GimmeStats.GoldenCount.ToPositionValue()} Golden");
                }
                else
                {
                    e.WithColor(give);
                    e.WithTitle(onGiveTitle);
                    e.WithDescription(GivenReply());
                    f.Append($"{(a.GimmeStats.WinStreak > 1 ? $"Wx{a.GimmeStats.WinStreak.ToPlaceValue()}" : $"{a.GimmeStats.WinCount.ToPlaceValue()} Win{(a.GimmeStats.WinCount > 1 ? "s" : "")}")}");
                }

                a.Give((ulong)reward);
            }
            else
            {
                if (a.Balance - reward < 0)
                {
                    e.WithColor(pity);
                    e.WithTitle(onPityTitle);
                    e.WithDescription(DebtReply());
                    a.Fine((ulong)reward);
                }
                else
                {
                    e.WithColor(take);
                    e.WithTitle(onTakeTitle);
                    e.WithDescription(TakenReply(reward));
                    a.Take((ulong)reward);
                }

                f.Append($"{(a.GimmeStats.LossStreak > 1 ? $"Lx{a.GimmeStats.LossStreak.ToPlaceValue()}" : $"{a.GimmeStats.LossCount.ToPlaceValue()} Loss{(a.GimmeStats.LossCount > 1 ? "es" : "")}")}");
            }

            f.Append($" | Give or Take");
            if (a.Config.VerboseGimme) e.WithFooter(f.ToString());
            await ReplyAsync(embed: e.Build());
        }

        public async Task WagerDiceRollRangeAsync(OldAccount a, int wager, int midpoint, bool dir = false)
            => await WagerDiceRollRangeAsync(a, wager, 6, midpoint, dir);

        public async Task WagerDiceRollRangeAsync(OldAccount a, int wager, int size, int midpoint, bool dir = false)
        {
            Dice d = new Dice(size);
            int min = dir ? 1 : 2;
            int max = dir ? (size - 1) : size;

            // exception catchers.
            if (!midpoint.IsInRange(min, max))
            {
                EmbedBuilder e = new EmbedBuilder();
                e.WithColor(EmbedData.GetColor("error"));

                if (!a.Config.Overflow)
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "Midpoint out of range.", $"The midpoint must be inside the range of {min} to {max}.", false));
                    return;
                }
                midpoint = midpoint.InRange(min, max);
            }

            CasinoResult outcome = CasinoService.BetRangedRoll(a, d, wager, midpoint, dir);
            await ReplyAsync(embed: outcome.Generate());
        }

        // bet on a six-sided dice
        // choose the sides that you think it will land on.
        public async Task WagerDiceRollAsync(OldAccount a, int wager, params int[] clear)
            => await WagerDiceRollAsync(a, wager, 6, clear);

        // bet on a dice roll with n amount of sides
        // choose the sides that you think it will land on.
        public async Task WagerDiceRollAsync(OldAccount a, int wager, int size, params int[] clear)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("error"));

            Dice d = new Dice(size);

            string err = "";

            if (clear.Length == 0)
            {
                err = "You need to at least place one landing point.";
                await ReplyAsync(embed: EmbedData.Throw(Context, "Empty landing points.", err, false));
                return;
            }

            if (clear.Length > (d.Sides - 1))
            {
                err = $"You can only place up to {d.Sides - 1} landing points.";
                await ReplyAsync(embed: EmbedData.Throw(Context, "Max landing points hit.", err, false));
                return;
            }

            List<int> called = new List<int>();
            foreach (int safe in clear)
            {
                if (safe > d.Sides)
                {
                    err = $"You can't place a landing point higher than {d.Sides}.";
                    await ReplyAsync(embed: EmbedData.Throw(Context, "Landing point out of range.", err, false));
                    return;
                }
                if (safe.EqualsAny(called))
                {
                    err = "You cannot place a landing point on a side twice.";
                    await ReplyAsync(embed: EmbedData.Throw(Context, "Duplicate landing point.", err, false));
                    return;
                }
                else
                    called.Add(safe);
            }

            CasinoResult outcome = CasinoService.BetSelectiveRoll(a, d, wager, clear);
            await ReplyAsync(embed: outcome.Generate());
        }

        public async Task WagerCoinFlipAsync(OldAccount a, ulong wager, bool face = false)
        {
            CasinoResult outcome = CasinoService.BetCoinFlip(a, wager);
            await ReplyAsync(embed: outcome.Generate());
        }

        // dice rolls in this range are based on the number of sides
        // and the midpoint they placed.
        // it also depends if they went above or under.

        /*
         example:

        6-sided dice, with a midpoint of 2, going under.

        6 sides, and the random number has to go under 2. (it cannot equal the midpoint)
        this means that the chance of the number being rolled under two is a 1/6 chance

        6-sided diced, with a midpoint of 1, going above.

        6 sides, and the random number has to go above 1. (it cannot equal the midpoint)
        this means that the chance of the number being rolled above one is a 5/6 chance.

        losing sides is the sides of a losing bet
        in ex2, where a 5/6 chance to win is shown
        the safety of this bet is 82.5%

        the raw safety is n /100 => 0.825.


        equation formula for getting risk multiplier
        risk = sides / rate of success

         */
    }
}