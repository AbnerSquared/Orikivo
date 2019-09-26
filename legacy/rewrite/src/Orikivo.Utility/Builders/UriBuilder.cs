namespace Orikivo.Utility
{
    /*
    public class UriBuilder
    {
        public UriBuilder() { }

        public UriBuilder(string url, string endpoint = "")
        {
            Base = url;
            Endpoint = endpoint;
        }

        public UriBuilder(string url, string endpoint, string k, string v) : this(url, endpoint)
        {
            Add(k, v);
        }

        public UriBuilder(string url, string endpoint, params (string k, string v)[] query) : this(url, endpoint)
        {
            AddMany(query);
        }

        public static string ToField(string key, string value)
            => $"{key}={value}";
        public static string ToQuery(NameValueCollection query)
        {
            StringBuilder q = new StringBuilder();
            string[] fields = (from k in query.AllKeys from v in query.GetValues(k) select ToField(k, v)).ToArray();
            if (fields.Length > 0)
            {
                q.Append($"?{fields.Conjoin("&")}");
                return q.ToString();
            }
            return "";
        }

        public string Base { get; set; }
        public string Endpoint { get; set; }
        public NameValueCollection Query { get; set; } = new NameValueCollection();

        public void SetEndpoint(string endpoint)
            => Endpoint = endpoint;

        public UriBuilder WithEndpoint(string endpoint)
        {
            SetEndpoint(endpoint);
            return this;
        }

        public UriBuilder Clear()
        {
            Query = new NameValueCollection();
            return this;
        }

        public UriBuilder WithQuery(string key, string value)
        {
            Add(key, value);
            return this;
        }

        public UriBuilder WithManyQueries(params (string k, string v)[] f)
        {
            AddMany(f);
            return this;
        }

        public string ReadQuery()
            => ToQuery(Query);

        public void Add(string k, string v)
            => Query.Add(k, v);

        public void Add((string k, string v) p)
            => Add(p.k, p.v);

        public void AddMany(params (string k, string v)[] f)
        {
            foreach ((string k, string v) p in f)
                Add(p);
        }

        public UriBuilder WithQuery(NameValueCollection q)
        {
            Query = q;
            return this;
        }

        public string Read()
            => $"{Base}{Endpoint}{ReadQuery()}";

        public Uri Build()
            => new Uri(Read());
    }
    */
}