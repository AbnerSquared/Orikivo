using Arcadia.Multiplayer;

namespace Arcadia
{
    public class CriterionContext
    {
        internal CriterionContext(ArcadeUser user, GameResult result = null)
        {
            User = user;
            Result = result;
        }

        public ArcadeUser User { get; }
        public GameResult Result { get; }
    }
}
