using Orikivo.Desync;
using System;
using System.Collections.Generic;
using Discord;
using Orikivo.Framework;

namespace Orikivo
{
    public class DesyncContainer
    {
        public OriGlobal Global { get; }
        public JsonContainer<User> Users { get; }
        public JsonContainer<BaseGuild> Guilds { get; }

        public DesyncContainer()
        {
            Logger.Debug("-- Now initializing JSON container services. --");

            Users = new JsonContainer<User>(@"..\data\users\");
            Logger.Debug($"-- Restored {Users.Count} {Format.TryPluralize("user", Users.Count)}. --");

            Guilds = new JsonContainer<BaseGuild>(@"..\data\guilds\");
            Logger.Debug($"-- Restored {Guilds.Count} {Format.TryPluralize("guild", Guilds.Count)}. --");

            Global = JsonHandler.Load<OriGlobal>("global.json") ?? new OriGlobal();
        }

        public User GetOrAddUser(IUser user)
        {
            User account;

            if (!Users.Values.ContainsKey(user.Id))
            {
                account = new User(user);
                Users.Values.AddOrUpdate(user.Id, account, (key, value) => account);
                return account;
            }

            Users.Values.TryGetValue(user.Id, out account);
            return account;
        }

        public BaseGuild GetOrAddGuild(IGuild guild)
        {
            BaseGuild account;

            if (!Guilds.Values.ContainsKey(guild.Id))
            {
                account = new BaseGuild(guild);
                Guilds.Values.AddOrUpdate(guild.Id, account, (key, value) => account);
                return account;
            }

            Guilds.Values.TryGetValue(guild.Id, out account); 
            return account;
        }

        public bool TryGetUser(ulong id, out User user)
            => Users.TryGet(id, out user);

        public bool TryGetGuild(ulong id, out BaseGuild guild)
            => Guilds.TryGet(id, out guild);

        public void AddOrUpdateUser(ulong id, User user)
        {
            if (id != user.Id)
                throw new Exception("The IDs do not match.");

            Users.Values.AddOrUpdate(user.Id, user, (key, value) => user);
        }

        public void AddOrUpdateGuild(ulong id, BaseGuild guild)
        {
            if (id != guild.Id)
                throw new Exception("The IDs do not match.");

            Guilds.Values.AddOrUpdate(guild.Id, guild, (key, value) => guild);
        }

        // saves the user to its directory
        public void SaveUser(User user)
        {
            Users.Values.AddOrUpdate(user.Id, user, (key, value) => user);
            JsonHandler.SaveJsonEntity(user);
        }

        public void TrySaveUser(User user)
        {
            if (Check.NotNull(user))
                SaveUser(user);
        }

        public void TrySaveGuild(BaseGuild guild)
        {
            if (Check.NotNull(guild))
                SaveGuild(guild);
        }
        // saves the guild to its directory
        public void SaveGuild(BaseGuild guild)
        {
            Guilds.Values.AddOrUpdate(guild.Id, guild, (key, value) => guild);
            JsonHandler.SaveJsonEntity(guild);
        }

        public void SaveAllUsers()
        {
            foreach (KeyValuePair<ulong, User> user in Users.Values)
                SaveUser(user.Value);
        }

        public void SaveAllGuilds()
        {
            foreach (KeyValuePair<ulong, BaseGuild> guild in Guilds.Values)
                SaveGuild(guild.Value);
        }
    }
}
