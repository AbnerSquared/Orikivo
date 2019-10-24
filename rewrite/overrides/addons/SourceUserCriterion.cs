using Discord;
using System.Threading.Tasks;

namespace Orikivo
{
    public class SourceUserCriterion : IMessageCriterion<IMessage>
    {
        public Task<bool> JudgeAsync(OriCommandContext sourceContext, IMessage parameter)
        {
            bool isUser = sourceContext.User.Id == parameter.Author.Id;
            return Task.FromResult(isUser);
        }
    }
}
