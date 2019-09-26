using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Networking
{
    public class OriWebClient : IOriWebClient, IDisposable
    {
        public static OriWebClient Default = new OriWebClient();

        private static HttpClient DefaultClient
        {
            get
            {
                HttpClient client = new HttpClient();
                ProductInfoHeaderValue agent = new ProductInfoHeaderValue(Global.ClientName, Global.ClientVersion);
                client.DefaultRequestHeaders.UserAgent.Add(agent);
                return client;
            }
        }

        internal HttpClient _client;
        internal IRateLimit _rateLimit;
        internal bool _ensured;

        public HttpRequestHeaders Headers => _client.DefaultRequestHeaders;

        internal OriWebClient()
        {
            _client = DefaultClient;
        }

        public OriWebClient(string address) : this()
        {
            SetBaseAddress(address);
        }

        public OriWebClient(string address, bool ensured) : this()
        {
            SetBaseAddress(address);
            _ensured = ensured;
        }

        public void SetBaseAddress(string address)
            => _client.BaseAddress = new Uri(address);

        public void SetAuthorization(string key)
            => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(key);

        public void SetAuthorization(string scheme, string parameter)
            => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);

        public void AddHeader(string header, string value)
            => _client.DefaultRequestHeaders.Add(header, value);

        public async Task<WebResponse<T>> RequestAsync<T>(string url)
            => await RequestAsync<T>(new Uri(url));

        public async Task<WebResponse<T>> RequestAsync<T>(Uri uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            return new WebResponse<T>(response);
        }

        public async Task<WebSourceResponse> RequestSourceAsync(string url)
            => await RequestSourceAsync(new Uri(url));

        public async Task<WebSourceResponse> RequestSourceAsync(Uri uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            return new WebSourceResponse(response);
        }

        public async Task<WebResponse> GetAsync(string url = "")
            => await SendAsync(HttpRequestType.GET, url);

        public async Task<WebResponse> DeleteAsync(string url = "")
            => await SendAsync(HttpRequestType.DELETE, url);

        public async Task<WebResponse> PostAsync(string url = "", string value = null)
            => await SendAsync(HttpRequestType.POST, url, value);

        public async Task<WebResponse> PatchAsync(string url = "", string value = null)
            => await SendAsync(HttpRequestType.PATCH, url, value);

        public async Task<WebResponse> PutAsync(string url = "", string value = null)
            => await SendAsync(HttpRequestType.PUT, url, value);

        public async Task<WebResponse> SendAsync(HttpRequestType type, string url = "", string value = null)
        {
            url.TrimStart('/');
            HttpMethod method = new HttpMethod(type.Read());
            using (HttpRequestMessage request = new HttpRequestMessage(method, url))
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    request.Content = new StringContent(value, Encoding.UTF8, HttpApplicationType.JSON.Read());
                    request.Content.Headers.ContentType.CharSet = null; // This is the UTF-8 being removed, to make sure it can actually format the value given.
                }
                return await SendAsync(request);
            }
        }

        public async Task<WebResponse> SendAsync(HttpRequestMessage request)
        {
            HttpResponseMessage message = await _client.SendAsync(request);

            if (_rateLimit.Exists())
                if (!await _rateLimit.CanRequestAsync((HttpRequestType)Enum.Parse(typeof(HttpRequestType), request.Method.Method.ToUpper()), request.RequestUri.ToString()))
                    throw new HttpRateLimitException(request.RequestUri);
            WebResponse response = new WebResponse(message);
            if (response.IsSuccess)
                if (_rateLimit.Exists())
                    await _rateLimit.OnSuccessAsync(response);
            if (_ensured)
                message.EnsureSuccessStatusCode();
            return response;
        }

        public void Dispose()
            => _client.Dispose();
    }
}