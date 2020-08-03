using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents the data of a <see cref="Multiplayer.Player"/> from a <see cref="GameSession"/>.
    /// </summary>
    public class PlayerData
    {
        public Player Player { get; internal set; }

        public List<GameProperty> Properties { get; set; }

        public void SetPropertyValue(string id, object value)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not the specified property '{id}'");

            Properties.First(x => x.Id == id).Set(value);
        }

        public void AddToProperty(string id, int value)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not the specified property '{id}'");

            var property = Properties.First(x => x.Id == id);

            if (property.ValueType != typeof(int))
                throw new Exception($"Cannot add to the specified property '{id}' as it is not a type of Int32");

            property.Value = ((int)property.Value) + value;
        }

        public void ResetProperty(string id)
            => GetProperty(id)?.Reset();

        public GameProperty GetProperty(string id)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not the specified property '{id}'");

            return Properties.First(x => x.Id == id);
        }

        public object GetPropertyValue(string id)
            => GetProperty(id)?.Value;

        public T GetPropertyValue<T>(string id)
        {
            GameProperty property = GetProperty(id);

            if (property.ValueType?.IsEquivalentTo(typeof(T)) ?? false)
                return (T) property.Value;

            throw new Exception($"The specified property '{id}' does not match the implicit type reference of {typeof(T).Name}");
        }

        public override string ToString()
        {
            var info = new StringBuilder();
            info.AppendLine($"Data for {Player.User.Username}:");

            foreach (GameProperty property in Properties)
            {
                info.AppendLine($"{property.Id}: {property.Value} ({property.ValueType.Name})");
            }

            return info.ToString();
        }
    }

}
