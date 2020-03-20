using System;

namespace Orikivo.Gaming
{
    public class GameAttribute // stores a loosely defined value with an internal default one to set.
    {
        public static GameAttribute Create<TValue>(string name, TValue defaultValue) where TValue : struct
        {
            var attribute = new GameAttribute(name, defaultValue);
            return attribute;
        }

        public GameAttribute(string name, object defaultValue)
        {
            Name = name;
            Value = DefaultValue = defaultValue;
        }

        public string Name { get; }

        public object Value { get; private set; }
        public object DefaultValue { get; }

        public bool Judge(Match match, object value)
        {
            throw new NotImplementedException();
        }

        public bool Check(AttributePredicate predicate)
        {
            return predicate.Name == Name && predicate.Func.Invoke(Value);
        }

        public bool Check(Func<object, bool> predicate)
        {
            return predicate.Invoke(Value);
        }

        public AttributePredicate Predicate(Func<object, bool> func)
        {
            return new AttributePredicate(Name, func);
        }
    }

    public class GameAttribute<TValue> where TValue : struct // stores an explicity defined value with an internal default one to set.
    {
        public GameAttribute(string name, TValue value = default)
        {
            Name = name;
            Value = DefaultValue = value;
        }

        public string Name { get; }

        public TValue Value { get; private set; }
        public TValue DefaultValue { get; }

        public bool Check(AttributePredicate<TValue> predicate)
        {
            return predicate.Name == Name && predicate.Func.Invoke(Value);
        }

        public bool Check(Func<TValue, bool> predicate)
        {
            return predicate.Invoke(Value);
        }

        public AttributePredicate<TValue> Predicate(Func<TValue, bool> func)
        {
            return new AttributePredicate<TValue>(Name, func);
        }

        public GameAttribute GetGenericType() // could probably auto-cast this as an object instead of creating an entirely new format
        {
            return GameAttribute.Create(Name, DefaultValue);
        }
    }
}
