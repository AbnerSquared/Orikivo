using System.Collections.Generic;
using System.Linq;

namespace Orikivo

{
    public class MeritCollection
    {
        public MeritCollection()
        {
            Claimed = new List<ClaimMerit>();

        }
        public List<ClaimMerit> Claimed { get; }

        public void Log(Merit merit)
        {
            if (!merit.EqualsAny(Claimed.Enumerate(x => x.Source).ToArray()))
                Claimed.Add(new ClaimMerit(merit));
        }
    }
}
