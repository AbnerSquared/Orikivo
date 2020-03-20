using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a strict handler for possible exceptions.
    /// </summary>
    public static class Catch
    {
        // string variant
        public static void NotNull(string obj, string name, string msg = null)
        {
            if (string.IsNullOrWhiteSpace(obj))
                throw ThrowObjectNullException(name, msg);
        }

        // Referenced from Discord.Net.Utils.
        public static void NotNull<T>(T obj, string name, string msg = null) where T : class
        {
            if (obj == null)
                throw ThrowObjectNullException(name, msg);
        }

        // Exception presets.
        private static ArgumentNullException ThrowObjectNullException(string name, string msg)
        {
            if (msg == null)
                return new ArgumentNullException(name);
            else
                return new ArgumentNullException(paramName: name, msg);
        }
    }
}
