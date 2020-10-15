using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Multiplayer
{
    public class PlayerResult
    {
        public long Money { get; set; }

        public ulong Exp { get; set; }

        public List<StatUpdatePacket> Stats { get; set; } = new List<StatUpdatePacket>();

        public List<ItemUpdatePacket> Items { get; set; } = new List<ItemUpdatePacket>();

        public IReadOnlyList<GameProperty> PlayerProperties { get; internal set; }

        public GameProperty GetProperty(string id)
        {
            Logger.Debug($"Getting property {id}");

            if (PlayerProperties.All(x => x.Id != id))
                throw new ValueNotFoundException("Could not find the specified property", id);

            return PlayerProperties.First(x => x.Id == id);
        }

        public object ValueOf(string id)
            => GetProperty(id).Value;

        public T ValueOf<T>(string id)
        {
            GameProperty property = GetProperty(id);

            if (property.ValueType == null || !property.ValueType.IsEquivalentTo(typeof(T)))
                throw new Exception("The specified type within the property does not match the implicit type reference");

            return (T)property.Value;
        }

        public Type TypeOf(string id)
            => GetProperty(id).ValueType;
    }
}
