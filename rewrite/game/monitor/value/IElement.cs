namespace Orikivo
{
    public interface IElement
    {
        string Id { get; }
        ElementType Type { get; }
        string Content { get; }
        string ContentFormatter { get; }
        string ToString();
    }
}
