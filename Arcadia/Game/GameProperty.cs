using System;

namespace Arcadia
{
    public class GameProperty
    {
        // this method is what enforces 
        public static GameProperty Create<T>(string name, T defaultValue = default)
        {
            return new GameProperty
            {
                Name = name,
                Value = defaultValue,
                DefaultValue = defaultValue,
                ValueType = typeof(T)
            };
        }

        public static GameProperty Create(string name, object defaultValue = default, bool enforceType = false)
        {
            return new GameProperty
            {
                Name = name,
                Value = defaultValue,
                DefaultValue = defaultValue,
                ValueType = enforceType ? defaultValue.GetType() : null
            };
        }



        public string Name { get; private set; }

        private object _value;
        public object Value {
            get => _value;
            internal set
            {
                if (ValueType != null)
                {
                    if (value.GetType().IsEquivalentTo(ValueType))
                        _value = value;
                    else
                        throw new Exception("Could not set the specified value to this property as their types do not match");
                }
                else
                    _value = value;
            }
        }

        // if this is set, it requires the value being set to consistently match this type
        public Type ValueType { get; private set; }

        // when this property is first being given to others, what is the default value to be set?
        public object DefaultValue { get; private set; }
    }

}
