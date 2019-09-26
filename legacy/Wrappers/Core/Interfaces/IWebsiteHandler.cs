using System;
using System.Threading.Tasks;
using Orikivo.Systems.Wrappers.Core;

namespace Orikivo.Systems.Wrappers.Core.Interfaces
{
    public interface IWebsiteHandler
    {
        Task<ReturnData> GetUri(Uri uri);
    }
}