using Discord;
using System.Threading.Tasks;

namespace Orikivo
{
    public class SourceChannelCriterion : IOriCriterion<IMessage>
    {
        public Task<bool> JudgeAsync(OriCommandContext sourceContext, IMessage parameter)
        {
            bool isChannel = sourceContext.Channel.Id == parameter.Channel.Id;
            return Task.FromResult(isChannel);
        }
    }
}
