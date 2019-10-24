using System;

namespace Orikivo
{
    /// <summary>
    /// A tool used to to catch any mistakes an object might have to throw exceptions accordingly.
    /// </summary>
    public static class Catcher
    {
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
