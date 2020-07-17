using System;

namespace Orikivo
{
    public class OptionData
    {
        public OptionData(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public string Name { get; }
        public Type Type { get;  }
        public object Value { get; }
    }
}
