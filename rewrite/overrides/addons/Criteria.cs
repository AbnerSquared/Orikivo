using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    // TODO: Modify the criteria accordingly to exist for Discord.
    public class MessageCriteria<T> : IMessageCriterion<T>
    {
        private List<IMessageCriterion<T>> _criteria = new List<IMessageCriterion<T>>();
        public MessageCriteria<T> AddCriterion(IMessageCriterion<T> criterion)
        {
            _criteria.Add(criterion);
            return this;
        }
        public async Task<bool> JudgeAsync(OriCommandContext sourceContext, T parameter)
        {
            foreach (IMessageCriterion<T> criterion in _criteria)
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
