using Discord.Commands;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Cache
{
    public class DataContainer
    {
        public Global Global;
        public ConcurrentDictionary<ulong, OriUser> Accounts;
        public ConcurrentDictionary<ulong, OriGuild> Guilds;
        public List<string> Modules; // names of all modules.

        public void Load()
        {
            Accounts = CacheManager.GetContainer<OriUser>(); // get users
            Guilds = CacheManager.GetContainer<OriGuild>(); // get guilds
        }

        public void LogModules(IEnumerable<ModuleInfo> modules)
            => Modules = modules.Enumerate(x => x.Name).ToList();

        /*public IModelEntity GetOrAddEntity(IModelEntity ent)
        {

        }

        public IModelEntity TryGetEntity(IModelEntity ent)
        {

        }*/
    }   
}