using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // Since Messy is a testing module, this doesn't need services.
    [Name("Messy")]
    [Summary("Commands that are under the works. Functionality is not to be expected.")]
    public class MessyModule : OriModuleBase<OriCommandContext>
    {
        private readonly GameManager _gameManager;
        public MessyModule(GameManager manager)
        {
            _gameManager = manager;
        }

        [RequireUserAccount]
        [Command("games")]
        [Summary("Returns a list of all visible **Games**.")]
        public async Task ShowLobbiesAsync([Summary("The page index for the list.")]int page = 1) // utilize a paginator.
            => await Context.Channel.SendMessageAsync(_gameManager.IsEmpty ? $"> **Looks like there's nothing here.**" : string.Join('\n', _gameManager.Games.Values.Select(x => x.ToString())));

        [RequireUserAccount]
        [Command("joingame"), Alias("jg")]
        [Summary("Join an open **Lobby**.")]
        public async Task JoinLobbyAsync([Summary("A string pointing to a specific **Game**.")]string id)
        {
            Game game = _gameManager[id];
            if (game == null)
                await Context.Channel.SendMessageAsync(_gameManager.ContainsUser(Context.User.Id) ?
                    "> **Wait a minute...**\n> You are already in a game." : $"**No luck.**\n> I couldn't find any games matching #**{id}**.");
            else
            {
                if (game.ContainsUser(Context.User.Id))
                    await Context.Channel.SendMessageAsync($"**???**\n> You are already in this game.");
                else
                {
                    await _gameManager.AddUserAsync(Context, id);
                    await Context.Channel.SendMessageAsync($"**Success!**\n> You have joined {game.Lobby.Name}. [{game.Receivers.First(x => x.Id == Context.Guild.Id).Mention}]");
                }
            }
        }

        [Command("whispertest")]
        public async Task WhisperTestAsync()
        {
            await WhisperAsync(".....hi.");
            await ReplyAsync("Psst... I sent you something");
        }

        [Command("roletoggletest")]
        public async Task RemoveRoleTestAsync()
        {
            ulong roleId = 614686327019012098;
            IGuildUser user = Context.Guild.GetUser(Context.User.Id);
            IRole role = Context.Guild.GetRole(roleId);

            string message = "You shouldn't see this.";

            if (user.RoleIds.Contains(roleId))
            {
                await user.RemoveRoleAsync(role);
                message = "Removed role.";
            }
            else
            {
                await user.AddRoleAsync(role);
                message = "Added role.";
            }

            await Context.Channel.SendMessageAsync(message);

        }

        [Command("presettest")]
        public async Task PresetTestAsync(bool useEmbed = false, bool hideUrl = false)
        {
            MessageBuilder msg = new MessageBuilder();
            msg.Content = "This is a message with content inside.";
            msg.Url = "https://steamcdn-a.akamaihd.net/steam/apps/730/header.jpg";
            msg.HideUrl = hideUrl;
            if (useEmbed)
                msg.Embedder = Embedder.Default;

            await Context.Channel.SendMessageAsync(msg.Build());
        }

        [Command("creategame"), Alias("crg")]
        [Summary("Create a **Game**.")]
        [RequireUserAccount]
        public async Task StartLobbyAsync([Summary("The **GameMode** to play within the **Game**.")]GameMode mode)
        {
            if (_gameManager.ContainsUser(Context.Account.Id))
            {
                await Context.Channel.SendMessageAsync($"> **Wait a minute...**\n> You are already in a game.");
                return;
            }
            try
            {
                Game game = await _gameManager.CreateGameAsync(Context, new GameConfig(mode, $"{Context.User.Username}'s Lobby")).ConfigureAwait(false);
                await Context.Channel.SendMessageAsync($"**Success!**\n> {game.Lobby.Name} has been created. [{game.Receivers[0].Mention}]");
                await _gameManager.StartGameSessionAsync(game.Id);
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("colortest")]
        [Summary("New color object testing.")]
        public async Task ColorAsync()
        {
            Color c = new Color(100, 100, 100);
            OriColor oriC = (OriColor)c;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"```bf");
            sb.AppendLine($"Discord.Color.RawValue == {c.RawValue}");
            sb.AppendLine($"OriColor.Value == {oriC.Value}");
            sb.AppendLine($"Discord.Color.R == {c.R}\nDiscord.Color.G == {c.G}\nDiscord.Color.B == {c.B}");
            sb.AppendLine($"OriColor.A == {oriC.A}\nOriColor.R == {oriC.R}\nOriColor.G == {oriC.G}\nOriColor.B == {oriC.B}");
            sb.AppendLine($"Discord.Color.ToString == {c.ToString()}");
            sb.AppendLine($"OriColor.ToString == {oriC.ToString()}");
            sb.AppendLine($"```");

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("windowtest")]
        public async Task WindowTestAsync([Remainder]string message = null)
        {
            message = Checks.NotNull(message) ? message : "This is a generic message.";
            try
            {
                GameWindow window = new GameWindow(GameWindowProperties.Lobby);
                window.CurrentTab.GetElement("element.info").Update(StringFormatter.LobbyInfoFormatter.WithArgs("New Lobby", KeyBuilder.Generate(8), GamePrivacy.Local.ToString(), GameMode.Werewolf.ToString()).ToString());
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs("Orikivo", "Message 1.").ToString(), "message-0"));
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs(Context.User.Username, message).ToString(), "message-1"));
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs(Context.User.Username, message).ToString(), "message-2"));
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs(Context.User.Username, message).ToString(), "message-3"));
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs(Context.User.Username, message).ToString(), "message-4"));
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs(Context.User.Username, message).ToString(), "message-5"));
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs(Context.User.Username, message).ToString(), "message-6"));
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs("Orikivo", "Message 8.").ToString(), "message-7"));
                window.CurrentTab.AddToGroup("elements.console",
                    new Element(StringFormatter.ConsoleLineFormatter.WithArgs("Orikivo", "Message 9.").ToString(), "message-8"));
                window.CurrentTab.GetElement("element.user_info").Update(StringFormatter.UserTitleFormatter.WithArgs("1", "15").ToString());
                window.CurrentTab.AddToGroup("elements.triggers", new Element(new GameTrigger("start").ToString()));
                window.CurrentTab.AddToGroup("elements.users", new Element(new User(Context.User.Id, Context.User.Username, Context.Guild.Id, UserTag.Host | UserTag.Playing).ToString()));
                window.CurrentTab.AddToGroup("elements.users", new Element(new User(Context.Client.CurrentUser.Id, Context.Client.CurrentUser.Username, Context.Guild.Id, UserTag.Playing).ToString()));

                await Context.Channel.SendMessageAsync(window.Content);
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("triggertest")]
        public async Task TriggerTestAsync([Remainder]string message)
        {
            List<Player> players = new List<Player>();

            List<GameAttribute> playerAttributes = new List<GameAttribute>();
            Player player = new Player(Context.User.Id, Context.User.Username, playerAttributes);

            players.Add(player);

            List<GameAttribute> rootAttributes = new List<GameAttribute>();
            List<GameTrigger> rootTriggers = new List<GameTrigger>();
            GameData rootData = new GameData(players, rootAttributes, rootTriggers);

            List<GameAttribute> taskAttributes = new List<GameAttribute>();
            List<GameTrigger> taskTriggers = new List<GameTrigger>();

            List<GameArgValue> values = new List<GameArgValue>();

            
            List<GameCriterion> valueCriteria = new List<GameCriterion>();
            List<GameUpdatePacket> onValueParseSuccess = new List<GameUpdatePacket>();
            GameArgValue value = new GameArgValue("world", onValueParseSuccess, valueCriteria);
            values.Add(value);

            // trigger1
            List<GameCriterion> argCriteria = new List<GameCriterion>();
            List<GameUpdatePacket> onArgParseSuccess = new List<GameUpdatePacket>();
            GameObject defaultArgValue = new GameObject(GameObjectType.String, "world");

            GameArg arg = new GameArg("message", values, argCriteria, onArgParseSuccess);

            List<GameCriterion> triggerCriteria = new List<GameCriterion>();
            List<GameUpdatePacket> onTriggerParseSuccess = new List<GameUpdatePacket>();
            GameTrigger trigger = new GameTrigger("test", arg, triggerCriteria, onTriggerParseSuccess);

            // trigger2
            GameArg arg2 = new GameArg("messages", GameObjectType.String, isArray: true);
            GameTrigger trigger2 = new GameTrigger("testmany", arg2);

            //trigger3
            GameArg arg3 = new GameArg("user", GameObjectType.User);
            GameTrigger trigger3 = new GameTrigger("testuser", arg3);


            taskTriggers.Add(trigger);
            taskTriggers.Add(trigger2);
            taskTriggers.Add(trigger3);

            GameTaskData taskData = new GameTaskData(rootData.TaskId, taskAttributes, taskTriggers);
            taskData.Root = rootData;

            TaskTriggerContext context = new TaskTriggerContext(taskData, player, message);

            for(int i = 0; i < taskData.Triggers.Count; i++)
            {
                taskData.Triggers[i].TryParse(context, out GameTriggerResult result);
                if (result.Error == TriggerParseError.InvalidTrigger && i + 1 < taskData.Triggers.Count)
                    continue;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"IsSuccess: {result.IsSuccess}");
                sb.AppendLine($"Context: \"{message}\"");
                sb.AppendLine($"Error: {result.Error}");
                sb.AppendLine($"ErrorReason: {result.ErrorReason}");
                sb.AppendLine($"TriggerId: {result.TriggerId}");
                sb.AppendLine($"ArgId: {result.ArgId}");
                string objects = string.Join("\n\n", result.Objects.Select(x => $"GameObject =>\nType: {x.Type}\nValue: {x.Value}\nIsArray: {x.IsArray}"));
                string packets = string.Join("\n\n", result.Packets.Select(x =>
                {
                    StringBuilder gb = new StringBuilder();
                    gb.AppendLine("GameUpdatePacket =>");
                    gb.AppendLine(string.Join("\n\n", x.AttributePackets
                    .Select(y =>
                    {
                        StringBuilder ab = new StringBuilder();
                        ab.AppendLine($"AttributeUpdatePacket =>");
                        ab.AppendLine($"Id: {y.Id}");
                        ab.AppendLine($"Method: {y.Method}");
                        ab.AppendLine($"Amount: {y.Amount}");
                        return ab.ToString();
                    }).Concat(x.WindowPackets.Select(y =>
                    {
                        StringBuilder wb = new StringBuilder();
                        wb.AppendLine($"WindowUpdatePacket =>");
                        wb.AppendLine($"WindowId: {y.WindowId}");
                        wb.AppendLine($"Output: {y.Output}");
                        wb.AppendLine($"ToTabId: {y.ToTabId}");
                        wb.AppendLine(string.Join("\n\n", y.Packets.Select(z =>
                        {
                            StringBuilder tb = new StringBuilder();
                            tb.AppendLine("TabUpdatePacket =>");
                            tb.AppendLine($"TabId: {z.TabId}");
                            tb.AppendLine(string.Join("\n\n", z.Packets.Select(a =>
                            {
                                StringBuilder eb = new StringBuilder();
                                eb.AppendLine($"ElementUpdatePacket =>");
                                eb.AppendLine($"ElementId: {a.ElementId}");
                                eb.AppendLine($"GroupId: {a.GroupId}");
                                eb.AppendLine($"Index: {a.Index}");
                                eb.AppendLine($"ElementUpdateMethod: {a.Method}");
                                eb.AppendLine($"Element: {a.Element?.ToString()}");
                                return eb.ToString();
                            })));
                            return tb.ToString();
                        })));
                        return wb.ToString();
                    }))));
                    return gb.ToString();
                }));
                sb.AppendLine(objects);
                sb.AppendLine(packets);

                await Context.Channel.SendMessageAsync(sb.ToString());
                return;
            }
        }

        [Command("shuffletest")]
        public async Task ShuffleTestAsync()
        {
            List<int> values = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Old: {{ {values.WriteValues()} }}");
            List<int> shuffledValues = OriRandom.Shuffle(values).ToList();
            sb.AppendLine($"OldEnsure: {{ {values.WriteValues()} }}");
            sb.AppendLine($"New: {{ {shuffledValues.WriteValues()} }}");
            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("dicetest")]
        public async Task DiceTestAsync()
        {
            Dice dice = Dice.Default;
            int result = OriRandom.Roll(dice);
            await Context.Channel.SendMessageAsync(result.ToString());
        }
    }
}
