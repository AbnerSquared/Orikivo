using System.Collections.Generic;

namespace Orikivo.Desync
{
    // adjust relationship level on the end of a dialog branch.
    public class DialogBranch
    {
        public string Id { get; set; }

        public List<Dialog> Dialogs { get; set; }

        // the topic
        public DialogTopic Topic { get; set; }

        // for the NPC
        public DialogTailor Tailor { get; set; }

        // for the user
        public DialogRecipient Recipient { get; set; }

        // how important this branch is for a user.
        public float Impact { get; set; }

        // TODO: Figure out a class that will update a user's objectives, or gifts
        // IF the dialog results in giving a gift to someone, etc.
    }

}
