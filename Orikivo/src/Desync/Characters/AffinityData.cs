using Orikivo.Drawing;
using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a relationship for a specific <see cref="Npc"/>.
    /// </summary>
    public class AffinityData
    {
        public static AffinityLevel GetLevel(float value)
        {
            return (AffinityLevel)((int)MathF.Floor(RangeF.Convert(-1.0f, 1.0f, -3, 3, RangeF.Clamp(-1.0f, 1.0f, value))));
        }

        public AffinityData(string npcId, float value)
        {
            NpcId = npcId;
            Value = value;
        }
        public string NpcId { get; set; }

        public float Value { get; set; }
    }
}
