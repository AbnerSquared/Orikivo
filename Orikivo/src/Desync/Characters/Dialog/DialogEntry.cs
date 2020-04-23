using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class DialogEntry
    {
        public DialogEntry(params string[] content)
        {
            if (content.Length < 1)
                throw new ArgumentException("At least one value must be specified for a DialogEntry.");

            Content = content;
        }

        // for each string, is a continuation of this dialog.
        // there must be at LEAST 1 string minimum.
        public IEnumerable<string> Content { get; set; }

        // determines what the NPC needs to meet in order to use this dialog entry.
        public ReplyCriterion Criterion { get; set; }

        // TODO: add weight onto entries to give others a larger chance of occurring.
    }
}
