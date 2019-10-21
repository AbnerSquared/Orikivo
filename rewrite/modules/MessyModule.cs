using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
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

        [Command("windowtest")]
        public async Task WindowTestAsync([Remainder]string message = null)
        {
            message = Checks.NotNull(message) ? message : "This is a generic message.";
            try
            {
                GameWindow window = new GameWindow(GameWindowProperties.Lobby);
                window.CurrentTab.AddToGroup("elements.chat", new Element(message, "message-0"));
                window.CurrentTab.AddToGroup("elements.chat", new Element(message, "message-1"));
                window.CurrentTab.AddToGroup("elements.chat", new Element(message, "message-2"));
                window.CurrentTab.AddToGroup("elements.chat", new Element(message, "message-3"));
                window.CurrentTab.AddToGroup("elements.chat", new Element(message, "message-4"));
                window.CurrentTab.AddToGroup("elements.chat", new Element(message, "message-5"));
                window.CurrentTab.AddToGroup("elements.chat", new Element(message, "message-6"));
                window.CurrentTab.AddToGroup("elements.chat", new Element("Message eight.", "message-7"));
                window.CurrentTab.AddToGroup("elements.chat", new Element("AHHH", "message-8"));
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

            GameTriggerContext context = new GameTriggerContext(taskData, player, message);

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
    }
}
