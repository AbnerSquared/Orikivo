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

}
