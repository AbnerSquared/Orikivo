using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class UriBuilder
    {
        public string ApiUrl { get; set; }
        public string Endpoint { get; set; }
        public NameValueCollection Query { get; set; }

        public static string FormatField(string key, string value)
            => $"{key}={value}";

        public static string ConcatQuery(NameValueCollection query)
        {
            StringBuilder sb = new StringBuilder();
            string[] fields = (from k in query.AllKeys from v in query.GetValues(k) select FormatField(k, v)).ToArray();
            if (fields.Length > 0)
                sb.Append($"?{string.Join("&", fields)}");
            return sb.ToString();
            
        }

        public UriBuilder()
        {
            Query = new NameValueCollection();
        }

        public UriBuilder(string rootAddress, string endpoint = "", NameValueCollection query = null) : this()
        {
            ApiUrl = rootAddress;
            Endpoint = endpoint;
            Query = query ?? Query;
        }

        public UriBuilder WithEndpoint(string endpoint)
        {
            UriBuilder uriBuilder = this;
            uriBuilder.Endpoint = endpoint;
            return uriBuilder;
        }

        public UriBuilder ClearQuery()
        {
            UriBuilder uriBuilder = this;
            uriBuilder.Query = new NameValueCollection();
            return this;
        }

        public UriBuilder Concat(string key, string value)
        {
            UriBuilder uriBuilder = this;
            uriBuilder.Query.Add(key, value);
            return uriBuilder;
        }

        public UriBuilder WithQuery(NameValueCollection query)
        {
            UriBuilder uriBuilder = this;
            uriBuilder.Query = query;
            return uriBuilder;
        }

        // in short, the endpoints should always be split by /
        // we could add new methods, allowing a user to back out of an endpoint
        // UriBuilder.MoveUp(); // takes a step back from the endpoint, if possible.
        public override string ToString()
            => $"{ApiUrl}{Endpoint}{ConcatQuery(Query)}"; // www.discord.com / endpoint1 / endpoint2 ? key=value&key=value&key=value 

        public Uri Build()
            => new Uri(ToString());
    }
}
