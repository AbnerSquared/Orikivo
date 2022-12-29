using System.Threading.Tasks;

namespace Orikivo.Net
{
    public interface IHttpRateLimit
    {
        Task<bool> CanRequestAsync(HttpMethod requestType, string url);

        Task OnSuccessAsync(WebResult result);
    }
}
