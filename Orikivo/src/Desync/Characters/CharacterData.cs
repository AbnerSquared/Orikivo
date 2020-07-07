using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class CharacterData
    {
        // the ID of the character this data is for.
        public string Id { get; set; }

        // represents a bitwise set of flags
        // the this husk brain knows about the character
        public KnownFlag Flags { get; set; }

        // represents everything that the character is holding onto right now.
        public Backpack Backpack { get; set; }

        // list of raw relationships for other characters
        // based on the choices that a player has made.
        public Dictionary<string, float> Affinity { get; set; }

        // represents the current location of a character.
        public Locator Location { get; set; }

        // represents the current destination of a character
        // if the character is interrupted, generate a new destination to the same location
        // from their current position.
        public Destination Destination { get; set; }

        // if true, this character never appears anywhere else.
        public bool IsDead { get; set; }
    }
}
