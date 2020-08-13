using System;

namespace Arcadia
{
    [Flags]
    public enum ItemTag
    {
        Palette = 1,
        Summon = 2,
        Font = 4,
        Capsule = 8,
        Tool = 16,
        Decorator = 32,
        Automaton = 64,
        Booster = 128
    }
}
