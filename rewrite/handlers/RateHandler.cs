using System;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Handles how rate modifiers affect existing rates.
    /// </summary>
    public class RateHandler
    {
        public RateHandler() => throw new NotImplementedException();
        
        public double GetExpRate(OriUser user)
        {
            double boosterRate = 0.0;
            user.Boosters.Where(x => x.Key == GenericBooster.Exp && !x.Value.IsExpired).ToList().ForEach(x => boosterRate += x.Value.GainRate);
            return 1.0 * boosterRate;
        }

        public double GetMoneyRate(OriUser user)
        {
            double boosterRate = 0.0;
            user.Boosters.Where(x => x.Key == GenericBooster.Money && !x.Value.IsExpired).ToList().ForEach(x => boosterRate += x.Value.GainRate);
            return 1.0 * boosterRate;
        }
    }
}
