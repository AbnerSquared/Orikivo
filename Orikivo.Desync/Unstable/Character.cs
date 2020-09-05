using Orikivo.Drawing;
using System;

namespace Orikivo.Desync.Unstable
{
    // TODO: auto-close conversations when
    // a character has to go to a new location

    // TODO: auto-close conversations when
    // a vendor has to close their store
    // However, they should allow the customer to finish
    // their current transaction before closing.

    // represents a specific character in this world
    public class Character
    {

        // how does this character think
        // and enjoy the world?
        public CharacterPersonality Personality;

        // what can this NPC gift
        public Item[] Gifts;

        // who does this NPC know?
        public Relationship[] Relationships;

        // what is the story for this character?
        public CharacterArc Arc;

        // what does this character do on a daily basis
        public Routine Routine;

        // where does this character live.
        public Construct Home;
    }

    // represents a relationship to a specific
    // character
    public class Relationship
    {
        // who is the recipient of this relationship
        public string CharacterId;

        // what is their relationship at
        public float Value;
    }

    // defines a rough basis of a relationship level.
    public enum RelationshipLevel
    {
        Stranger = 0,
        Acquaintance = 1
    }
    public class CharacterExpression
    {
        // the tone that this is meant for
        public DialogTone Tone;

        // the image that this character's face is
        public AppearanceNode Value;
    }

    // a set of clothing this character might wear
    public class CharacterOutfit
    {
        public string Id;

        // what goes in front of their head
        public AppearanceNode HeadOverlay;

        // what goes in front of their torso
        public AppearanceNode TorsoOverlay;
    }

    // defines how a character acts
    public class CharacterPersonality
    {
        public MindType Mind;
        public EnergyType Energy;
        public NatureType Nature;
        public TacticType Tactic;
        public IdentityType Identity;

        // what does this NPC love
        public Item[] LovedItems;

        // what does this NPC like
        public Item[] LikedItems;

        // what does this NPC hate
        public Item[] DislikedItems;

        // how does this NPC handle specific tones
        public ToneImpact[] Impact;
    }

    public class ToneImpact
    {
        // what are the tones that this impact is for?
        public DialogTone Tone;

        // how strong a specific tone impacts an npc
        public float Impact;
    }


    // STORY

    // the overall story for a character
    public class CharacterArc
    {
        // what is needed to start this character
        // arc
        public StoryCriteria Criteria;

        // the segments of this story.
        public StoryChapter[] Chapters;

        // what happens at the end of this 
        // character arc.
        public StoryResult Result;
    }

    public class StoryCriteria : UnlockCriteria
    {
        // the level of relationship needed
        public RelationshipLevel Relationship;
    }

    // a specific chapter for a character
    public class StoryChapter
    {
        // a collection of objectives for this chapter
        public Objective[] Objectives;

        // what is needed to start this chapter
        public StoryCriteria Criteria;

        public StoryResult Result;
    }

    // what is updated upon the completion of this chapter.
    public class StoryResult
    {
        // the event flag to raise
        // when this chapter was completed
        public string FlagId;

        // all of the relationships this NPC
        // now has
        public Relationship[] Relationships;

        // the NPC's new routine
        public Routine Routine;
    }

    public enum RoutineSortOrder
    {
        // cycles through each routine once
        Cycle,

        // cycles through each routine randomly
        // until none are left
        Shuffle,

        // randomly chooses a routine each time
        Random
    }

    public class CharacterMood
    {
        // a set of new tone impacts for this mood
        public ToneImpact[] Impact;

        // what this mood denies execution of
        public MoodDeny Deny;
    }

    // a set of actions a user
    // cannot executed
    [System.Flags]
    public enum MoodDeny
    {
        // no talking
        Talk,

        // no gifting
        Gift,

        // no objectives
        Objective
    }

    // represents what a character does in a world
    public class Routine
    {
        // how this routine is set up for a character
        public RoutineSortOrder SortOrder;

        // a collection of daily routines
        public RoutineEntry[] Entries;
    }

    // represents what this character does
    // in a day
    public class RoutineEntry
    {
        // a set of everything this character does in a day.
        public RoutineNode[] Nodes;
    }

    public class RoutineNode : PathNode
    {
        // does this character instantly get to this location?
        public bool Instant;

        // how long can this character be stopped from reaching
        // this location? if none is set, the character can be
        // stopped for as long as possible.

        // a character is stopped if someone begins talking
        // to them.

        // a character CANNOT talk if they are in a form
        // of transportation that's not walking
        public TimeSpan MaxStopTime;
        // if set to zero, this character cannot talk

        // how long does this character stay at this location?
        // if a player talks to an NPC, the time they stay
        // is subtracted from this duration
        // if the next node start before they reach this one
        // they will proceed to travel to the next location.

        // the exact hour and minute they should be here

        public int StartingHour;
        public int StartingMinute;
        

        // a Path has to be set up to get to each location
        // they have to listen to barriers, routes
        // and other forms to handle such.
        public TimeSpan Duration;

        // what is this character doing
        public CharacterAction Action;

        // how does this character feel right now?
        // if none are set, it simply refers to their
        // default personality.
        public CharacterMood Mood;
    }

    // what is a character doing?
    public enum CharacterAction
    {
        // this character is idling around
        Idle,
        
        // this character is working
        // if this is active AND
        // the ID set is at a Market
        // the market will be open.
        Work,

        // this character is sleeping
        Sleep,

        // this character is doing something
        Busy
    }
}
