using System;

namespace Orikivo.Desync.Unstable
{
    // stores all possible branches
    // and generates trees accordingly
    public class DialogMap
    {
        // a collection of all possible dialog openers
        public DialogEntry[] Greetings;

        // a collection of all dialog closings
        public DialogEntry[] Farewells;

        // a collection of all possible dialogs
        // to use if the conversation times out
        public DialogEntry[] Timeout;

        // a collection of all possible branches.
        public DialogBranch[] Branches;
    }

    // trees are generated
    // no IDs are stored on a tree
    public class DialogTree
    {
        // determines if completed branches can be spoken again
        public bool AllowRepeats;

        // Who speaks first?
        public DialogSpeaker Speaker;

        // a collection of everything that can be said
        public DialogBranch[] Branches;
    }

    // stored in a dialog map
    public class DialogBranch
    {
        public string Id;

        // WHAT is this about?
        public DialogTopic Topic;

        // WHO would say this
        public DialogTailor Tailor;

        // WHO is this meant for
        public DialogRecipient Recipient;

        // the set of dialogs that is spoken
        public Dialog[] Dialogs;

        // determines if this branch can be randomly selected
        public bool IsGeneric;

        // all dialogs marked with the START marker
        // are the initial choices that can appear
        // if more than one START marker is specified
        // one of them is chosen at random

        // if a dialog has no ReplyReference values
        // that marks the END of a dialog branch

        // if a dialog has ReplyReference values
        // that marks the REPLY of a dialog branch
        // which keeps the branch going.

        // what happens at the end of this branch?
        public DialogResult Result;
    }

    // a specific collection of paragraphs
    public class Dialog
    {
        public string Id;
        // who this the speaker that uses this dialog?
        // if left empty, it will alternate.
        // this is ignored if using a dialog tree
        public DialogSpeaker Speaker;

        // how is this dialog perceived
        public DialogTone Tone;

        // a possible set of entries
        // one is chosen at random.
        public DialogEntry[] Entries;

        public DialogReply[] Replies;

        // what to append to the branches
        // main result
        public DialogResult Result;
    }

    // a specific paragraph
    public class DialogEntry
    {
        // only used for DialogMap.Specifics
        // is ignored for a Dialog class
        public string Id;

        // how often is this entry going to be chosen?
        // if left empty, it will equalize
        // with the remainder missing weight
        public float Weight;

        // everything that is spoken in this entry.
        public DialogNode[] Nodes;

        public DialogResult Result;
    }

    // a specific sentence
    public class DialogNode
    {
        // what is spoken
        public string Value;

        // how long this value is displayed for.
        public TimeSpan Duration;
    }


    // CUSTOMS
    public class DialogReply
    {
        // the dialog to point to
        public string Id;

        // the criteria needed in order to refer to this criteria
        public ReplyCriteria Criteria;
    }

    public class ReplyCriteria
    {
        // what has the user accomplished
        public string[] Flags;

        // how close is the user to this NPC
        public RelationshipLevel Relationship;
    }

    // represents the end result of a dialog branch.
    public class DialogResult
    {
        // what does this user receive?
        public Item[] Inbound;

        // what objectives does this user now have to do?
        public Objective[] Objectives;

        // what does this user give?
        public Item[] Outbound;

        // what is the value to append onto this current
        // relationship
        public float Impact;

        // where does this branch now go?
        // if left empty, it will return to all
        // branches set to initial for a dialog tree
        // if there is only one branch specified,
        // it will automatically roll over
        // and if there are no available dialog branches left
        // the npc will end the conversation.
        public string[] BranchIds;
    }

    // who might be the kind of person that speaks this dialog
    public class DialogTailor
    {
        // a reference to the place they work at
        public string MarketId;

        // what is the gender of the person that 
        // might use this
        public CharacterGender Gender;

        public MindType Mind;
        public EnergyType Energy;
        public NatureType Nature;
        public TacticType Tactic;
        public IdentityType Identity;
    }

    public class DialogRecipient
    {
        public RelationshipLevel Relationship;

        // a list of current objectives
        // the recipient might be doing
        public string[] ObjectiveIds;

        // everything the recipient has completed up to this
        // point
        public string[] FlagIds;
    }

    public class DialogTopic
    {
        // the general summary of this topic
        public TopicType Type;

        // a direct pointer to what the topic is
        // if none is set, it refers to its generic counterpart
        public string Id;
    }

    public enum TopicType
    {

        // talks about the time of day
        Time,

        // talks about a specific item
        Item,

        // talks about a specific location
        Location,

        // talks about a specific character
        Npc,

        // talks about something related to the story
        Story,

        // talks about an objective
        Objective
    }

    // who speaks this dialog
    public enum DialogSpeaker
    {
        // points this dialog for the user to say
        User = 1,

        // points this dialog for the NPC to say
        Npc = 2
    }

    // what this dialog references
    public enum DialogType
    {
        // marks the start of a dialog branch
        Start,

        // continues the current dialog branch
        Reply,

        // marks the end of a dialog branch
        End,
    }

    // what a dialog expresses
    public enum DialogTone
    {
        // no emotion
        Neutral,

        // happy from something
        Happy,

        // sad from something
        Sadness,

        // mad at something
        Anger,

        // scared of something
        Fear,

        // mourning over something
        Grief,

        // laughing at something
        Funny,

        // worried for something
        Worry,

        // :|
        // :)
        // :(
        // >:(
        // :0
    }

}
