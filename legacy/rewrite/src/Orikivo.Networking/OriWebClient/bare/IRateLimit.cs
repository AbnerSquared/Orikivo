using System.Threading.Tasks;

namespace Orikivo.Networking
{
    public interface IRateLimit
    {
        Task<bool> CanRequestAsync(HttpRequestType method, string url);
        Task OnSuccessAsync(WebResponse response);
    }
}