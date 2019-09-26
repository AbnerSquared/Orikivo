using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{

    public class WerewolfGame
    {
        private Display _oldDisplay;
        private BaseSocketClient _rootClient;

        public WerewolfGame(BaseSocketClient client, Display display, List<Receiver> receivers, List<User> users)
        {
            _rootClient = client;
            _oldDisplay = display;
            Display = display;
            Display.Clear();

            Users = new List<WerewolfUser>();
            foreach (User user in users)
            {
                Users.Add(new WerewolfUser(user));
            }

        }

        private int _dayTicks; // each tick represents a new day
        private WerePhase _phase; // the current phase in the match.
        private List<WereAction> _currentActions; // actions that happen at night, cleared on a new day.
        private WereDay _currentDay; // the data of everything that happened on the current _dayTicks;

        public async Task StartAsync()
        {
            /*
            async Task<WereAction> TryGetAction(OriWerewolfUser user)
            {
                WerewolfRole role = WerewolfRole.FromRole(user.Role);
                if (role.Abilities != null)
                {
                    return null;
                }

                SocketUser _user = _rootClient.GetUser(user.Id);
                if (_user != null)
                {
                    IDMChannel dm = _user.GetOrCreateDMChannelAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    if (dm != null)
                    {
                        WereAbilityType ability = role.Abilities.First();
                        StringNodeGroup group = new StringNodeGroup
                        {
                            AllowFormatting = true,
                            Title = $"**Players**\nChoose a player to {ability.ToString().ToLower()}.",
                            TitleFrame = "{0}",
                            GroupSeparator = "\n",
                            GroupContentFrame = "{0}",
                            GroupValueFrame = "{0}"
                        };
                        foreach (OriWerewolfUser wolfUser in Users)
                        {
                            StringNode node = new StringNode();
                            string content = $"**{wolfUser.Name}**";
                            if (wolfUser.Dead)
                                content = string.Format("☠ ~~{0}~~", content);
                            if (user.IsWerewolf)
                            {
                                content += wolfUser.IsWerewolf ? "🐺" : "";
                            }
                            if (user.Role == WerewolfRoleType.Seer)
                            {
                                content += GetUserScanIcon(wolfUser);
                            }
                            node.Content = content;
                            group.Add(node);
                        }

                        async Task Listen(SocketMessage message)
                        {
                            if (message.Author.Id != user.Id)
                            {
                                return;
                            }
                            if (message.Channel != dm)
                            {
                                return;
                            }

                            IUserMessage userMsg = dm.SendMessageAsync(group.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
                        }


                        WereAction action = new WereAction();
                    }
                }
            }
            */
        }

        private string GetUserScanIcon(WerewolfUser user)
        {
            if (user.Scanned)
            {
                if (user.IsWerewolf)
                {
                    return "🔺";
                }
                return "🔹";
            }
            return "▫";
        }

        private StringNodeGroup _userList
        {
            get
            {
                StringNodeGroup group = new StringNodeGroup
                {
                    AllowFormatting = true,
                    Title = "Players",
                    ValueSeparator = "\n",
                    ContentMap = "{0}",
                    ValueMap = "{0}"
                };
                foreach (WerewolfUser user in Users)
                {
                    StringNode node = new StringNode();
                    string content = $"**{user.Name}**";
                    if (user.Dead)
                        content = string.Format("☠ ~~{0}~~", content);
                    node.Content = content;
                    group.AddNode(node);
                }
                return group;
            }
        }

        public Display Display { get; }
        public List<Receiver> Receivers { get; }
        public List<WerewolfUser> Users { get; } 
    }
}
