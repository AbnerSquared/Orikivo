using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // a dialog branch can be grouped by:
    // - the personality it tailors to
    // - the relationship level
    // - the npc's current mood

    // if a user has this Item: have the NPC use the ability to talk about it
    // use TimeCycle in the DesyncEngine to handle when it is daytime, nighttime, etc.

    // Modify the Schedule to implement TimeCycle variables to make it easier to set up a Shift

    // create dialog relating to an Item
    // create dialog relating to a Location
    // create dialog relating to an ItemTag
    // create dialog relating to an Npc
    // create dialog relating to an Objective
    // create dialog relating to you
    // create dialog relating to work
    // create dialog relating to the story
    // create dialog relating to their mood
    // create dialog relating for the morning
    // create dialog relating for the day
    // create dialog relating for the evening
    // create dialog relating for the night

    // A Dialog can be grouped by a Topic, which is what generalizes what the Dialog is about
    // A Dialog can be tailored by a Tailor, which is what generalizes the kind of character that will use this Dialog

    // the dialog that this tailors to.
    // if this is true, this dialog can be used.

    public class DialogTopic
    {
        string ItemId { get; set; }
        string LocationId { get; set; }

        ItemTag Item { get; set; }
    }

    // generalizes the person that can receive this Dialog. If this criteria is not met, the dialog cannot be used.
    public class DialogRecipient
    {
        // this is the relationship level this user has to be with this NPC.
        public RelationshipLevel Level { get; set; }


        // the person receiving this has to have these specified flags
        public List<string> Flags { get; set; }
    }

    // Topic is what stores what the dialog is about

    // Listener stores who the player is that is eligble to receive this dialog
    // this can set required flags, relationship levels, etc.

    // tailor only stores how an NPC speaks
    public class DialogTailor
    {
        MindType? Mind { get; set; }
        EnergyType? Energy { get; set; }

        NatureType? Nature { get; set; }

        TacticType? Tactic { get; set; }
        IdentityType? Identity { get; set; }
    }

    /*
             // tailors this dialog to people that have these flags
        // flags are markers that a player has completed a specific objective or storyline.
        List<string> Flags { get; set; }
         
         
         */

    // this stores a dialogue that has multiple sentences
    public class DialogNode
    {
        public string Entry { get; set; }

        // if none is set, default to Dialogue.Duration
        public TimeSpan Duration { get; set; }
    }

    // this stores only one option
    public class DialogEntry
    {
        public List<DialogNode> Nodes { get; set; }
    }


    // make sure that dialog branches can't be repeated, and if there are no new topics,
    // prevent communication with the NPC.
    public class DialogTree
    {
        public string Id { get; set; }
        // this represents all of the possible dialog routes that can be taken
        public List<DialogBranch> Branches { get; set; }

        // what will be spoken if the chat handler times out.
        public string OnTimeout { get; set; }

        // determines if this dialog can be randomly selected or not.
        public bool IsGeneric { get; set; }
    }

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

    public class Dialog
    {
        // if left null, it will instead be referenced by BranchId#index
        public string Id { get; set; }

        // determines how a conversation continues.
        public DialogueType Type { get; set; }

        // handles how an NPC will respond and appear
        public DialogueTone Tone { get; set; }

        public DialogEntry Entry => Entries.First();
        // only one of these nodes are randomly chosen
        public List<DialogEntry> Entries { get; set; }
    }

    /// <summary>
    /// Represents a conversation node, usually for an <see cref="Npc"/> or <see cref="Vendor"/>.
    /// </summary>
    public class Dialogue
    {
        /// <summary>
        /// A unique identifier for this <see cref="Dialogue"/>.
        /// </summary>
        public string Id { get; set; }

        public string Entry => Entries.First();

        /// <summary>
        /// Represents a collection of sentences that can be used when this <see cref="Dialogue"/> is called. The first value specified is considered the main entry.
        /// </summary>
        public List<string> Entries { get; set; }

        // TODO: Implement auto duration
        public TimeSpan AutoDuration { get; set; }

        /// <summary>
        /// Determines how this <see cref="Dialogue"/> is handled.
        /// </summary>
        public DialogueType Type { get; set; }

        /// <summary>
        /// Determines how an <see cref="Npc"/> receives this <see cref="Dialogue"/> (user-side) or how an <see cref="Npc"/> will look when speaking (client-side).
        /// </summary>
        public DialogueTone Tone { get; set; }

        /// <summary>
        /// Determines how strong this <see cref="Dialogue"/> affects an <see cref="Npc"/>.
        /// </summary>
        public float Importance { get; set; }

        /// <summary>
        /// A collection of possible <see cref="Dialogue"/> values that can be used in response to this <see cref="Dialogue"/>.
        /// </summary>
        public List<string> ReplyIds { get; set; }

        // if none is set, ignore.
        public DialogueCriterion Criterion { get; set; }

        /// <summary>
        /// Returns the best matching reply ID based on a <see cref="Personality"/> and <see cref="Relationship"/>.
        /// </summary>
        public string GetBestReplyId(Archetype personality)
        {
            // TODO: Create a system for choosing best replies.
            return Randomizer.Choose(ReplyIds);
        }

        /// <summary>
        /// Returns a random <see cref="string"/> that represents what will be spoken.
        /// </summary>
        public string NextEntry()
            => Randomizer.Choose(Entries);
    }

}
