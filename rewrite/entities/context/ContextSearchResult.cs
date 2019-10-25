namespace Orikivo
{
    public struct ContextSearchResult
    {
        public IDisplayInfo Result { get; internal set; }

        public ContextInfoType? Type => Result?.Type;

        public bool IsSuccess => !Error.HasValue;

        public ContextError? Error { get; internal set; }
    }
}
