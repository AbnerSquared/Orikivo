using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public class OriJsonContainer
    {
        public OriGlobal Global { get; }
        public ConcurrentDictionary<ulong, OriUser> Users { get; }
        public ConcurrentDictionary<ulong, OriGuild> Guilds { get; }

        public OriJsonContainer()
        {
            Console.WriteLine("-- Now initializing JSON container services. --");
            Users = OriJsonHandler.RestoreJsonContainer<OriUser>();
            Console.WriteLine($"-- Restored {Users.Count} {OriFormat.GetNounForm("user", Users.Count)}. --");
            Guilds = OriJsonHandler.RestoreJsonContainer<OriGuild>();
            Console.WriteLine($"-- Restored {Guilds.Count} {OriFormat.GetNounForm("guild", Guilds.Count)}. --");
            Global = OriJsonHandler.Load<OriGlobal>("global.json") ?? new OriGlobal();
        }

        public OriUser GetOrAddUser(SocketUser user)
        {
            OriUser oriUser;
            if (!Users.ContainsKey(user.Id))
            {
                oriUser = new OriUser(user);
                Users.AddOrUpdate(user.Id, oriUser, (key, value) => oriUser);
                return oriUser;
            }
            if (!Users.TryGetValue(user.Id, out oriUser))
                throw new Exception("There was an error extracting a user account.");
            return oriUser;
        }

        public OriGuild GetOrAddGuild(SocketGuild guild)
        {
            OriGuild oriGuild;
            if (!Guilds.ContainsKey(guild.Id))
            {
                oriGuild = new OriGuild(guild);
                Guilds.AddOrUpdate(guild.Id, oriGuild, (key, value) => oriGuild);
                return oriGuild;
            }
            if (!Guilds.TryGetValue(guild.Id, out oriGuild))
                throw new Exception("There was an error extracting a guild account.");
            return oriGuild;
        }

        public bool TryGetUser(ulong id, out OriUser oriUser)
            => Users.TryGetValue(id, out oriUser);

        public bool TryGetGuild(ulong id, out OriGuild oriGuild)
            => Guilds.TryGetValue(id, out oriGuild);

        private void AddOrUpdateUser(OriUser oriUser)
        {
            oriUser.LastSaved = DateTime.UtcNow;
            Users.AddOrUpdate(oriUser.Id, oriUser, (key, value) => oriUser);
        }

        private void AddOrUpdateGuild(OriGuild oriGuild)
        {
            Guilds.AddOrUpdate(oriGuild.Id, oriGuild, (key, value) => oriGuild);
        }

        // saves the user to its directory
        public void SaveUser(OriUser oriUser)
        {
            oriUser.LastSaved = DateTime.UtcNow;
            AddOrUpdateUser(oriUser);
            OriJsonHandler.SaveJsonEntity(oriUser);
        }

        public void TrySaveUser(OriUser user)
        {
            if (Checks.NotNull(user))
                SaveUser(user);
        }

        public void TrySaveGuild(OriGuild guild)
        {
            if (Checks.NotNull(guild))
                SaveGuild(guild);
        }
        // saves the guild to its directory
        public void SaveGuild(OriGuild oriGuild)
        {
            // due to the json container being updated globally
            // you don't need to check if it updated
            // just save it directly.
            oriGuild.LastSaved = DateTime.UtcNow;
            AddOrUpdateGuild(oriGuild);
            OriJsonHandler.SaveJsonEntity(oriGuild);
        }

        public void SaveAllUsers()
        {
            foreach (KeyValuePair<ulong, OriUser> user in Users)
                SaveUser(user.Value);
        }

        public void SaveAllGuilds()
        {
            foreach (KeyValuePair<ulong, OriGuild> guild in Guilds)
                SaveGuild(guild.Value);
        }
    }
}
