using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Core.Converters;
using Orikivo.Systems.Wrappers.Core.Handlers;
using Orikivo.Systems.Wrappers.Tenor.Parameters;
using Orikivo.Systems.Wrappers.Tenor.Parameters.Enumerators;
using Orikivo.Systems.Wrappers.Tenor.Results;
using Orikivo.Systems.Wrappers.Core.Interfaces;

namespace Orikivo.Systems.Wrappers.Tenor
{
    public class TenorWrapper
    {
        private readonly IWebsiteHandler _site = new WebsiteHandler();

        private readonly string _key;
        private const string BaseUri = "https://api.tenor.com/v1";
        /*private const string baseKeyUri = "&key=";
        private const string baseAutoSearchUri = "/autocomplete";
        private const string baseSearchUri = "/search";
        private const string baseIdUri = "/gifs";
        private const string baseTrendingUri = "/trending";
        private const string baseShareUri = "/registershare";
        private const string baseSuggestionUri = "/search_suggestions";
        private const string baseTagUri = "/tags";*/

        public TenorWrapper(string key = "")
        {
            _key = key;
        }

        public async Task<TenorRandomResults> GetGifRandomly(TenorRandom random)
        {
            var collection = new NameValueCollection
            {
                { "key", _key },
                { "q", random.Q }
            };
            if (random.SafeSearch != TenorRating.Disabled)
            {
                collection.Add("safesearch", random.SafeSearch.ToString());
            }

            var url = await _site.GetUri(new Uri($"{BaseUri}/random{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("The Tenor API GIFs you called for failed to work.\n" + url + url.Exception);
            }

            return JsonConvert.DeserializeObject<TenorRandomResults>(url.JsonData);
        }
    }
}