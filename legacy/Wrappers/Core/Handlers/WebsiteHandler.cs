using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Orikivo.Systems.Wrappers.Core.Interfaces;

namespace Orikivo.Systems.Wrappers.Core.Handlers
{
    internal class WebsiteHandler : IWebsiteHandler
    {
        public async Task<ReturnData> GetUri(Uri uri)
        {
            Console.WriteLine($"{uri.ToString()}");
            var http = new HttpClient();
            var userAgent = new ProductInfoHeaderValue("Orikivo", "1.0");
            http.DefaultRequestHeaders.UserAgent.Add(userAgent);

            ReturnData attempt = new ReturnData(false, "");

            var site = await http.GetAsync(uri);
            var siteContent = await site.Content.ReadAsStringAsync();
            attempt.IsSuccess = site.IsSuccessStatusCode;
            attempt.JsonData = siteContent;
            Console.WriteLine($"{attempt.IsSuccess}\n{attempt.JsonData}");
            return attempt;
        }
    }
}