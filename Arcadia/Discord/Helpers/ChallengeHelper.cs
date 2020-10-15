using System.Collections.Generic;
using System.Linq;
using Orikivo;

namespace Arcadia
{
    public static class ChallengeHelper
    {
        public static readonly int SetSize = 3;

        public static IEnumerable<Challenge> GetChallengeSet()
        {
            return Randomizer.ChooseMany(Assets.Challenges, SetSize);
        }

        // New challenges cannot be given until the entire set is completed.

        public static void JudgeChallenges(ArcadeUser user)
        {

        }
    }
}
