namespace Orikivo
{
    public struct ContextSearchResult
    {
        public static ContextSearchResult FromSuccess(IDisplayInfo value)
            => new ContextSearchResult { Value = value };
        public static ContextSearchResult FromError(ContextError? error, string reason = null)
            => new ContextSearchResult { Error = error, ErrorReason = reason };

        public IDisplayInfo Value { get; private set; }

        public ContextInfoType? Type => Value?.Type;

        public bool IsSuccess => !Error.HasValue;

        public ContextError? Error { get; private set; }

        public string ErrorReason { get; private set; }
    }
}
