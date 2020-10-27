using System;
using System.Collections.Generic;

namespace Arcadia
{
    public class ItemPropertyData
    {
        internal ItemPropertyData(long value, List<ModifierData> modifiers, DateTime? expiresOn)
        {
            Value = value;
            Modifiers = modifiers;
            ExpiresOn = expiresOn;
        }

        public long Value { get; internal set; }

        // A collection of temporary modifiers for the specified property (eg. +3 ATTACK for 20 uses)
        public List<ModifierData> Modifiers { get; }

        public DateTime? ExpiresOn { get; }
    }
}
