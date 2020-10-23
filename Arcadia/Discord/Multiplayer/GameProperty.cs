using System;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a variable for a <see cref="GameBase"/>.
    /// </summary>
    public class GameProperty
    {
        /// <summary>
        /// Creates a new <see cref="GameProperty"/> of the specified <see cref="Type"/> and value.
        /// </summary>
        /// <typeparam name="T">The enforced <see cref="Type"/> of this <see cref="GameProperty"/>.</typeparam>
        /// <param name="id">The name of this <see cref="GameProperty"/>.</param>
        /// <param name="defaultValue">The default value of this <see cref="GameProperty"/>.</param>
        /// <returns>A new <see cref="GameProperty"/> with an enforced <see cref="Type"/>.</returns>
        public static GameProperty Create<T>(string id, T defaultValue = default)
        {
            return new GameProperty
            {
                Id = id,
                Value = defaultValue,
                DefaultValue = defaultValue,
                ValueType = typeof(T)
            };
        }

        /// <summary>
        /// Creates a new <see cref="GameProperty"/> with the specified value.
        /// </summary>
        /// /// <param name="id">The name of this <see cref="GameProperty"/>.</param>
        /// <param name="defaultValue">The default value of this <see cref="GameProperty"/>.</param>
        /// <param name="enforceType">If true, enforces the <see cref="Type"/> of the specified default value.</param>
        public static GameProperty Create(string id, object defaultValue = default, bool enforceType = false)
        {
            if (defaultValue?.GetType() == null)
                throw new Exception("Cannot initialize a game property with an empty value");

            return new GameProperty
            {
                Id = id,
                Value = defaultValue,
                DefaultValue = defaultValue,
                ValueType = enforceType ? defaultValue.GetType() : null
            };
        }

        protected GameProperty() { }

        /// <summary>
        /// Gets the name of this <see cref="GameProperty"/>.
        /// </summary>
        public string Id { get; internal set; }

        private object _value;

        /// <summary>
        /// Gets the current value of this <see cref="GameProperty"/>.
        /// </summary>
        public object Value
        {
            get => _value;

            internal set
            {
                if (ValueType != null)
                {
                    if (value.GetType().IsEquivalentTo(ValueType))
                        _value = value;
                    else
                        throw new Exception($"Could not set the specified value to the property '{Id}' as their types do not match");
                }
                else
                    _value = value;
            }
        }

        /// <summary>
        /// If specified, requires the inbound value to match this <see cref="Type"/>.
        /// </summary>
        public Type ValueType { get; protected set; }

        /// <summary>
        /// Represents the default value specified on the initialization of this <see cref="GameProperty"/>.
        /// </summary>
        public object DefaultValue { get; protected set; }

        /// <summary>
        /// Sets the value of this <see cref="GameProperty"/> to the specified value.
        /// </summary>
        public void Set(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Resets the value of this <see cref="GameProperty"/> to its default value.
        /// </summary>
        public void Reset()
        {
            Value = DefaultValue;
        }
    }

}
