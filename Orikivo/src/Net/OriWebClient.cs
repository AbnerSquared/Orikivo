using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Net
{
    public class OriWebClient : IDisposable
    {
        public static OriWebClient Default = new OriWebClient();

        private static HttpClient DefaultHttpClient
        {
            get
            {
                HttpClient client = new HttpClient();
                ProductInfoHeaderValue agent = new ProductInfoHeaderValue(OriGlobal.ClientName, OriGlobal.ClientVersion);
                client.DefaultRequestHeaders.UserAgent.Add(agent);
                return client;
            }
        }

        internal HttpClient _client;
        internal IHttpRateLimit _rateLimit;
        internal bool _ensured;

        public HttpRequestHeaders Headers => _client.DefaultRequestHeaders;

        internal OriWebClient()
        {
            _client = DefaultHttpClient;
        }

        public OriWebClient(string address) : this()
        {
            SetBaseAddress(address);
        }

        public OriWebClient(string address, bool ensured) : this(address)
        {
            _ensured = ensured;
        }

        // this is used to set up an ApiHandler
        // you can set the root address of an api service here
        // and when you request, you just need to type the endpoints.
        public void SetBaseAddress(string address)
            => _client.BaseAddress = new Uri(address);

        public void SetAuthorization(string scheme)
            => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme);

        public void SetAuthorization(string scheme, string parameter)
            => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);

        public void AddRequestHeader(string header, string value)
            => _client.DefaultRequestHeaders.Add(header, value);

        public async Task<OriWebResult<T>> RequestAsync<T>(string url)
            => await RequestAsync<T>(new Uri(url));
        public async Task<OriWebResult<T>> RequestAsync<T>(Uri uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            return new OriWebResult<T>(response);
        }

        public async Task<OriWebResult> RequestAsync(string url)
            => await RequestAsync(new Uri(url));
        public async Task<OriWebResult> RequestAsync(Uri uri)
        {
            HttpResponseMessage response = await _client.GetAsync(uri);
            return new OriWebResult(response);
        }

        public async Task<OriWebResult> GetAsync(string url = "")
            => await SendAsync(HttpMethod.GET, url);

        public async Task<OriWebResult> DeleteAsync(string url = "")
            => await SendAsync(HttpMethod.DELETE, url);

        public async Task<OriWebResult> PostAsync(string url = "", string value = null)
            => await SendAsync(HttpMethod.POST, url, value);

        public async Task<OriWebResult> PatchAsync(string url = "", string value = null)
            => await SendAsync(HttpMethod.PATCH, url, value);

        public async Task<OriWebResult> PutAsync(string url = "", string value = null)
            => await SendAsync(HttpMethod.PUT, url, value);

        public async Task<OriWebResult> SendAsync(HttpMethod requestType, string url = "", string value = null)
        {
            url.TrimStart('/');
            System.Net.Http.HttpMethod method = new System.Net.Http.HttpMethod(requestType.ToString());
            using (HttpRequestMessage request = new HttpRequestMessage(method, url))
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    request.Content = new StringContent(value, Encoding.UTF8, $"application/{HttpMedia.JSON.ToString().ToLower()}");
                    request.Content.Headers.ContentType.CharSet = null; // This is the UTF-8 being removed, to make sure it can actually format the value given.
                }
                return await SendAsync(request);
            }
        }

        public async Task<OriWebResult> SendAsync(HttpRequestMessage request)
        {
            HttpResponseMessage response = await _client.SendAsync(request);

            if (_rateLimit != null)
                if (!await _rateLimit.CanRequestAsync((HttpMethod)Enum.Parse(typeof(HttpMethod), request.Method.Method.ToUpper()), request.RequestUri.ToString()))
                    throw new HttpRequestException($"{request.RequestUri} is currently prohibited due to a ratelimit.");
            OriWebResult result = new OriWebResult(response);
            if (result.IsSuccess)
                if (_rateLimit != null)
                    await _rateLimit.OnSuccessAsync(result);
            if (_ensured)
                response.EnsureSuccessStatusCode();
            return result;
        }


        public void Dispose()
            => _client.Dispose();
    }
}
