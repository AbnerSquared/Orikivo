using System;
using System.Threading.Tasks;

namespace Orikivo.Networking
{
    public interface IOriWebClient
    {
        Task<WebResponse<T>> RequestAsync<T>(string url);
        Task<WebResponse<T>> RequestAsync<T>(Uri uri);
    }
}