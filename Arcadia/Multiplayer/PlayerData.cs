using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    public class PlayerData
    {
        public Player Player { get; internal set; }

        public List<GameProperty> Properties { get; set; }

        public void SetPropertyValue(string id, object value)
        {
            if (!Properties.Any(x => x.Id == id))
                throw new Exception($"Unable to find the specified attribute '{id}'");

            Properties.First(x => x.Id == id).Set(value);
        }

        public void AddToProperty(string id, int value)
        {
            if (!Properties.Any(x => x.Id == id))
                throw new Exception($"Cannot find the attribute '{id}' specified");

            var attribute = Properties.First(x => x.Id == id);

            if (attribute.ValueType != typeof(int))
                throw new Exception($"Cannot add to attribute '{id}' as it is not a type of Int32");

            attribute.Value = ((int)attribute.Value) + value;
        }

        public void ResetProperty(string id)
            => GetProperty(id)?.Reset();

        public GameProperty GetProperty(string id)
        {
            if (!Properties.Any(x => x.Id == id))
                throw new Exception($"Unable to find the specified attribute '{id}'");

            return Properties.First(x => x.Id == id);
        }

        public object GetPropertyValue(string id)
            => GetProperty(id)?.Value;

        public T GetPropertyValue<T>(string id)
        {
            var property = GetProperty(id);

            if (property.ValueType != null)
            {
                if (property.ValueType.IsEquivalentTo(typeof(T)))
                {
                    return (T)property.Value;
                }
            }

            throw new Exception("The specified type within the property does not match the implicit type reference");
        }
    }

}
