using System;
using Orikivo;

namespace Arcadia.Multiplayer
{
    public class StatUpdatePacket
    {
        public StatUpdatePacket(string id, long value, StatUpdateType type = StatUpdateType.Add)
        {
            Id = id;
            Value = value;
            Type = type;
        }

        public StatUpdatePacket(string id, string valueId, StatUpdateType type = StatUpdateType.Add)
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
                value = user.GetVar(ValueId);
            }

            UpdateStat(user, Id, value, Type);
        }

        private void UpdateStat(ArcadeUser user, string a, long b, StatUpdateType type)
        {
            switch(type)
            {
                case StatUpdateType.Add:
                    user.AddToVar(a, b);
                    break;
                case StatUpdateType.Set:
                    user.SetVar(a, b);
                    break;
                case StatUpdateType.SetIfGreater:
                    Var.SetIfGreater(user, a, b);
                    break;
                case StatUpdateType.SetIfLesser:
                    Var.SetIfLesser(user, a, b);
                    break;
                case StatUpdateType.SetIfEmpty:
                    Var.SetIfEmpty(user, a, b);
                    break;
                default:
                    throw new Exception("Unknown stat update method");
            };
        }
    }
}