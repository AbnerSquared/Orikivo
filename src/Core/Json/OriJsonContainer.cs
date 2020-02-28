using Discord.WebSocket;
using Orikivo.Desync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Orikivo
{


    public class OriJsonContainer
    {
        public OriGlobal Global { get; }
        public ConcurrentDictionary<ulong, User> Users { get; }
        public ConcurrentDictionary<ulong, OriGuild> Guilds { get; }

        public OriJsonContainer()
        {
            Console.WriteLine("-- Now initializing JSON container services. --");

            Users = JsonHandler.RestoreContainer<User>();
            Console.WriteLine($"-- Restored {Users.Count} {OriFormat.GetNounForm("user", Users.Count)}. --");

            Guilds = JsonHandler.RestoreContainer<OriGuild>();
            Console.WriteLine($"-- Restored {Guilds.Count} {OriFormat.GetNounForm("guild", Guilds.Count)}. --");

            Global = JsonHandler.Load<OriGlobal>("global.json") ?? new OriGlobal();
        }

        public User GetOrAddUser(SocketUser user)
        {
            User account;

            if (!Users.ContainsKey(user.Id))
            {
                account = new User(user);
                Users.AddOrUpdate(user.Id, account, (key, value) => account);
                return account;
            }

            Users.TryGetValue(user.Id, out account);
            return account;
        }


        public OriGuild GetOrAddGuild(SocketGuild guild)
        {
            OriGuild account;

            if (!Guilds.ContainsKey(guild.Id))
            {
                account = new OriGuild(guild);
                Guilds.AddOrUpdate(guild.Id, account, (key, value) => account);
                return account;
            }

            Guilds.TryGetValue(guild.Id, out account); 
            return account;
        }

        public bool TryGetUser(ulong id, out User user)
            => Users.TryGetValue(id, out user);

        public bool TryGetGuild(ulong id, out OriGuild guild)
            => Guilds.TryGetValue(id, out guild);

        public void AddOrUpdateUser(ulong id, User user)
        {
            if (id != user.Id)
                throw new Exception("The IDs do not match.");

            Users.AddOrUpdate(user.Id, user, (key, value) => user);
        }
        public void AddOrUpdateGuild(ulong id, OriGuild guild)
        {
            if (id != guild.Id)
                throw new Exception("The IDs do not match.");

            Guilds.AddOrUpdate(guild.Id, guild, (key, value) => guild);
        }

        // saves the user to its directory
        public void SaveUser(User user)
        {
            Users.AddOrUpdate(user.Id, user, (key, value) => user);
            JsonHandler.SaveJsonEntity(user);
        }

        public void TrySaveUser(User user)
        {
            if (Check.NotNull(user))
                SaveUser(user);
        }

        public void TrySaveGuild(OriGuild guild)
        {
            if (Check.NotNull(guild))
                SaveGuild(guild);
        }
        // saves the guild to its directory
        public void SaveGuild(OriGuild guild)
        {
            Guilds.AddOrUpdate(guild.Id, guild, (key, value) => guild);
            JsonHandler.SaveJsonEntity(guild);
        }

        public void SaveAllUsers()
        {
            foreach (KeyValuePair<ulong, User> user in Users)
                SaveUser(user.Value);
        }

        public void SaveAllGuilds()
        {
            foreach (KeyValuePair<ulong, OriGuild> guild in Guilds)
                SaveGuild(guild.Value);
        }
    }
}
