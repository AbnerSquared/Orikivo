using System;

namespace Orikivo
{
    // these were referenced from Discord.Net.Utils;
    // a class that stores exception checks to help keep repetition to a minimum.
    public static class Preconditions
    {
        public static void NotNull<T>(T obj, string name, string msg = null) where T : class
        {
            if (obj == null)
                throw ThrowObjectNullException(name, msg);
        }

        private static ArgumentNullException ThrowObjectNullException(string name, string msg)
        {
            if (msg == null)
                return new ArgumentNullException(name);
            else
                return new ArgumentNullException(paramName: name, msg);
        }
    }
}
