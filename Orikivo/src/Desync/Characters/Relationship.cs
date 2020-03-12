using Orikivo.Drawing;
using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a relationship for a specific <see cref="Npc"/>.
    /// </summary>
    public class Relationship
    {
        public static RelationshipLevel GetLevel(float value)
        {
            return (RelationshipLevel)((int)MathF.Floor(RangeF.Convert(-1.0f, 1.0f, -1, 6, RangeF.Clamp(-1.0f, 1.0f, value))));
        }

        public Relationship(string npcId, float value)
        {
            NpcId = npcId;
            Value = value;
        }
        public string NpcId { get; set; }
        // a value from -1.0 to 1.0 that defines the current relationship level.
        // 0.8 is max for best friends?
        public float Value { get; set; }
    }

    public enum RelationshipLevel
    {
        Dispised = -1,
        Hated = 0,
        Disliked = 1,
        Acquaintance = 2,
        Friend = 3,
        GoodFriend = 4,
        BestFriend = 5,
        Loved = 6
    }
}
