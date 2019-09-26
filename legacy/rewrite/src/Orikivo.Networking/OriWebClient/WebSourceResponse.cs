using System.Net.Http;

namespace Orikivo.Networking
{
    public class WebSourceResponse : IWebSourceResponse
    {
        public WebSourceResponse(HttpResponseMessage response)
        {
            IsSuccess = response.IsSuccessStatusCode;
            Content = response.Content.ReadAsStringAsync().Result;
        }

        public bool IsSuccess { get; internal set; }
        public string Content { get; internal set; }
    }
}
