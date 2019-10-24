using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// A tool used to simplify URI construction.
    /// </summary>
    public class UriBuilder
    {
        /// <summary>
        /// Creates a new UriBuilder with a specified baseURL.
        /// </summary>
        public UriBuilder(string baseUrl)
        {
            if (!Checks.NotNull(baseUrl))
                throw new ArgumentNullException("A UriBuilder requires a valid base URL.");
            BaseUrl = baseUrl.Trim('/', ' ', '\n') + '/';
        }

        /// <summary>
        /// Defines the root website to utilize.
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// Defines the main route that the URI will focus on.
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// The collection of query arguments in place for the URI.
        /// </summary>
        public List<QueryArg> Args { get; } = new List<QueryArg>();

        /// <summary>
        /// Gets the string formatted query used for the URI.
        /// </summary>
        public string Query => Args.Count > 0 ? string.Join("&", Args.Select(x => x.ToString())) : string.Empty;

        /// <summary>
        /// Sets the endpoint for the URI.
        /// </summary>
        public UriBuilder WithNewEndpoint(string endpoint)
        {
            Endpoint = endpoint.Trim('/');
            return this;
        }

        /// <summary>
        /// Concatenates an endpoint to the existing endpoint set in place.
        /// </summary>
        public UriBuilder WithEndpoint(string endpoint)
        {
            Endpoint += $"{(Checks.NotNull(endpoint) ? "/" : string.Empty)}{endpoint.Trim('/')}";
            return this;
        }

        /// <summary>
        /// Creates a query argument and adds it into the argument pool.
        /// </summary>
        public UriBuilder WithArg(string key, string value)
            => WithArg(new QueryArg(key, value));

        /// <summary>
        /// Adds a query argument into the argument pool.
        /// </summary>
        public UriBuilder WithArg(QueryArg arg)
        {
            Args.Add(arg);
            return this;

        }

        /// <summary>
        /// Appends a collection of query arguments for the URI.
        /// </summary>
        public UriBuilder WithArgs(params QueryArg[] args)
            => WithArgs(args.ToList());

        /// <summary>
        /// Appends a collection of query arguments for the URI.
        /// </summary>
        public UriBuilder WithArgs(IEnumerable<QueryArg> args)
        {
            Args.AddRange(args);
            return this;
        }

        /// <summary>
        /// Clears all of the arguments set for the URI.
        /// </summary>
        public void ClearArgs()
            => Args.Clear();

        /// <summary>
        /// Completely clears all optional fields set for the UriBuilder.
        /// </summary>
        public void Clear()
        {
            Endpoint = string.Empty;
            Args.Clear();
        }

        /// <summary>
        /// Creates a new copy of the UriBuilder with its specified base URL.
        /// </summary>
        public UriBuilder Copy()
            => new UriBuilder(BaseUrl);

        /* NOTES:
         In short, endpoints are always split using '/'.
         We could expand upon this, allowing a method to move out of an endpoint tab by its '/'.
         The name could be something like UriBuilder.MoveUp();
         We could also allow the user to specify all of the endpoints at once, and go to an endpoint by index?
        */

        /// <summary>
        /// Creates a new URI.
        /// </summary>
        public Uri Build()
            => new Uri(ToString());

        /// <summary>
        /// Returns the string format of the UriBuilder.
        /// </summary>
        public override string ToString()
            => $"{BaseUrl}{(Checks.NotNull(Endpoint) ? Endpoint : string.Empty)}{Query}";

        
    }
}
