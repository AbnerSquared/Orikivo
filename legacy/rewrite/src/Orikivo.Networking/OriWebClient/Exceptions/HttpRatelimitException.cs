using System;
using System.Net.Http;

namespace Orikivo.Networking
{
    public class HttpRateLimitException : HttpRequestException
    {
        private Uri _uri;
        public override string Message => $"{_uri.ToString()} has been ratelimited.";
        public HttpRateLimitException(Uri uri) : base()
        {
            _uri = uri;
        }
    }
}
