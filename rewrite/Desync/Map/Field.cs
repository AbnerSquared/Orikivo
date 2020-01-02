using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Field // a dangerous location
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }

        public List<CreatureTag> CreatureTable { get; set; }
    }

    public enum CreatureTag
    {
        
    }

    public enum LootTag
    {

    }

    // a creature that may contain loot and such
    public class Creature
    {
        public List<LootTag> LootTable { get; set; }
    }
}
