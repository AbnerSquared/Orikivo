using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    public class Criteria<T> : ICriterion<T>
    {
        private List<ICriterion<T>> _criteria = new List<ICriterion<T>>();
        public Criteria<T> AddCriterion(ICriterion<T> criterion)
        {
            _criteria.Add(criterion);
            return this;
        }
        public async Task<bool> JudgeAsync(OriCommandContext sourceContext, T parameter)
        {
            foreach (ICriterion<T> criterion in _criteria)
            {
                // check through each criterion to see if it's true
                bool result = await criterion.JudgeAsync(sourceContext, parameter).ConfigureAwait(false);
                if (!result)
                    return false;
            }
            return true;
        }
    }
}
