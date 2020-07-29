using System;
using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    // add custom criterion for an ArcadeUser needed to start a game.
    // this is the result of a GameSession that is used to update EXP, give rewards, etc.
    public class SessionResult
    {
        // the list of user IDs to update.
        public List<ulong> UserIds { get; set; } = new List<ulong>();
        public Dictionary<ulong, long> Money { get; } = new Dictionary<ulong, long>();
        public Dictionary<ulong, ulong> Exp { get; } = new Dictionary<ulong, ulong>();
        public Dictionary<ulong, List<StatUpdatePacket>> Stats { get; } = new Dictionary<ulong, List<StatUpdatePacket>>();
        public Dictionary<ulong, List<ItemUpdatePacket>> Items { get; } = new Dictionary<ulong, List<ItemUpdatePacket>>();

        // Experience
        // Rewards
        // Stats
        // Money

        public void Apply(ArcadeContainer container)
        {
            foreach (ulong userId in UserIds)
            {
                if (!container.Users.TryGet(userId, out ArcadeUser user))
                    continue;

                if (Money.ContainsKey(userId))
                    user.Give(Money[userId]);

                // NOTE: No exp yet, formulas not implemented

                if (Stats.ContainsKey(userId))
                    foreach (StatUpdatePacket packet in Stats[userId])
                        packet.Apply(user);

                if (Items.ContainsKey(userId))
                    foreach (ItemUpdatePacket packet in Items[userId])
                        ItemHelper.GiveItem(user, packet.Id, packet.Amount);

                container.Users.AddOrUpdate(userId, user);
            }
        }
    }

    public class ItemUpdatePacket
    {
        public ItemUpdatePacket(string id, int amount = 1)
        {
            Id = id;
            Amount = amount;
        }

        public string Id { get; set; }
        public int Amount { get; set; }
    }

    // TODO: implement a reserved set of stats that cannot be changed.
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

    public enum StatUpdateType
    {
        Set = 1,
        Update = 2,
        SetIfGreater = 3,
        SetIfLesser = 4,
        SetIfEmpty = 5
    }
}
