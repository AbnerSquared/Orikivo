namespace Orikivo.Desync
{
    public class CreatureAttributes
    {
        // represents a creature's health
        public int Health { get; set; }

        // represents a creature's defence
        public int Defense { get; set; }

        // represents an element immunity the creature has.
        public ElementType Immunity { get; set; }

        // represents the damage a creature inflicts.
        public int Damage { get; set; }
    }

}
