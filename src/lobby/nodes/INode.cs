namespace Orikivo
{
    public interface INode
    {
        ulong? ReceiverId { get; set; }
        int? Index { get; set; }
        string Title { get; set; }
        string Content { get; }
        bool AllowMapping { get; }
        bool AllowFormatting { get; }
        
        string TitleMap { get; }
        string PropertyMap { get; }
        string ToString();
    }
}
