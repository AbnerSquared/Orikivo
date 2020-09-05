using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    public class DialogBranch
    {
        public DialogBranch() { }

        public DialogBranch(string id, DialogUsage usage = DialogUsage.Always)
        {
            Id = id;
            Usage = usage;
        }

        public string Id { get; set; }

        // if false, once this branch has been used, it cannot be used again.
        public DialogUsage Usage { get; set; } = DialogUsage.Always;

        public List<Dialog> Values { get; set; } = new List<Dialog>();

        public Dialog GetDialog(string id)
        {
            return Values.First(x => x.Id == id);
        }

        // gets the list of starting dialogs to choose from based on the npc and player.
        public IEnumerable<Dialog> GetEntryDialogs(Character npc, Husk husk, HuskBrain brain, ChatLog log)
        {
            return Values.Where(x => x.Type == DialogType.Initial);
        }

        // gets the best reply for a dialog based on the criteria met for an NPC.
        // and the current dialog given.
        public Dialog GetBestReply(Character character, Husk husk, HuskBrain brain, ChatLog log, Dialog dialog)
        {
            var bestReplys = GetAvailableReplys(character, husk, brain, log, dialog)
                .GroupBy(x => x.Criterion?.Priority ?? 0)
                .OrderByDescending(x => x.Key);

            if (bestReplys?.Count() > 0)
                return Randomizer.Choose(bestReplys.First());

            return null; // there isn't a reply.
        }

        // gets a collection of available replies for the player to use based
        // on the criteria met.
        // Chat logs store all used dialog IDs already. If CanRepeat is false on the specified ID
        // it can no longer be referenced.
        public IEnumerable<Dialog> GetAvailableReplys(Character character, Husk husk, HuskBrain brain, ChatLog log, Dialog dialog)
        {
            foreach (string replyId in dialog.ReplyIds)
            {
                var reply = GetDialog(replyId);

                if (reply.Criterion?.Judge.Invoke(character, husk, brain) ?? true)
                    yield return reply;
            }
        }
    }

}
