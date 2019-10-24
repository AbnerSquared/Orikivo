using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public interface IHttpRateLimit
    {
        Task<bool> CanRequestAsync(HttpMethod requestType, string url);
        Task OnSuccessAsync(OriWebResult result);
    }
}
