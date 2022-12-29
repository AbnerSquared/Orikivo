using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Net
{
    public class WebClient : IDisposable
    {
        public static WebClient Default = new WebClient();

        private static HttpClient DefaultHttpClient
        {
            get
            {
                HttpClient client = new HttpClient();
                return client;
            }
        }

        internal HttpClient _client;
        internal IHttpRateLimit _rateLimit;
        internal bool _ensured;

        public HttpRequestHeaders Headers => _client.DefaultRequestHeaders;

        public WebClient()
        {
            _client = DefaultHttpClient;
        }

        public WebClient(string address) : this()
        {
            SetBaseAddress(address);
        }

        public WebClient(string address, bool ensured) : this(address)
        {
            _ensured = ensured;
        }

        public void SetBaseAddress(string address)
            => _client.BaseAddress = new Uri(address);

        public void SetAuthorization(string scheme)
            => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme);

        public void SetAuthorization(string scheme, string parameter)
            => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);

        public void AddRequestHeader(string header, string value)
            => _client.DefaultRequestHeaders.Add(header, value);

        public async Task<WebResult<T>> RequestAsync<T>(string url)
            => await RequestAsync<T>(new Uri(url));

        public async Task<WebResult<T>> RequestAsync<T>(Uri uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            return new WebResult<T>(response);
        }

        public async Task<WebResult> RequestAsync(string url)
            => await RequestAsync(new Uri(url));

        public async Task<WebResult> RequestAsync(Uri uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            return new WebResult(response);
        }

        public async Task<WebResult> GetAsync(string url = "")
            => await SendAsync(HttpMethod.GET, url);

        public async Task<WebResult> DeleteAsync(string url = "")
            => await SendAsync(HttpMethod.DELETE, url);

        public async Task<WebResult> PostAsync(string url = "", string value = null)
            => await SendAsync(HttpMethod.POST, url, value);

        public async Task<WebResult> PatchAsync(string url = "", string value = null)
            => await SendAsync(HttpMethod.PATCH, url, value);

        public async Task<WebResult> PutAsync(string url = "", string value = null)
            => await SendAsync(HttpMethod.PUT, url, value);

        public async Task<WebResult> SendAsync(HttpMethod requestType, string url = "", string value = null)
        {
            url.TrimStart('/');
            System.Net.Http.HttpMethod method = new System.Net.Http.HttpMethod(requestType.ToString());
            
            using (HttpRequestMessage request = new HttpRequestMessage(method, url))
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    request.Content = new StringContent(value, Encoding.UTF8, $"application/{HttpMedia.JSON.ToString().ToLower()}");
                    request.Content.Headers.ContentType.CharSet = null;
                }

                return await SendAsync(request);
            }
        }

        public async Task<WebResult> SendAsync(HttpRequestMessage request)
        {
            HttpResponseMessage response = await _client.SendAsync(request);

            if (_rateLimit != null)
            {
                if (!await _rateLimit.CanRequestAsync((HttpMethod)Enum.Parse(typeof(HttpMethod), request.Method.Method.ToUpper()), request.RequestUri.ToString()))
                    throw new HttpRequestException($"{request.RequestUri} is currently prohibited due to a ratelimit.");
            }

            WebResult result = new WebResult(response);
            
            if (result.IsSuccess)
            {
                if (_rateLimit != null)
                    await _rateLimit.OnSuccessAsync(result);
            }

            if (_ensured)
                response.EnsureSuccessStatusCode();

            return result;
        }

        public void Dispose()
            => _client.Dispose();
    }
}
