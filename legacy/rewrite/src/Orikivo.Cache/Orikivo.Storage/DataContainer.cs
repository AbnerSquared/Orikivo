using System.Collections.Concurrent;
using Orikivo.Static;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using Orikivo.Systems.Presets;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Linq;
using Orikivo.Networking;
using System.Drawing;
using Orikivo.Storage;
using Newtonsoft.Json;
using System;

namespace Orikivo
{
    // Make datamanager static, to prevent the need to constantly rebuild.
    public class DataContainer
    {
        // Get all data from saved data
        public OldGlobal Global;
        public ConcurrentDictionary<ulong, OldAccount> Accounts;
        public ConcurrentDictionary<ulong, Server> Guilds;
        //public List<ModuleInfo> Modules;
        public List<ModuleInfo> Modules;

        //load blacklist.json
        public List<string> Blacklist;

        // Reset all temporary dictionaries.
        public ConcurrentDictionary<ulong, ulong> Gamblers;

        public DataContainer()
        {
            Build();
            ResetAsync();
        }

        public void EnsureList(IEnumerable<ModuleInfo> modules)
        {
            Modules = modules.ToList();
        }

        public void Build()
        {
            EnsureGlobal();
            Accounts = Accounts.GetContainer();
            Guilds = Guilds.GetContainer();
            Blacklist = new List<string>(); // to ensure nothing is null...
        }

        #region Servers
        public Server GetOrAddServer(SocketGuild g)
        {
            if (!Guilds.ContainsKey(g.Id))
                AddServer(g);
            Guilds.TryGetValue(g.Id, out Server tmp);
            return tmp;
        }

        public bool TryGetServer(SocketGuild g, out Server s)
        {
            s = null;
            if (Guilds.TryGetValue(g.Id, out s))
            {
                return true;
            }
            return false;
        }

        public void AddServer(SocketGuild g)
        {
            Update(new Server(g));
        }
        #endregion

        #region Accounts
        public OldAccount GetOrAddAccount(SocketUser u)
        {
            if (!Accounts.ContainsKey(u.Id))
                AddAccount(u);
            TryGetAccount(u.Id, out OldAccount a);
            return a;
        }

        public OldAccount GetOrAddAccount(ulong id)
        {
            if (!Accounts.ContainsKey(id))
                AddAccount(id);
            TryGetAccount(id, out OldAccount a);
            return a;
        }

        public bool TryGetAccount(SocketUser u, out OldAccount a)
            => TryGetAccount(u.Id, out a);

        public bool TryGetAccount(ulong id, out OldAccount a)
        {
            a = null;
            if(Accounts.TryGetValue(id, out a))
            {
                return true;
            }

            return false;
        }

        public void AddAccount(SocketUser u)
            => Update(new OldAccount(u));

        public void AddAccount(ulong id)
            => Update(new OldAccount(id));

        public void Update(Server s)
        {
            Guilds.AddOrUpdate(s.Id, s, (key, value) => s);
            s.Save();
        }

        public void Update(params Server[] i)
        {
            foreach (Server s in i)
                Update(s);
        }

        public void Update(OldAccount a)
        {
            Accounts.AddOrUpdate(a.Id, a, (key, value) => a);
            a.Save();
        }

        public void Update(params OldAccount[] i)
        {
            foreach (OldAccount a in i)
                Update(a);
        }
        #endregion

        #region Global
        public void Update(OldGlobal g)
        {
            Global.SetObject(g);
            g.Save();
        }

        public void EnsureGlobal()
        {
            if (!Global.TryGetObject(out OldGlobal value))
                Update(new OldGlobal());
            else
                Global = value;
        }
        #endregion

        // These fields are only logged from the current service.
        // However, money should be returned.

        #region Gamblers
        public async Task ResetAsync()
        {
            foreach (KeyValuePair<ulong, ulong> pair in Gamblers)
                await ReturnGamblerAsync(pair.Key, pair.Value);
        }

        private Embed GetGambleError(OldAccount a, ulong wager)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("error"));
            StringBuilder su = new StringBuilder();
            su.AppendLine($"Hello, {a.GetName()}.");
            su.AppendLine($"As you may know, I was accidently shut down during a wager you placed, and for that, I am sorry.");
            su.Append($"I have returned {EmojiIndex.Balance}{wager.ToPlaceValue().MarkdownBold()} to your wallet.");
            e.WithDescription(su.ToString());
            return e.Build();
        }

        /// <summary>
        /// Returns any wagers that may have occured upon Orikivo shutting down.
        /// </summary>
        private async Task ReturnGamblerAsync(ulong id, ulong wager)
        {
            OldAccount a = GetOrAddAccount(id);
            CompactMessage msg = new CompactMessage(embeds: GetGambleError(a, wager));
            OldMail m = new OldMail("Orikivo", "Gambling Error", msg);
            //await m.SendAsync(a, GlobalClient);
            a.Give(wager);
        }

        public void AddGambler(OldAccount a, ulong wager)
            => AddGambler(a.Id, wager);

        public void AddGambler(ulong id, ulong wager)
        {
            Gamblers.AddOrUpdate(id, wager, (key, value) => wager);
        }

        public void RemoveGambler(OldAccount a, out ulong wager)
            => RemoveGambler(a.Id, out wager);

        public void RemoveGambler(ulong id, out ulong wager)
        {
            Gamblers.TryRemove(id, out wager);
        }
        #endregion
    }
}
