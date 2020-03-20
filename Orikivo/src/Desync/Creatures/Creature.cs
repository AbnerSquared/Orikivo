using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Desync
{

    //

    public class Creature
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Sprite Image { get; set; }

        public CreatureArchetype Archetype { get; set; }

        // make sure it relates to a creature tho
        public CatalogGenerator Table { get; set; }

        public CreatureAction GetNextAction(ActionLog interaction)
        {
            // TODO: Implement a simple system in which the creature makes a choice based on a personality
            
            // if the 
            throw new System.NotImplementedException();
        }
    }

}
