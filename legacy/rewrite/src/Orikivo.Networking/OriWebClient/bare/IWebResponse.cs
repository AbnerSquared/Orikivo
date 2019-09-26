using System.Net.Http;

namespace Orikivo.Networking
{
    public interface IWebResponse
    {
        bool IsSuccess { get; }
        HttpContent Result { get; }
    }

    public interface IWebResponse<T>
    {
        bool IsSuccess { get; }
        string Content { get; }
        T Data { get; }
    }
}
