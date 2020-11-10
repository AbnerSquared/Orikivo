using System.Collections.Generic;

namespace Orikivo.Text
{
    public class LocaleNode
    {
        internal LocaleNode(string id, string value)
        {
            Id = id;
            Value = value;
        }

        public string Id { get; }

        public string Value { get; } // set to Values later on

        public override string ToString()
            => Value;

        public string ToString(params object[] args)
            => string.Format(Value, args);
    }
}