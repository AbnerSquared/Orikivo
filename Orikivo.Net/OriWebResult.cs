using Newtonsoft.Json;
using System.Net.Http;

namespace Orikivo.Net
{
    public class OriWebResult
    {
        public OriWebResult(HttpResponseMessage response)
        {
            IsSuccess = response.IsSuccessStatusCode;
            Content = response.Content;
        }

        public bool IsSuccess { get; internal set; }

        public HttpContent Content { get; internal set; }

        public string RawContent => Content.ReadAsStringAsync().Result;
    }

    public class OriWebResult<T>
    {
        public OriWebResult(HttpResponseMessage response)
        {
            IsSuccess = response.IsSuccessStatusCode;
            Content = response.Content;
            Result = JsonConvert.DeserializeObject<T>(RawContent);
        }

        public bool IsSuccess { get; internal set; }

        public HttpContent Content { get; internal set; }

        public string RawContent => Content.ReadAsStringAsync().Result;

        public T Result { get; internal set; }
    }
}
