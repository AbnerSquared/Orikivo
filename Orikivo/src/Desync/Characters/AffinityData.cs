using Orikivo.Drawing;
using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a relationship for a specific <see cref="Desync.Character"/>.
    /// </summary>
    public class AffinityData
    {
        public static AffinityLevel GetLevel(float value)
        {
            return (AffinityLevel)((int)MathF.Floor(RangeF.Convert(-1.0f, 1.0f, -3, 3, RangeF.Clamp(-1.0f, 1.0f, value))));
        }

        public AffinityData(string characterId, float value)
        {
            CharacterId = characterId;
            Value = value;
        }

        public string CharacterId { get; set; }

        public float Value { get; set; }
    }
}
