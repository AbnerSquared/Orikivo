using System;

namespace Orikivo.Gaming
{
    public class AttributePredicate
    {
        public AttributePredicate(string name, Func<object, bool> func)
        {
            Name = name;
            Func = func;
        }

        public string Name { get; }
        public Func<object, bool> Func { get; }
    }

    public class AttributePredicate<TValue> where TValue : struct
    {
        public AttributePredicate(string name, Func<TValue, bool> func)
        {
            Name = name;
            Func = func;
        }

        public string Name { get; }
        public Func<TValue, bool> Func { get; }
    }
}
