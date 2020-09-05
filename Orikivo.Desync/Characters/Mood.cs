using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class Mood
    {
        // an optional list of modifiers
        public List<ToneModifier> Modifiers { get; set; }

        public MoodDeny Deny { get; set; }
    }
}
