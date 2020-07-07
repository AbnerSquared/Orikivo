using System.Collections.Generic;

namespace Orikivo.Desync
{

    /// <summary>
    /// Represents a non-playable entity that lives in a <see cref="World"/>.
    /// </summary>
    public class Character
    {
        // the ID of the world they live in.
        public string WorldId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        // if unset, use they/them pronouns
        public CharacterGender Gender { get; set; }

        // this defines how the NPC thinks
        public Personality Personality { get; set; }

        // this is what the NPC looks like
        public CharacterModel Model { get; set; }
        
        // this is who the NPC likes/dislikes initially
        public List<AffinityData> Affinity { get; set; }

        // The NPC will cycle through each routine.
        // The routine takes effect on the first day at which the user awakens
        // the routine progress is then determined based on their starting time in UTC.

        // Likewise, if the routine isn't a daily basis, it starts once the user awakens
        /// <summary>
        /// Represents a <see cref="Character"/>'s set of tasks. If unspecified, the character will remain home.
        /// </summary>
        public Routine Routine { get; set; }

        // represents the character's default location
        // used if a routine is unspecified.
        public Locator DefaultLocation { get; set; }
    }
}
