using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    public class Dialogue
    {
        public string Id { get; set; }

        public string Entry => Entries.First();

        public List<string> Entries { get; set; }

        // TODO: Implement auto duration
        public TimeSpan AutoDuration { get; set; }

        public DialogType Type { get; set; }

        public DialogTone Tone { get; set; }

        public float Importance { get; set; }

        public List<string> ReplyIds { get; set; }

        // if none is set, ignore.
        public DialogCriterion Criterion { get; set; }

        public string GetBestReplyId(Personality personality)
        {
            // TODO: Create a system for choosing best replies.
            return Randomizer.Choose(ReplyIds);
        }

        public string NextEntry()
            => Randomizer.Choose(Entries);
    }

}
