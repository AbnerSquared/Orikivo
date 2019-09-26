using System;

namespace Orikivo
{
    public static class TypeExtension
    {
        public static T Build<T>(this Type t)
            => (T)Activator.CreateInstance(t);
    }
}
