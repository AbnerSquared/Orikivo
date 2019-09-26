using System.Net.Http;

namespace Orikivo.Networking
{
    public class WebResponse : IWebResponse
    {
        public WebResponse(HttpResponseMessage response)
        {
            IsSuccess = response.IsSuccessStatusCode;
            Result = response.Content;
        }

        public bool IsSuccess { get; internal set; }
        public HttpContent Result { get; internal set; }
    }

    public class WebResponse<T> : IWebResponse<T>
    {
        public WebResponse(HttpResponseMessage response)
        {
            IsSuccess = response.IsSuccessStatusCode;
            Content = response.Content.ReadAsStringAsync().Result;
            Data = Manager.Deserialize<T>(Content);
        }

        public bool IsSuccess { get; internal set; }
        public string Content { get; internal set; }
        public T Data { get; internal set; }
    }
}
