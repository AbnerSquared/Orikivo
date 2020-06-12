namespace Arcadia.Old
{
    public interface IElementGroupConfig : IElementConfig
    {
        string ElementFormatter { get; }
        string ElementSeparator { get; }
    }
}
