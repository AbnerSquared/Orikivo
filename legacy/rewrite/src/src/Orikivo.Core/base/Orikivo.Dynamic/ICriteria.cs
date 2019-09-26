using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Orikivo.Dynamic
{
    public interface ICriteria<in T>
    {
        Task<bool> EnsureAsync(OrikivoCommandContext Context, T parameter);
    }

    public class Criteria<T> : ICriteria<T>
    {
        private List<ICriteria<T>> _criteria = new List<ICriteria<T>>();

        public Criteria<T> Add(ICriteria<T> criteria)
        {
            _criteria.Add(criteria);
            return this;
        }

        public async Task<bool> EnsureAsync(OrikivoCommandContext Context, T parameter)
        {
            foreach (ICriteria<T> criteria in _criteria)
            {
                bool result = await criteria.EnsureAsync(Context, parameter).ConfigureAwait(false);
                if (!result)
                    return false;
            }

            return true;
        }
    }

    public class EnsureUserCriteria : ICriteria<IMessage>
    {
        public Task<bool> EnsureAsync(OrikivoCommandContext Context, IMessage message)
        {
            bool equals = Context.User.Id == message.Author.Id;
            return Task.FromResult(equals);
        }
    }

    public class EnsureChannelCriteria : ICriteria<IMessage>
    {
        public Task<bool> EnsureAsync(OrikivoCommandContext Context, IMessage message)
        {
            bool equals = Context.Channel.Id == message.Channel.Id;
            return Task.FromResult(equals);
        }
    }

    public class EmptyCriteria<T> : ICriteria<T>
    {
        public Task<bool> EnsureAsync(OrikivoCommandContext Context, T parameter)
            => Task.FromResult(true);
    }

    public class EnsureReactionFromUserCriteria : ICriteria<SocketReaction>
    {
        public Task<bool> EnsureAsync(OrikivoCommandContext Context, SocketReaction parameter)
        {
            bool result = parameter.UserId == Context.User.Id;
            return Task.FromResult(result);
        }
    }
}
