using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Systems.Services
{
    public class GuildPrefUtility
    {
        private ConcurrentDictionary<ulong, _safeData> _safeQuota = new ConcurrentDictionary<ulong, _safeData>();
        private JsonSerializer _JsonSerializer = new JsonSerializer();

        public GuildPrefUtility()
        {
            LoadSafeQuota();
        }
        public async Task GetCurrentPrefs(OrikivoCommandContext Context)
        {
            if (_safeQuota.ContainsKey(Context.Guild.Id))
            {
                _safeData safeData = new _safeData();
                _safeQuota.TryGetValue(Context.Guild.Id, out safeData);
                await Context.Channel.SendMessageAsync($"`current safe preference : {safeData.guildSafePref}\n [sayd state : {safeData.guildSayDToggle}`");
            }
            else
            {
                var returnMsg = "The guild pref file has not been set, and will default to true.\nUse [gpref safemode <t || f> or [gpref sayd <t || f> to set.";
                await Context.Channel.SendMessageAsync(returnMsg);
            }
        }
        private async Task SafeDefineBool(OrikivoCommandContext Context, bool safeReturn, bool hasUpdated)
        {
            _safeData safeData = new _safeData()
            {
                guildSafePref = safeReturn,
                guildSayDToggle = GetSayDPref(Context, Context.Guild),
            };
            _safeData ignore;
            _safeQuota.TryRemove(Context.Guild.Id, out ignore);
            _safeQuota.AddOrUpdate(Context.Guild.Id, safeData, ((key, oldValue) => safeData));
            var embedSafeCheck = new EmbedBuilder
            {
                Color = new Color(0, 0, 0),
                Description = $"**Safe Check** ( { (hasUpdated == true ? $"Safe mode set to {safeData.guildSafePref}." : $"Guild preferences created and set to {safeData.guildSafePref}.") } )"
            };
            await Context.Channel.SendMessageAsync("", false, embedSafeCheck.Build());
        }
        public async Task SetSafePref(OrikivoCommandContext Context, bool safeReturn)
        {
            try
            {
                if (!_safeQuota.ContainsKey(Context.Guild.Id))
                {
                    await SafeDefineBool(Context, safeReturn, false);
                }
                else
                {
                    await SafeDefineBool(Context, safeReturn, true);
                }
                SaveSafeQuota();
            }
            catch (Exception onError)
            {
                Console.WriteLine(onError);
            }
        }

        private async Task SayDBool(OrikivoCommandContext Context, bool sayCheck, bool hasUpdated)
        {
            _safeData safeData = new _safeData()
            {
                guildSafePref = GetSafePref(Context, Context.Guild),
                guildSayDToggle = sayCheck
            };
            _safeData ignore;
            _safeQuota.TryRemove(Context.Guild.Id, out ignore);
            _safeQuota.AddOrUpdate(Context.Guild.Id, safeData, ((key, oldValue) => safeData));
            var embedSafeCheck = new EmbedBuilder
            {
                Color = new Color(0, 0, 0),
                Description = $"**Safe Check** ( { (hasUpdated == true ? $"[sayd is set to {safeData.guildSayDToggle}." : $"Guild preferences for [sayd created and set to {safeData.guildSayDToggle}.") } )"
            };
            await Context.Channel.SendMessageAsync("", false, embedSafeCheck.Build());
        }
        public async Task SetSayDPref(OrikivoCommandContext Context, bool sayCheck)
        {
            try
            {
                if (!_safeQuota.ContainsKey(Context.Guild.Id))
                {
                    await SayDBool(Context, sayCheck, false);
                }
                else
                {
                    await SayDBool(Context, sayCheck, true);
                }
                SaveSafeQuota();
            }
            catch (Exception onError)
            {
                Console.WriteLine(onError);
            }
        }

        public bool GetSafePref(OrikivoCommandContext Context, SocketGuild guild)
        {
            if (_safeQuota.ContainsKey(guild.Id))
            {
                _safeData safeData = new _safeData();
                _safeQuota.TryGetValue(guild.Id, out safeData);
                Console.WriteLine($"guild.safeSend => {safeData.guildSafePref}");
                bool safeSend = safeData.guildSafePref;
                return safeSend;
            }
            else
            {
                return true;
            }
        }

        public bool GetSayDPref(OrikivoCommandContext Context, SocketGuild guild)
        {
            if (_safeQuota.ContainsKey(guild.Id))
            {
                _safeData safeData = new _safeData();
                _safeQuota.TryGetValue(guild.Id, out safeData);
                Console.WriteLine($"guild.sayDSend => {safeData.guildSayDToggle}");
                bool sayDSend = safeData.guildSayDToggle;
                return sayDSend;
            }
            else
            {
                return true;
            }
        }

        private void SaveSafeQuota()
        {
            using (StreamWriter safeSave = File.CreateText(@"safeQuota.json"))
            {
                using (JsonWriter streamSaver = new JsonTextWriter(safeSave))
                {
                    _JsonSerializer.Serialize(streamSaver, _safeQuota);
                }
            }
        }

        private void LoadSafeQuota()
        {
            if (File.Exists("safeQuota.json"))
            {
                using (StreamReader safeRead = File.OpenText(@"safeQuota.json"))
                {
                    using (JsonReader streamReader = new JsonTextReader(safeRead))
                    {
                        var Temp = _JsonSerializer.Deserialize<ConcurrentDictionary<ulong, _safeData>>(streamReader);
                        if (Temp == null)
                            return;
                        _safeQuota = Temp;
                    }
                }
            }
            else
            {
                File.Create("safeQuota.json").Dispose();
            }
        }

        public struct _safeData
        {
            public bool guildSafePref;
            public bool guildSayDToggle;
        }
    }
}
