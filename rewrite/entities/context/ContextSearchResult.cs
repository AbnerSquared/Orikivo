namespace Orikivo
{
    public struct ContextSearchResult
    {
        // the set accessors should be private/internal
        public ModuleDisplayInfo Group { get; internal set; }
        public ModuleDisplayInfo Module { get; internal set; }
        public CommandDisplayInfo Command { get; internal set; }
        public OverloadDisplayInfo Overload { get; internal set; }
        public ParameterDisplayInfo Parameter { get; internal set; }
        public ContextInfoType? ResultType { get; internal set; }
        public bool IsSuccess { get; internal set; }
        public ContextError? ErrorReason { get; internal set; }
        public string GetResultInfo() // change to .Result
        {
            switch(ResultType)
            {
                case ContextInfoType.Module:
                    return Module.GetDisplay();
                case ContextInfoType.Group:
                    return Group.GetDisplay();
                case ContextInfoType.Command:
                    return Command.GetDisplay();
                case ContextInfoType.Overload:
                    return Overload.GetDisplay();
                case ContextInfoType.Parameter:
                    return Parameter.GetDisplay();
                default:
                    return null;
            }
        }
    }
}
