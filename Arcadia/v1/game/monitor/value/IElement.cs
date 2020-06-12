namespace Arcadia.Old
{
    public interface IElement
    {
        string Id { get; }
        ElementType Type { get; }
        int Priority { get; }
        string Content { get; }
        string ContentFormatter { get; }
        string ToString();
    }
}
