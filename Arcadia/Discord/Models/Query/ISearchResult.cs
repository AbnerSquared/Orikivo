using System.Collections.Generic;

namespace Arcadia.Services
{
    public interface ISearchResult<out T>
    {
        IEnumerable<T> Result { get; }
    }
}