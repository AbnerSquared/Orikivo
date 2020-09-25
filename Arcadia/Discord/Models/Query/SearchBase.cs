namespace Arcadia.Services
{
    public abstract class SearchBase<T>
    {
        public abstract ISearchResult<T> Search(ISearchQuery<T> query);

        public abstract string OnWriteResult(ISearchResult<T> result);

        public abstract string OnWriteElement(T element);
    }
}