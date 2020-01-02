using System;

namespace Orikivo
{
    public struct ContextSearchResult
    {
        private ContextSearchResult(IDisplayInfo value = null, ContextError? error = null, string errorReason = null)
        {
            Value = value;
            Error = error;
            ErrorReason = errorReason;
        }

        public static ContextSearchResult FromSuccess(IDisplayInfo value)
            => new ContextSearchResult(value);
        public static ContextSearchResult FromError(ContextError? error, string reason = null)
            => new ContextSearchResult(null, error, reason);

        public IDisplayInfo Value { get; private set; }

        public ContextInfoType? Type => Value?.Type;

        public bool IsSuccess => !Error.HasValue;

        public ContextError? Error { get; private set; }

        public string ErrorReason { get; private set; }
    }
}
