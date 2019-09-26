using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Orikivo.Dynamic;
using Orikivo.Helpers;
using Orikivo.Providers;
using Orikivo.Static;
using Orikivo.Systems.Presets;
using Orikivo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SysColor = System.Drawing.Color;


namespace Orikivo.Modules
{
    [Name("Utility")]
    [Summary("Provides a range of helpers.")]
    [DontAutoLoad]
    public class UtilityModule : ModuleBase<OrikivoCommandContext>
    {
        [Name("Random")]
        [Summary("Utilize a range of tools that provide random data.")]
        public class RandomModule : ModuleBase<OrikivoCommandContext>
        {
            private readonly CommandService _service;
            public RandomModule(CommandService service)
            {
                _service = service;
            }

            [Command("randompin"), Alias("rpin")]
            [Summary("Randomly get a pinned message from a channel.")]
            public async Task GetPinAsync()
            {
                if (!Context.Channel.HasPins(out IReadOnlyCollection<RestMessage> pins))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "This channel has no pinned messages."));
                    return;
                }

                RestMessage pin = pins.ToList()[RandomProvider.Instance.Next(1, pins.Count) - 1];
                await ModuleManager.TryExecute(Context.Channel, Context.Channel.SendRestMessageAsync(pin));
            }

            [Command("command"), Alias("cmd")]
            [Summary("Randomly get a usable command.")]
            public async Task RandomCommandResponseAsync()
                => await GetRandomCommandAsync();

            [Command("color"), Alias("col")]
            [Summary("Get a random color.")]
            public async Task RandomColorResponseAsync()
                => await GetRandomColorAsync();

            [Command("anynumber"), Alias("anynum")]
            [Summary("Randomly get a full range integer.")]
            public async Task AnyNumberResponseAsync()
                => await GetAnyRandomNumberAsync();

            [Command("number"), Alias("num"), Priority(2)]
            [Summary("Randomly get an integer.")]
            public async Task NumberResponseAsync()
                => await GetRandomNumberAsync();

            [Command("number"), Alias("num"), Priority(1)]
            [Summary("Randomly get an integer, with an upper limit.")]
            public async Task NumberResponseAsync(int max)
                => await GetRandomNumberAsync(max);

            [Command("number"), Alias("num"), Priority(0)]
            [Summary("Randomly get an integer within two numbers.")]
            public async Task NumberResponseAsync(int min, int max)
                => await GetRandomNumberAsync(min, max);

            [Command("roll"), Priority(2)]
            [Summary("Roll a simple six-sided dice.")]
            public async Task DiceResponseAsync()
                => await GetRandomDiceRollAsync();

            [Command("roll"), Priority(1)]
            [Summary("Roll a dice with n number of sides.")]
            public async Task DiceResponseAsync(int sides)
                => await GetRandomDiceRollAsync(new Dice(sides));

            [Command("roll"), Priority(0)]
            [Summary("Roll a dice with n number of sides.")]
            public async Task DiceResponseAsync(int sides, int times)
                => await GetRandomDiceRollAsync(new Dice(sides), times);

            [Command("flip")]
            [Summary("Flip a coin.")]
            public async Task CoinResponseAsync()
                => await GetRandomCoinFlipAsync();

            [Command("flip")]
            [Summary("Flip a coin n amount of times.")]
            public async Task CoinResponseAsync(int times)
                => await GetRandomCoinFlipAsync(times.InRange(1, 100));

            public async Task GetRandomCommandAsync()
            {
                IEnumerable<ModuleInfo> modules = Context.Data.Modules.Where(x => x.IsActive(_service.Modules));
                List<IReadOnlyList<CommandInfo>> collection = new List<IReadOnlyList<CommandInfo>>();
                foreach (ModuleInfo m in modules)
                {
                    collection.Add(m.Commands);
                }
                List<CommandInfo> commands = new List<CommandInfo>();
                foreach (IReadOnlyList<CommandInfo> coll in collection)
                {
                    commands.Merge(coll.ToList());
                }

                int n = RandomProvider.Instance.Next(1, collection.Count()) - 1;
                string info = $"{Context.Server.Config.Prefix ?? Context.Client.CurrentUser.Mention + " "}{commands[n].Name}";
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithColor(EmbedData.GetColor("owo"));
                e.WithFooter($"{info} | Random Command");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomNumberAsync()
            {
                int n = RandomProvider.Instance.Next();
                string message = n.ToPlaceValue();
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithFooter($"{EmojiIndex.Counter} {message} | Random Number | x");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomColorAsync()
            {
                int r = RandomProvider.Instance.Next(0, 255);
                int g = RandomProvider.Instance.Next(0, 255);
                int b = RandomProvider.Instance.Next(0, 255);
                SysColor col = SysColor.FromArgb(r, g, b);
                string info = $"{EmojiIndex.Hex} {col.GetHexCode()}";

                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithColor(col.ToDiscordColor());
                e.WithFooter($"{info} | Random Color");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetAnyRandomNumberAsync()
            {
                int n = RandomProvider.Instance.Next(int.MinValue, int.MaxValue);
                string message = n.ToPlaceValue();
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithFooter($"{EmojiIndex.Counter} {message} | Random Number | - ≤ x ≤ +");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomNumberAsync(int max)
            {
                int min = 0;
                //max = max.InRange(1, int.MaxValue);

                string minstr = "";
                string maxstr = $" ≤ {max.ToPlaceValue()}";

                if (max < min)
                {
                    min = max;
                    minstr = $"{min.ToPlaceValue()} ≤ ";
                    max = 0;
                    maxstr = "";
                }
                int n = RandomProvider.Instance.Next(min, max);
                string message = n.ToPlaceValue();
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithFooter($"{EmojiIndex.Counter} {message} | Random Number | {minstr}x{maxstr}");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomNumberAsync(int min, int max)
            {
                int tmp = max > min ? max : min;

                min = min > max ? max : min;
                max = tmp;
                max = max.InRange(int.MinValue, int.MaxValue);

                int n = RandomProvider.Instance.Next(min, max);
                string message = n.ToPlaceValue();
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithFooter($"{EmojiIndex.Counter} {message} | Random Number | {min.ToPlaceValue()} ≤ x ≤ {max.ToPlaceValue()}");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomDiceRollAsync()
            {
                int side = RandomProvider.Instance.Roll();
                string message = side.ToPlaceValue();
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithFooter($"{EmojiIndex.Dice}{message} | Dice Roll | D6");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomDiceRollAsync(Dice d)
            {
                int side = RandomProvider.Instance.Roll(d);
                string message = side.ToPlaceValue();
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithFooter($"{EmojiIndex.Dice}{message} | Dice Roll | D{d.Sides}");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomDiceRollAsync(Dice d, int times)
            {
                times = times.InRange(1, 100);

                if (times == 1)
                {
                    await GetRandomDiceRollAsync(d);
                    return;
                }

                List<int> rolls = RandomProvider.Instance.RollMany(d, times);
                IGrouping<int, int> top = rolls.GroupBy(x => x).OrderByDescending(x => x.Count()).First();
                int toptimes = top.Count();
                int roll = top.Key;

                if (toptimes == 1)
                {
                    roll = rolls.OrderByDescending(x => x).First();
                }

                string info = $"{roll}{(toptimes == 1 ? "" : $" (x{toptimes})")}";
                StringBuilder sb = new StringBuilder();
                foreach (int r in rolls)
                {
                    sb.Append($"[{r}] ");
                }
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithDescription(sb.ToString().TrimEnd(' '));
                e.WithFooter($"{EmojiIndex.Dice}{info} | Dice Roll | {times}D{d.Sides}");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomCoinFlipAsync()
            {
                bool flip = RandomProvider.Instance.Flip();
                //string message = flip.AsCoinFlip();
                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithFooter($"{(flip ? EmojiIndex.Coin : EmojiIndex.Coin)}{flip.AsCoinFlip()} | Coin Flip");
                await ReplyAsync(embed: e.Build());
            }

            public async Task GetRandomCoinFlipAsync(int times)
            {
                if (times == 1)
                {
                    await GetRandomCoinFlipAsync();
                    return;
                }

                StringBuilder sb = new StringBuilder();
                List<bool> flips = new List<bool>();
                for (int i = 0; i < times; i++)
                {
                    bool flip = RandomProvider.Instance.Flip();
                    sb.Append($"{flip.AsCoinFlip()} "); // .ToSuperscript()
                    flips.Add(flip);
                }

                int heads = flips.Where(x => x).Count();
                int tails = flips.Where(x => !x).Count();

                EmbedBuilder e = EmbedData.DefaultEmbed;
                e.WithDescription(sb.ToString().TrimEnd(' '));
                e.WithFooter($"{EmojiIndex.Coin}{heads}H | Coin Flip | {EmojiIndex.Coin}{tails}T"); //$"{EmojiIndex.Coin}{heads}H | {EmojiIndex.Coin}{tails}T | Coin Flip"
                await ReplyAsync(embed: e.Build());
            }
        }

        [Name("Math")]
        [Summary("Offers a range of basic equation solving methods.")]
        public class MathModule : ModuleBase<OrikivoCommandContext>
        {
            public MathModule()
            {

            }

            [Command("sortm")]
            [Summary("Sorts an equation into an organized index.")]
            public async Task SortEquationAsync([Remainder]string equ) =>
            await ModuleManager.TryExecute(Context.Channel, SortMathAsync(equ));

            public async Task SortMathAsync(string equ)
            {
                Equation e = new Equation(equ);
                await ReplyAsync(e.Summarize().DiscordBlock());
            }

            [Command("derive")]
            [Summary("Returns a summary of the derivative of the base, x-value, and exponent.")]
            public async Task DeriveAsync(double scalar, double value, double exponent)
            {
                OldCalculator.Derive(scalar, value, exponent, out double b, out double x, out double n, out double r);
                string s =
                $"derivative_base | (b·n){VariableIndex.X}{"n-1".ToSuperscript()}\n" +
                $"base | {scalar}·{exponent} | {b.ToPlaceValue()}\n" +
                $"exponent |  {exponent}-1 | {n.ToPlaceValue()}\n" +
                $"result | {b}·{x}{n.ToPlaceValue().ToSuperscript()} | {r.ToPlaceValue()}";
                await ReplyAsync(s.DiscordBlock());
            }

            [Command("integrate")]
            [Summary("Returns a summary of the integral of the base, x-value, and exponent.")]
            public async Task IntegrateAsync(double scalar, double value, double exponent)
            {
                OldCalculator.Integrate(scalar, value, exponent, out double b, out double x, out double n, out double r);
                string s =
                $"integral_base | b{VariableIndex.X}{"n+1".ToSuperscript()}/n+1\n" +
                $"exponent | {exponent}+1 | {n}\n" +
                $"base | {b}\n" +
                $"result | {b}·{x}{n.ToPlaceValue().ToSuperscript()}/{n} | {r.ToPlaceValue()}";
                await ReplyAsync(s.DiscordBlock());
            }

            [Command("add")]
            [Summary("Adds two numbers together.")]
            public async Task AddAsync(double int1, double int2)
            {
                await ReplyAsync($"{int1 + int2}");
            }

            [Command("subtract")]
            [Summary("Subtracts two numbers together.")]
            public async Task SubtractAsync(double int1, double int2)
            {
                await ReplyAsync($"{int1 - int2}");
            }

            [Command("multiply")]
            [Summary("Multiply two numbers together.")]
            public async Task MultiplyAsync(double int1, double int2)
            {
                await ReplyAsync($"{int1 * int2}");
            }

            [Command("divide")]
            [Summary("Divides two numbers together.")]
            public async Task DivideAsync(double int1, double int2)
            {
                await ReplyAsync($"{int1 / int2}");
            }

            [Command("squareroot"), Alias("sqrt")]
            [Summary("Returns a number in a squate root.")]
            public async Task SquareRootAsync(double int1)
            {
                await ReplyAsync($"{Math.Sqrt(int1)}");
            }

            [Command("cuberoot"), Alias("cbrt")]
            [Summary("Returns a number in a cubic root.")]
            public async Task CubeRootAsync(double int1)
            {
                await ReplyAsync($"{Math.Cbrt(int1)}");
            }

            [Command("power"), Alias("pow")]
            [Summary("Returns a number to the power of another number.")]
            public async Task ExponentAsync(double int1, double int2)
            {
                await ReplyAsync($"{Math.Pow(int1, int2)}");
            }
        }

        //Utility.
        [Name("StringBuilder")]
        [Summary("Provides builders for customized strings.")]
        public class StringBuilderModule : ModuleBase<OrikivoCommandContext>
        {
            public StringBuilderModule()
            {

            }

            #region StringBuilder#Tasks
            public async Task ReverseStringAsync(string content)
            {
                string result = content.Reverse();
                await ReplyAsync(result);
            }

            public string ReplaceAll(string content, string sub, params string[] matches)
            {
                string s = content; // Hello.
                                    // Hi.
                                    // e,o


                return s;           // HHillHi.
            }

            public async Task ReplaceSubstringsAsync(string content, string[] substr, string str)
            {
                content = content.ReplaceMany(str, substr);

                await ReplyAsync(content);
            }

            public async Task SortStringAsync(string content)
            {
                string result = new string(content.SortCharacters());
                await ReplyAsync(result);
            }

            public async Task FlipStringCasingAsync(string content)
            {
                string result = content.FlipCasing();
                await ReplyAsync(result);
            }

            public async Task AlternateStringCasingAsync(string content)
            {
                string result = content.AlternateCasing();
                await ReplyAsync(result);

            }

            public async Task SpanStringAsync(string content, int length)
            {
                string result = content.Span(length);
                await ReplyAsync(result);
            }

            public async Task DragStringAsync(string content, int length)
            {
                string result = content.Drag(length);

                await ReplyAsync(result);
            }

            public async Task SubStringAsync(string content)
            {
                string result = content.ToSubscript();

                await ReplyAsync(result);
            }

            public async Task SuperStringAsync(string content)
            {
                string result = content.ToSuperscript();

                await ReplyAsync(result);
            }

            public async Task BoldStringAsync(string content)
            {
                string result = content.ToBold();

                await ReplyAsync(result);
            }

            public async Task ItalicizeStringAsync(string content)
            {
                string result = content.ToItalics();

                await ReplyAsync(result);
            }

            public async Task EmojifyLettersAsync(string content, bool size = true)
            {
                if (!content.Exists())
                {
                    //Invalid character.
                    await ReplyAsync(embed: EmbedData.Throw(Context, "Empty string.", "Only the following characters are rendered: A-Z a-z 0-9 #!?", false));
                    return;
                }

                string result = content.Emojify(size);

                await ReplyAsync(result);
            }
            #endregion

            public bool TryGetReplacingString(string definer, out string[] substr, out string str)
            {
                substr = null;
                str = null;
                const int LENGTH = 2;
                string split = "|";
                string subsplit = ",";

                string[] strings = definer.Split(split);
                if (strings.Length != LENGTH)
                {
                    return false;
                }

                substr = strings[0].Split(subsplit);
                str = strings[1];
                return true;
            }

            public async Task RegexAsync(string pattern, string content)
            {
                Regex r = new Regex(pattern);
                Match m = r.Match(content);
                if (m.Success)
                {
                    string v = m.Value;
                    EmbedBuilder e = EmbedData.DefaultEmbed;
                    EmbedFooterBuilder f = new EmbedFooterBuilder();
                    e.WithFooter(f);
                    f.WithText($"Regex | @\"{pattern}\" | \"{content}\"");

                    // style1 - content.Replace(v, v.MarkdownBold())
                    // styl2 - $"Found: {v}\nFrom: {content}".MarkdownBlock()
                    // style3 - $"{v} | {content}"
                    // style4 - $"{content} => {v}"
                    e.WithDescription($"{v}");

                    await ReplyAsync(embed: e.Build());
                    return;
                }
                await ReplyAsync(embed: EmbedData.Throw(Context, "No matches were found."));
            }

            public string GetHexadecimal(string s)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in s)
                {
                    sb.Append($"{GetHexadecimal(c)}"); // \\x
                }
                return sb.ToString();
            }

            public string GetHexadecimal(char c)
            {
                UnicodeEncoding encoder =new UnicodeEncoding();
                /*byte[] i = Encoding.UTF32.GetBytes($"{c}");
                StringBuilder sb = new StringBuilder();
                foreach(byte b in i)
                {
                    sb.Append($"\\u{b:X}");
                }*/
                int i = c;
                string hex = $"{i:X}";
                return $"{(hex.Length > 2 ? "\\u": "\\x")}{hex}";//sb.ToString();
            }

            [Command("length"), Alias("len", "size")]
            [Summary("Returns the length of a string.")]
            public async Task LengthResponseAsync([Remainder]string content)
            {
                await ModuleManager.TryExecute(Context.Channel, LengthAsync(content));
            }

            public async Task LengthAsync(string content)
            {
                EmbedBuilder eb = Embedder.DefaultEmbed;
                eb.WithTitle(Format.Bold(content.Length.ToPlaceValue()));
                await Context.Channel.SendEmbedAsync(eb.Build());
            }

            [Command("regex")]
            [Summary("Utilize a regex pattern on a string, and return all matches.")]
            public async Task RegexResponseAsync(string pattern, [Remainder]string content)
            {
                // Make TryExecute(Context.Channel, () => { // Any async task goes here. }); work.
                await ModuleManager.TryExecute(Context.Channel, RegexAsync(pattern, content));
            }

            [Command("escape"), Alias("encode", "esc")]
            [Summary("Returns a string in which all character values are escaped.")]
            public async Task EscapeStringResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, ReplyAsync(GetHexadecimal(content)));

            [Command("unescape"), Alias("decode", "unesc")]
            [Summary("Returns a string in which all metacharacter values are escaped.")]
            public async Task UnscapeStringResponseAsync([Remainder]string content)
            {
                try
                {
                   
                    content = Regex.Unescape(content);
                }
                catch (ArgumentException e)
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "Parsing error.",
                        $"The string specified failed to properly escape.{(e.Message.Exists() ? $"\n{e.Message.DiscordBlock()}" : "")}", false));
                    return;
                }
                await ReplyAsync(content);
            }

            [Command("sort")]
            [Summary("Sorts a string based on letter placed.")]
            public async Task SortStringResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, SortStringAsync(content));

            // Find a better replacement method.
            // Regex? replace word, word2, word3 | this is what replaces all of those.
            // to use commas, \, or to use straight lines, \| or to use a space \s.
            // or replace word | this || word2 | works || word3 | too
            // or replace word, word2 | this || word3 | is || word4 | nice.
            // || = Splits multiple replace parameters
            // , = used to list replacing words
            // | = used as replacing word parameter break.


            // INVALID USAGE
            // word | ok | cool. >a parameter break cannot be used more than once if one is open.
            // | um >a parameter break cannot be used if the given substring is null.
            // \ | ok > invalid escape sequence.
            [Command("replace")]
            [Summary("Returns a string in which the defining substring will be replaced with the defining replacement.")]
            public async Task ReplaceLetterResponseAsync(string definer, [Remainder]string content)
            {
                if (!TryGetReplacingString(definer, out string[] substr, out string str))
                {
                    await ReplyAsync(embed: EmbedData.Throw(Context, "You failed to parse the definer.", "replace substring(,)|string <content>"));
                    return;
                }

                await ReplaceSubstringsAsync(content, substr, str);
            }

            [Command("flipcase")]
            [Summary("Flips the casing of each character in a string.")]
            public async Task FlipCasingResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, FlipStringCasingAsync(content));

            [Command("altcase")]
            [Summary("Gets the string, and places it through a casing pattern alternator, based off of the first character.")]
            public async Task AlternateCasingResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, AlternateStringCasingAsync(content));

            [Command("reverse")]
            [Summary("Reverses the string.")]
            public async Task ReverseResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, ReverseStringAsync(content));

            [Command("span")]
            [Summary("Spaces out a string with a specified spacing length.")]
            public async Task SpanResponseAsync(int length, [Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, SpanStringAsync(content, length));
   
            [Command("drag")]
            [Summary("Stretchs all characters in the string by a specified amount.")]
            public async Task DragResponseAsync(int length, [Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, DragStringAsync(content, length));

            [Command("subscript"), Alias("sub")]
            [Summary("Converts a string into subscript Unicode on applicable text.")]
            public async Task SubResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, SubStringAsync(content));

            [Command("superscript"), Alias("super")]
            [Summary("Converts a string into superscript Unicode on applicable text.")]
            public async Task SuperResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, SuperStringAsync(content));

            [Command("bold")]
            [Summary("Converts a string into bold Unicode on applicable text.")]
            public async Task BoldResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, BoldStringAsync(content));

            [Command("italic"), Alias("ital")]
            [Summary("Converts a string into italic Unicode on applicable text.")]
            public async Task ItalicResponseAsync([Remainder]string content)
                => await ModuleManager.TryExecute(Context.Channel, ItalicizeStringAsync(content));

            [Command("emojify")]
            [Summary("Converts a string into Discord emoji text.")]
            public async Task EmojifyResponseAsync([Remainder]string content = null)
                => await ModuleManager.TryExecute(Context.Channel, EmojifyLettersAsync(content));
            
            [Command("emojifysmall")]
            [Summary("Converts a string into Discord emoji text as a forced default string size.")]
            public async Task EmojifySmallReponseAsync([Remainder]string content = null)
                => await ModuleManager.TryExecute(Context.Channel, EmojifyLettersAsync(content, false));
        }
    }
}