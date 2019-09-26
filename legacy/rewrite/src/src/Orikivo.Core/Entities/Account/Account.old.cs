using System;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Storage;
using System.Collections.Generic;
using Discord;
using System.Threading.Tasks;
using Orikivo.Static;
using Orikivo.Systems.Presets;
using Newtonsoft.Json;

namespace Orikivo
{
    public class OldAccount : IStorable
    {
        public OldAccount(SocketUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
            DiscriminatorValue = user.DiscriminatorValue;
            CreationDate = DateTime.Now;
        }

        public OldAccount(ulong id)
        {
            Id = id;
            CreationDate = DateTime.Now;
        }

        public OldAccount()
        {
            CreationDate = DateTime.Now;
        }

        // Ensure is meant to restore missing objects, in the case of this occuring.
        public void Ensure()
        {
        }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("textdiscriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("discriminator")]
        public ushort DiscriminatorValue { get; set; }

        [JsonProperty("createdat")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("lastdaily")]
        public DateTime UsedDaily { get; set; }

        [JsonProperty("dailystreak")]
        public int DailyStreak { get; set; }

        [JsonProperty("config")]
        public AccountConfig Config { get; set; } = new AccountConfig();

        //[JsonProperty("notifiers")]
        //public AccountNotifier Notifiers {get; set;} = new AccountNotifier();

        //[JsonProperty("trades")]
        //public List<TradeOffer> Trades {get; set;} = new List<TradeOffer>();
        [JsonProperty("gimmestats")]
        public GiveOrTakeAnalyzer GimmeStats { get; set; } = new GiveOrTakeAnalyzer();

        //[JsonProperty("inventory")]
        //public AccountInventory Inventory { get; set; } = new AccountInventory();

        //[JsonProperty("achievements")]
        //public AchievementData Achievements { get; set; }

        //[JsonProperty("display")]
        //public CardConfig Card { get; set; } = new CardConfig();

        //[JsonProperty("mailbox")]
        //public AccountMailbox Mailbox { get; set; } = new AccountMailbox();

        [JsonProperty("cooldowns")]
        public AccountCooldown Cooldowns { get; set; }

        [JsonProperty("isupdated")]
        public bool Updated { get; set; } = true;

        [JsonProperty("isbuilding")]
        public bool Building { get; set; } = false;

        [JsonProperty("balance")]
        public ulong Balance { get; set; } = 0;

        [JsonProperty("debt")]
        public ulong Debt { get; set; } = 0;

        //[JsonProperty("status")]
        //public AccountStatus Status { get; set; } = null;

        [JsonProperty("data")]
        public GlobalData Data { get; set; } = new GlobalData();

        //[JsonProperty("playlists")]
        //public Dictionary<string, List<Song>> Playlists { get; set; } = new Dictionary<string, List<Song>>();

        //[JsonProperty("clipboards")]
        //public List<Clipboard> Clipboards { get; set; } = new List<Clipboard>();

        [JsonProperty("stats")]
        public AnalyzerOld Analytics { get; set; } = new AnalyzerOld();

        [JsonProperty("verbosemodules")]
        public bool VerboseModules { get; set; } = true;

        public void ToggleVerboseModules()
            => VerboseModules = !VerboseModules;

        public void TickDaily()
        {
            DailyStreak += 1;
        }

        public void RepairDiscriminator()
        {
            string discrim = $"{DiscriminatorValue}";
            if (discrim.Length < 4)
            {
                for (int i = discrim.Length; i < 4; i++)
                {
                    discrim.Insert(0, "0");
                }
            }

            Discriminator = discrim;
        }
        /*
        public async Task NotifyAsync(OldMail m, DiscordSocketClient client)
        {
            if (!Config.InboundMail)
                return;

            SocketUser user = User(client);
            IDMChannel channel = await user.GetOrCreateDMChannelAsync();
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("owo"));
            e.WithTitle($"{EmojiIndex.InboundMail} You have have received new mail!");
            e.WithDescription($"{m.Author.Name.MarkdownBold()}\nSubject: {m.Subject.MarkdownBold()}");
            await channel.SendMessageAsync(embed: e.Build());
            await channel.CloseAsync();
        }
        */

        public string GetDefaultName()
            => $"{Username}#{DiscriminatorValue}";

        public string GetName()
            => Config.Nickname ?? Username ?? $"User #{Id.ToPlaceValue()}";

        public List<string> Callers
        {
            get
            {
                List<string> l = new List<string>();
                l.Add(GetDefaultName());
                if (Config.Nickname.Exists())
                {
                    l.Add(Config.Nickname);
                }
                if (Username.Exists())
                {
                    l.Add(Username);
                }
                if (Id.Exists())
                {
                    l.Add($"{Id}");
                }

                return l;
            }
        }

        /*
        public List<Song> DeriveFromQueue(Queue<Song> queue)
        {
            List<Song> tmp = new List<Song>();
            foreach(Song song in queue)
                tmp.Add(song);
            return tmp;
        }*/

        public bool TracksGimme { get { return GimmeStats.Exists(); } }

        public void Use(ActionItem item, OrikivoCommandContext Context)
        {
            //if (!Inventory.HasItem(item))
                //return;

            item.Invoke(this, Context);
            //Inventory.Delete(item);
        }

        //public void SaveQueueAsPlaylist(Server server, string name)
            //=> Playlists.Add(name, DeriveFromQueue(server.Queue));
        
        public void Save()
            => Manager.Save(this, FileManager.TryGetPath(this));

        /*public void TryAddTransaction(Transaction t)
        {
            if (Analytics.Transactions.Count >= Config.TransactionCapacity)
            {
                Analytics.Transactions.Add(t); // fix
            }
        }*/

        public void Store(ActionItem item)
        {
            //Inventory.Store(item);
        }

        public void Store(OldCardColorScheme item)
        {
            //Inventory.Store(item);
        }

        public void UpdateCard()
        {
            // Checks in place, so that the card building system doesnt go oof.
            if (Updated || Building) return;
            Building = true;
            // methods to update the card are here.
            Building = false;
        }

        public bool IsEmpty()
            => Balance == 0;

        public void SetWallet(ulong bal)
        {
            Balance = bal;
            Analytics.TryUpdateMaxHeld(Balance);
        }

        public void Give(ulong amt)
        {
            Balance += amt;
            TryClearDebt();
            Analytics.TryUpdateMaxHeld(Balance);
        }
        
        public void Take(ulong amt)
        {   
            Balance -= (Balance - (double)amt < 0) ? Balance : amt;
            Analytics.UpdateExpended(amt);
        }

        // fines an account with debt.
        public void Fine(ulong amt)
        {
            Debt += amt;
            Analytics.UpdateExpended(amt);
        }

        public void ClearDebt()
        {
            Debt = 0;
        }

        public void TryClearDebt()
        {
            if (Debt == 0)
                return;

            if (Balance - (double)Debt < 0)
            {
                ulong tax = (ulong)(Debt - (Math.Abs(Balance - (double)Debt)));
                Balance -= tax;
                Debt -= tax;
            }
            else
            {
                Balance -= Debt;
                Debt = 0;
            }
        }

        // Attempts to take money in terms of purchasing something, and returns false if it can't.
        public  bool TryBuy(OldCardColorScheme item)
            => TryBuy(item.Cost);

        public bool TryBuy(ActionItem item)
            => TryBuy(item.Cost);

        public bool TryBuy(ulong amt)
        {
            if (Balance - (double)amt < 0)
                return false;
            else
                Take(amt);

            return true;
        }

        // Attempts to take money, and returns false if it can't. Config.Overflow overrides.
        public bool TryTake(ulong amt)
        {
            if (Balance - (double)amt < 0)
            {
                if (Config.Overflow)
                    amt = Balance;
                else
                    return false;
            }

            Take(amt);
            return true;
        }

        public void Donate(OldAccount a, ulong amt)
        {
            if (IsEmpty())
                return;

            Take(amt);
            a.Give(amt);
        }

        public void Steal(OldAccount a, ulong amt)
        {
            if (a.IsEmpty())
                return;

            Give(amt);
            a.Take(amt);
        }

        //public void SetStatus(AccountStatus status)
        //    => Status = status;
        
        //public void SetStatus(StatusState state, string note = null)
         //   => SetStatus(new AccountStatus(state, note));

        //public string GetStatus()
        //    => Status.Exists() ? Status.ToString() : "no status set";

        public void ClearStatus()
        {
         //   if (!Status.Exists())
         //       Status = null;
        }

        public void Update(DataContainer d)
            => d.Update(this);

        public SocketUser User(DiscordSocketClient client)
            => client.GetUser(Id);
        
        #region Account#Override
        public override string ToString()
            => $"account-{Username.ToLower()}.{Id}";
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as IStorable);
        }

        public bool Equals(IStorable storable)
            => Id == storable.Id;

        public override int GetHashCode()
            => unchecked((int)Id);
        #endregion
    }
}