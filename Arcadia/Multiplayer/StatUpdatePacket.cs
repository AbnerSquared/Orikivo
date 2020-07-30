using System;
using Orikivo;

namespace Arcadia.Multiplayer
{
    public class StatUpdatePacket
    {
        public StatUpdatePacket(string id, long value, StatUpdateType type = StatUpdateType.Update)
        {
            Id = id;
            Value = value;
            Type = type;
        }

        public StatUpdatePacket(string id, string valueId, StatUpdateType type = StatUpdateType.Update)
        {
            Id = id;
            ValueId = valueId;
            Type = type;
        }

        public string Id { get; set; }

        // if ValueId is specified, use that instead.
        // the stat ID to reference when updating
        public string ValueId { get; set; }
        public long Value { get; set; }
        public StatUpdateType Type { get; set; }

        public void Apply(ArcadeUser user)
        {
            long value = Value;

            if (Check.NotNull(ValueId))
            {
                value = user.GetStat(ValueId);
            }

            UpdateStat(user, Id, value, Type);
        }

        private void UpdateStat(ArcadeUser user, string a, long b, StatUpdateType type)
        {
            switch(type)
            {
                case StatUpdateType.Update:
                    user.UpdateStat(a, b);
                    break;
                case StatUpdateType.Set:
                    user.SetStat(a, b);
                    break;
                case StatUpdateType.SetIfGreater:
                    StatHelper.SetIfGreater(user, a, b);
                    break;
                case StatUpdateType.SetIfLesser:
                    StatHelper.SetIfLesser(user, a, b);
                    break;
                case StatUpdateType.SetIfEmpty:
                    StatHelper.SetIfEmpty(user, a, b);
                    break;
                default:
                    throw new Exception("Unknown stat update method");
            };
        }
    }
}