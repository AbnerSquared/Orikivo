using System.IO;

namespace Orikivo
{
    internal static class JsonUtils
    {
        internal static string GetDirectoryIndex<T>()
        {
            if (typeof(T) == typeof(Desync.User))
                return Directory.CreateDirectory(@"..\data\users\").FullName;

            if (typeof(T) == typeof(BaseGuild))
                return Directory.CreateDirectory(@"..\data\guilds\").FullName;

            return Directory.CreateDirectory(@"..\data\").FullName;
        }
    }
}