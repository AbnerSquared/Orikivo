using System;

namespace Orikivo
{
    // used to simplify displaying options.
    public struct OptionInfo
    {
        internal static OptionInfo FromOption<T>(T obj, string objName) => new OptionInfo { Name = objName, Type = typeof(T), Value = obj };

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public object Value { get; private set; }
    }
}
