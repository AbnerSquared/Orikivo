using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Unstable
{

    public enum CreatureArchetype
    {

        Neutral = 1, // a neutral creature, won't attack unless attacked
        Timid = 2, // a creature that tends to run away often
        Hostile = 3, // a creature that will attack every chance it gets
        Aggressive = 4, // a creature that is neutral, but will be ruthless when provoked
        Sturdy = 5, // a creature that won't do much, but can reach a breaking point given enough pushing
    }

    //

    public class Creature
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Sprite Image { get; set; }

        public CreatureArchetype Archetype { get; set; }

        // make sure it relates to a creature tho
        public LootTable Table { get; set; }

        public CreatureAction GetNextAction(ActionLog interaction)
        {
            // TODO: Implement a simple system in which the creature makes a choice based on a personality
            
            // if the 
            throw new System.NotImplementedException();
        }
    }

}
