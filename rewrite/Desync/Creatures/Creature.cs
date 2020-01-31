using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Creature
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Sprite Image { get; set; }

        // make sure it relates to a creature tho
        public List<ItemTag> LootTable { get; set; }

        public CreatureAction GetNextAction(Interaction interaction)
        {
            throw new System.NotImplementedException();
        }
    }

}
