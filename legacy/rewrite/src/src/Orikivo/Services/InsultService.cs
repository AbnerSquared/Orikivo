using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Orikivo.Systems.Services
{
    // Rebuild, and make it to where insulting people with a set level lower than
    // yours will cancel it out, and allow an override of insult levels on the
    // base server.
    public class InsultService {
        private ConcurrentDictionary<ulong, _insultData> _insultQuota = new ConcurrentDictionary<ulong, _insultData>();
        private JsonSerializer _JsonSerializer = new JsonSerializer();

        public InsultService() {
            LoadInsultQuota();
        }

        string[] insultCurrentLevelNames =
        {
            "_polite",
            "_adaquate",
            "_brash",
            "_rude",
            "_cruel",
            "_painful",
            "_may-actually-set-you-on-fire",
            "_help"
        };

        private async Task InsultDefineLevel(OrikivoCommandContext Context, int insultLevel, bool hasUpdated)
        {
            _insultData insultData = new _insultData()
            {
                guildInsultLevel = insultLevel
            };
            _insultData ignore;
            _insultQuota.TryRemove(Context.Guild.Id, out ignore);
            _insultQuota.AddOrUpdate(Context.Guild.Id, insultData,((key, oldValue) => insultData));
            var embedInsultCheck = new EmbedBuilder
            {
                Color = new Color(0, 0, 0),
                Description = $"**Insult-o-Meter** ( { (hasUpdated == true ? $"Insult level set to {insultData.guildInsultLevel + 1} : {insultCurrentLevelNames[insultData.guildInsultLevel]}" : $"Insult level created and set to {insultData.guildInsultLevel + 1} : {insultCurrentLevelNames[insultData.guildInsultLevel]}") } )"
            };
            await Context.Channel.SendMessageAsync("", false, embedInsultCheck.Build());
        }

        public async Task SetInsultLevelData(OrikivoCommandContext Context, int insultLevel) {
            try {
                if (!_insultQuota.ContainsKey(Context.Guild.Id)) {
                    await InsultDefineLevel(Context, insultLevel, false);
                }
                else {
                        await InsultDefineLevel(Context, insultLevel, true);
                }
                SaveInsultQuota();
            }
            catch (Exception onError) {
                Console.WriteLine(onError);
            }
        }

    public async Task LevelRead(OrikivoCommandContext Context) {
        if (_insultQuota.ContainsKey(Context.Guild.Id)){
            _insultData insultData = new _insultData();
            _insultQuota.TryGetValue(Context.Guild.Id, out insultData);
                /*string insultLevelShow = $"`Your current insult level is {insultData.guildInsultLevel + 1} : {insultCurrentLevelNames[insultData.guildInsultLevel]}`";*/
            int totalLevel = 8;
            int roastAmount = insultData.guildInsultLevel + 1;
            int cleanAmount = totalLevel - roastAmount;
            Emoji isRoastLevel = new Emoji("🔹");
            Emoji isCleanLevel = new Emoji("▫");
            string emoteRoastLine = new String('y', roastAmount).Replace("y", $"{isRoastLevel}");
            string emoteCleanLine = new String('n', cleanAmount).Replace("n", $"{isCleanLevel}");
            string emoteCheck = $"{emoteRoastLine}{emoteCleanLine}";
            string emoteStrip = $"{emoteCheck}";
            string insultName = $"level {roastAmount} : {insultCurrentLevelNames[insultData.guildInsultLevel]}";
            var embedMeter = new EmbedBuilder
            {
                Description = $"**Insult-o-Meter** ({emoteStrip})",
                Footer = new EmbedFooterBuilder
                {
                    Text = insultName
                }
            };
            await Context.Channel.SendMessageAsync("", false, embedMeter.Build());
        }
        else
        {
            string insultLevelShow = $"`Your current insult level is not set. Until you do so, your level will be set to {0 + 1} : {insultCurrentLevelNames[0]}`";
            await Context.Channel.SendMessageAsync(insultLevelShow);
            }
    }

        /*Note from Peter:
         that else branch gives off a feeling you can be returning early.
         How about you check if the file doesn't exist first, then return and reduce nesting that way? */

        public int LevelGet(OrikivoCommandContext Context)
        {
            if (_insultQuota.ContainsKey(Context.Guild.Id))
            {
                _insultData insultData = new _insultData();
                _insultQuota.TryGetValue(Context.Guild.Id, out insultData);
                Console.WriteLine($"Current Guild Insult Level : {insultData.guildInsultLevel}");
                int insultLevelReturn = insultData.guildInsultLevel;
                return insultLevelReturn;
            }
            else
            {
                return 0;
            }
        }

        private void SaveInsultQuota() {
            using (StreamWriter insultSave = File.CreateText(@"insultQuota.json")) {
                using (JsonWriter streamSaver = new JsonTextWriter(insultSave)) {
                    _JsonSerializer.Serialize(streamSaver, _insultQuota);
                }
            }
        }

        private void LoadInsultQuota() {
            if (File.Exists("insultQuota.json")) {
                using (StreamReader insultRead = File.OpenText(@"insultQuota.json")) {
                    using (JsonReader streamReader = new JsonTextReader(insultRead)) {
                        var Temp = _JsonSerializer.Deserialize<ConcurrentDictionary<ulong, _insultData>>(streamReader);
                        if (Temp == null)
                            return;
                        _insultQuota = Temp;
                    }
                }
            }
            else {
                File.Create("insultQuota.json").Dispose();
            }
        }

        public struct _insultData
        {
            public int guildInsultLevel;
        }


    }
}