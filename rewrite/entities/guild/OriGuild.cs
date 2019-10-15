using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // socketguild account counterpart
    public class OriGuild : IDiscordEntity<SocketGuild>, IJsonEntity
    {
        [JsonConstructor]
        internal OriGuild(ulong id, ulong? ownerId, string name, DateTime createdAt, OriGuildOptions options,
            List<CustomGuildCommand> customCommands, List<UserRoleInfo> tempRoles)
        {
            Id = id;
            OwnerId = ownerId ?? 0;
            Name = name;
            CreatedAt = createdAt;
            Options = options;
            CustomCommands = customCommands ?? new List<CustomGuildCommand>();
            TempRoles = tempRoles ?? new List<UserRoleInfo>();
        }

        public OriGuild(SocketGuild guild)
        {
            Id = guild.Id;
            OwnerId = guild.OwnerId;
            Name = guild.Name;
            CreatedAt = DateTime.UtcNow;
            Options = OriGuildOptions.Default;
            CustomCommands = new List<CustomGuildCommand>();
            TempRoles = new List<UserRoleInfo>();
        }

        [JsonProperty("id")]
        public ulong Id { get; } // guild id

        // update these at OriCommandContext
        [JsonProperty("owner_id")]
        public ulong OwnerId { get; internal set; } // guild owner id

        [JsonProperty("name")]
        public string Name { get; internal set; } // name of guild

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        // guild exp/balance is only grown through group actions/tasks
        [JsonProperty("balance")]
        public ulong Balance { get; private set; }

        [JsonProperty("exp")]
        public ulong Exp { get; private set; }

        // inner-layer properties
        [JsonProperty("options")]
        public OriGuildOptions Options { get; private set; }

        [JsonProperty("custom_commands")]
        public List<CustomGuildCommand> CustomCommands { get; private set; }

        [JsonProperty("temp_roles")]
        public List<UserRoleInfo> TempRoles { get; private set; }

        public List<ulong> UserBlacklist { get; private set; }


        public bool HasMuted(ulong userId)
            => TempRoles.Any(x => x.UserId == userId && !x.HasExpired);

        public string Greet(SocketUser user)
            => OriFormat.ParseGreeting(OriRandom.NextElement(Options.Greetings ?? OriGuildOptions.Default.Greetings).Message, user);

        public void Mute(ulong userId, double seconds)
        {
            TempRoles.RemoveAll(x => x.HasExpired);
            IEnumerable<UserRoleInfo> userMuteInfo = TempRoles.TakeWhile(x => x.UserId == userId && x.RoleId == Options.MuteRoleId.Value);
            if (userMuteInfo.Count() > 0)
            {
                if (userMuteInfo.Count() > 1)
                    throw new Exception("There are multiple mute counts.");
                TempRoles.ElementAt(TempRoles.IndexOf(userMuteInfo.First())).ExpiresOn = userMuteInfo.First().ExpiresOn.Value.AddSeconds(seconds);
                return;
            }
            TempRoles.Add(new UserRoleInfo(userId, Options.MuteRoleId.Value, seconds));
        }

        // make unmuting independent of muteroleid, just in case the mute role id has changed
        public void Unmute(ulong userId)
        {
            TempRoles.RemoveAll(x => x.HasExpired);
            if (HasMuted(userId))
                TempRoles.RemoveAll(x => x.UserId == userId && x.RoleId == Options.MuteRoleId.Value);
        }
        

        // update upon any properties being changed
        [JsonIgnore]
        public DateTime? LastSaved { get; internal set; }

        [JsonIgnore]
        public bool HasChanged { get; internal set; }

        public void AddCommand(CustomGuildCommand command)
        {
            if (CustomCommands.Any(x => x.Name.ToLower() == command.Name.ToLower()))
                throw new Exception("This command name conflicts with an existing command.");
            // reserved command name check
            CustomCommands.Add(command);
        }

        public bool TryRemoveCommand(string commandName)
        {
            if (CustomCommands.Any(x => x.Name.ToLower() == commandName.ToLower()))
            {
                CustomCommands.Remove(CustomCommands.First(x => x.Name.ToLower() == commandName.ToLower()));
                return true;
            }
            return false;
        }

        // functions
        public OriMessage GetDisplay(EntityDisplayFormat displayFormat)
        {
            OriMessage oriMessage = new OriMessage();
            switch (displayFormat)
            {
                case EntityDisplayFormat.Json:
                    StringBuilder sbj = new StringBuilder();
                    sbj.AppendLine("```json");
                    sbj.AppendLine("{");
                    sbj.AppendLine($"    \"name\": \"{Name}\",");
                    sbj.AppendLine($"    \"id\": \"{Id}\",");
                    sbj.AppendLine($"    \"created_at\": \"{CreatedAt}\"");
                    sbj.AppendLine("}```");
                    oriMessage.Text = sbj.ToString();
                    return oriMessage;
                default:
                    StringBuilder sbd = new StringBuilder();
                    sbd.AppendLine($"{Format.Bold(Name)}"); // name display
                    sbd.AppendLine($"{Format.Bold("id")}:{Id}");// id display
                    sbd.AppendLine($"{Format.Bold("joined")}:{CreatedAt.ToString($"`yyyy`.`MM`.`dd` • `hh`:`mm`:`ss`{CreatedAt.ToString("tt")}")}");
                    oriMessage.Text = sbd.ToString();
                    return oriMessage;
            }
        }

        public bool TryGetSocketEntity(BaseSocketClient client, out SocketGuild guild)
        {
            guild = client.GetGuild(Id);
            return guild != null;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as IJsonEntity);
        }

        public bool Equals(IJsonEntity obj)
            => Id == obj.Id;

        public override int GetHashCode()
            => unchecked((int)Id);
    }
}
