using System.Collections.Generic;

namespace Arcadia.Services
{
    public interface ISearchQuery<T>
    {
        string Query { get; }
    }

    // This could possibly be used instead so that it's easier to filter out stuff
    public abstract class SearchBase<T>
    {
        public abstract ISearchResult<T> Search(ISearchQuery<T> query);

        public abstract string OnWriteResult(ISearchResult<T> result);

        public abstract string OnWriteElement(T element);
    }

    public interface ISearchResult<out T>
    {
        IEnumerable<T> Result { get; }
    }

    public enum LeaderboardQuery
    {
        Default = 0,
        Money = 1,
        Debt = 2,
        Level = 3,
        Chips = 4,
        Merits = 5,
        Custom = 6 // This allows for leaderboards by stats
    }
}