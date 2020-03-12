namespace Orikivo
{
    public interface IElementGroupConfig : IElementConfig
    {
        string ElementFormatter { get; }
        string ElementSeparator { get; }
    }
}
