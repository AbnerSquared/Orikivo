using System.IO;

namespace Orikivo
{
    internal static class JsonUtils
    {
        internal static string GetDirectoryIndex<T>()
        {
            if (typeof(T) == typeof(BaseUser))
                return Directory.CreateDirectory(@"bin/Release/data/users/").FullName;

            if (typeof(T) == typeof(BaseGuild))
                return Directory.CreateDirectory(@"bin/Release/data/guilds/").FullName;

            return Directory.CreateDirectory(@"bin/Release/data/").FullName;
        }
    }
}