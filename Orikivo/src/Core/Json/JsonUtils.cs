using System.IO;

namespace Orikivo
{
    internal static class JsonUtils
    {
        internal static string GetDirectoryIndex<T>()
        {
            if (typeof(T) == typeof(Desync.User))
                return Directory.CreateDirectory(@"..\data\users\").FullName;

            if (typeof(T) == typeof(OriGuild))
                return Directory.CreateDirectory(@"..\data\guilds\").FullName;

            if (typeof(T) == typeof(OriGlobal))
                return Directory.CreateDirectory(@"..\data\global\").FullName;

            return Directory.CreateDirectory(@"..\data\").FullName;
        }
    }
}