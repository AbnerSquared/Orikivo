using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a strict handler for possible exceptions.
    /// </summary>
    public static class Catch
    {
        public static void NotNull(string obj, string name, string msg = null)
        {
            if (string.IsNullOrWhiteSpace(obj))
                throw ThrowObjectNullException(name, msg);
        }

        public static void NotNull<T>(T obj, string name, string msg = null) where T : class
        {
            if (obj == null)
                throw ThrowObjectNullException(name, msg);
        }

        private static ArgumentNullException ThrowObjectNullException(string name, string msg)
        {
            if (msg == null)
                return new ArgumentNullException(name);

            return new ArgumentNullException(name, msg);
        }
    }
}
