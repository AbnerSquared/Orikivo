using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Orikivo.Systems.Services
{
    public class StatusService {
        private ConcurrentDictionary<ulong, _statusData> _statusQuota = new ConcurrentDictionary<ulong, _statusData>();

        private JsonSerializer _JsonSerializer = new JsonSerializer();
        public StatusService() {
            LoadStatusQuota();
        }

        private async Task StatusCreate(OrikivoCommandContext Context, string statusTypeSelection, string expression, bool hasUpdated) {
            if (expression == null) {
                expression = "";
            }
            _statusData statusData = new _statusData {
                statusType = statusTypeSelection,
                expression = expression
            };
            _statusData ignore;
            _statusQuota.TryRemove(Context.User.Id, out ignore);
            _statusQuota.AddOrUpdate(Context.User.Id, statusData,((key, oldValue) => statusData));
            await Context.Channel.SendMessageAsync($"{(hasUpdated == true ? $"`You updated your status to {statusData.statusType}.`" : $"`You are now set as {statusData.statusType}.`")}");
        }
        

        public async Task SetStatus(OrikivoCommandContext Context, string statusType, string expression) {
            try {
                if (!_statusQuota.ContainsKey(Context.User.Id)) {
                    await StatusCreate(Context, statusType, expression, false);
                }
                else {
                    await StatusCreate(Context, statusType, expression, true);
                }
                SaveStatusQuota();
            }
            catch (Exception onError) {
                Console.WriteLine(onError);
            }
        }

        public async Task ClearStatus(OrikivoCommandContext Context)
        {
            try
            {
                _statusData ignore;
                _statusQuota.TryRemove(Context.User.Id, out ignore);
                await Context.Channel.SendMessageAsync($"`You are no longer set as {ignore.statusType}.`");
                SaveStatusQuota();
            }
            catch (Exception onError)
            {
                Console.WriteLine(onError);
            }
        }

        public async Task StatusRead(SocketUser User, OrikivoCommandContext Context) {
        int[] online = {112, 229, 130};
        int[] away = {255, 238, 129};
        int[] busy = {213, 16, 93};
        var embedAuthor = new EmbedAuthorBuilder() {
            IconUrl = User.GetAvatarUrl(),
            Name = User.Username
        };
            var descBoot = $"{User.Username} is";
            if (User == Context.Message.Author)
            {
                descBoot = "You are";
            }

            if (_statusQuota.ContainsKey(User.Id)){
            _statusData statusData = new _statusData();
            _statusQuota.TryGetValue(User.Id, out statusData);
            var expressionReturn = "";
            
            
                if (statusData.expression != "")
            {
                expressionReturn += $"\nLog: '{statusData.expression}";
            }

            

            
            

            
            if (statusData.statusType == "away")
            {
                var embedAway = new EmbedBuilder()
                {
                    Author = embedAuthor,
                    Color = new Color(away[0], away[1], away[2]),
                    Description = $"{descBoot} displayed as away.\nUser mentions will be deleted upon usage."
                                  + expressionReturn
                };
                    await Context.Channel.SendMessageAsync("", false, embedAway.Build());
            }
            else if (statusData.statusType == "busy")
            {
                var embedBusy = new EmbedBuilder()
                {
                    Author = embedAuthor,
                    Color = new Color(busy[0], busy[1], busy[2]),
                    Description = $"{descBoot} displayed as busy.\nUser mentions will be deleted upon usage and will be notified."
                                  + expressionReturn
                };
                    await Context.Channel.SendMessageAsync("", false, embedBusy.Build());
            }
        }
        else {
            var embedOnline = new EmbedBuilder() {
                Author = embedAuthor,
                Color = new Color(online[0], online[1], online[2]),
                Description = $"{descBoot} displayed as online."
            };
            await Context.Channel.SendMessageAsync("", false, embedOnline.Build());
        }
    }

    public async Task MessageRecieved(SocketMessage message) {
            if (message.MentionedUsers.Count < 1) {return;}
            if (message.Author.IsBot) {return;}
            foreach (var User in message.MentionedUsers) {
                
                if (_statusQuota.ContainsKey(User.Id)) {
                    _statusData statusData = new _statusData();
                    _statusQuota.TryGetValue(User.Id, out statusData);

                    var awayUser = new EmbedAuthorBuilder() {
                        IconUrl = (User.GetAvatarUrl()),
                        Name = $"{User.Username} is away. Please try again later."
                    };
                    var busyUser = new EmbedAuthorBuilder()
                    {
                        IconUrl = (User.GetAvatarUrl()),
                        Name = $"{User.Username} is busy. Refrain from mentioning this user."
                    };
                    var embedNotifier = new EmbedBuilder() {
                        Color = new Color(0, 0, 0),
                        Description = statusData.expression
                    };
                    if (User != message.Author)
                    {
                        if (statusData.statusType == "away")
                        {
                            embedNotifier.WithAuthor(awayUser);
                            await message.Channel.SendMessageAsync("", false, embedNotifier.Build());
                        }
                        else if (statusData.statusType == "busy")
                        {
                            embedNotifier.WithAuthor(busyUser);
                            await message.Channel.SendMessageAsync("", false, embedNotifier.Build());
                        }
                    }
                    

                    
                }
            }
        }

        private void SaveStatusQuota() {
            using (StreamWriter statusSave = File.CreateText(@"statusQuota.json")) {
                using (JsonWriter streamSaver = new JsonTextWriter(statusSave)) {
                    _JsonSerializer.Serialize(streamSaver, _statusQuota);
                }
            }
        }

        private void LoadStatusQuota() {
            if (File.Exists("statusQuota.json")) {
                using (StreamReader statusRead = File.OpenText(@"statusQuota.json")) {
                    using (JsonReader streamReader = new JsonTextReader(statusRead)) {
                        var Temp = _JsonSerializer.Deserialize<ConcurrentDictionary<ulong, _statusData>>(streamReader);
                        if (Temp == null)
                            return;
                        _statusQuota = Temp;
                    }
                }
            }
            else {
                File.Create("statusQuota.json").Dispose();
            }
        }

        public struct _statusData
        {
            public string statusType;
            public string expression;
        }


    }
}