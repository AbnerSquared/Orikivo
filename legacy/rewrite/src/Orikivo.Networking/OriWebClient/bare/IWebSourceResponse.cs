namespace Orikivo.Networking
{
    public interface IWebSourceResponse
    {
        bool IsSuccess { get; }
        string Content { get; }
    }
}
