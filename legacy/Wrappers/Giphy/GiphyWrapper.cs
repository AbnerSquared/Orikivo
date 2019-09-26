using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Core.Converters;
using Orikivo.Systems.Wrappers.Core.Handlers;
using Orikivo.Systems.Wrappers.Giphy.Parameters;
using Orikivo.Systems.Wrappers.Giphy.Parameters.Enumerators;
using Orikivo.Systems.Wrappers.Giphy.Results;
using Orikivo.Systems.Wrappers.Core.Interfaces;

namespace Orikivo.Systems.Wrappers.Giphy
{
    public class GiphyWrapper
    {
        private readonly IWebsiteHandler _site = new WebsiteHandler();
        private readonly string _key;
        private const string BaseUri = "http://api.giphy.com/v1/gifs";

        public GiphyWrapper(string key = "")
        {
            _key = key;
        }

        public async Task<GiphySearchResults> SearchGif(GiphySearch search)
        {
            if (string.IsNullOrEmpty(search.Query))
            {
                throw new FormatException("You must place a query/term to search for.");
            }

            var collection = new NameValueCollection
            {
                { "api_key", _key },
                { "q", search.Query },
                { "limit", search.Limit.ToString() },
                { "offset", search.Rating.ToRatingString() }
            };
            if (search.Rating != GiphyRatings.Empty)
            {
                collection.Add("rating", search.Rating.ToRatingString());
            }

            if (!string.IsNullOrEmpty(search.Format))
            {
                collection.Add("fmt", search.Format);
            }

            var url = await _site.GetUri(new Uri($"{BaseUri}/search{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("The GIFs you called for failed to work.");
            }
            return JsonConvert.DeserializeObject<GiphySearchResults>(url.JsonData);
        }
        /*Single GIF ID*/
        public async Task<GiphyIdResult> GetGifByIdType(string id)
        {
            var collection = new NameValueCollection
            {
                { "api_key", _key }
            };

            var url = await _site.GetUri(new Uri($"{BaseUri}/{id}{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("The GIFs you called for failed to work.");
            }
            return JsonConvert.DeserializeObject<GiphyIdResult>(url.JsonData);
        }
        /*Multiple GIF IDs*/
        public async Task<GiphyIdResults> GetGifByIdTypes(string[] ids)
        {
            var collection = new NameValueCollection
            {
                { "api_key", _key },
                { "ids", string.Join(",", ids) }
            };

            var url = await _site.GetUri(new Uri($"{BaseUri}/{ConvertUri.ToUrlString(collection, false)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("The GIFs you called for failed to work.");
            }
            return JsonConvert.DeserializeObject<GiphyIdResults>(url.JsonData);
        }

        public async Task<GiphyIdResult> TranslateGif(GiphyTranslation translate)
        {
            if (string.IsNullOrEmpty(translate.Phrase))
            {
                throw new FormatException("You must place a query/term to translate.");
            }

            var collection = new NameValueCollection
            {
                { "api_key", _key },
                { "s", translate.Phrase }
            };
            if (translate.Rating != GiphyRatings.Empty)
            {
                collection.Add("rating", translate.Rating.ToRatingString());
            }
            if (!string.IsNullOrEmpty(translate.Format))
            {
                collection.Add("fmt", translate.Format);
            }

            var url = await _site.GetUri(new Uri($"{BaseUri}/translate{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("The GIFs you called for failed to work.");
            }
            return JsonConvert.DeserializeObject<GiphyIdResult>(url.JsonData);
        }

        public async Task<GiphyRandomResult> GetGifRandomly(GiphyRandom random)
        {
            var collection = new NameValueCollection
            {
                { "api_key", _key }
            };
            if (random.Rating != GiphyRatings.Empty)
            {
                collection.Add("rating", random.Rating.ToRatingString());
            }
            if (!string.IsNullOrEmpty(random.Format))
            {
                collection.Add("fmt", random.Format);
            }
            if (!string.IsNullOrEmpty(random.Tag))
            {
                collection.Add("tag", random.Tag);
            }

            var url = await _site.GetUri(new Uri($"{BaseUri}/random{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("The GIFs you called for failed to work.");
            }
            return JsonConvert.DeserializeObject<GiphyRandomResult>(url.JsonData);
        }

        public async Task<GiphySearchResults> GetGifByTrendingTypes(GiphyTrending trend)
        {
            var collection = new NameValueCollection
            {
                { "api_key", _key },
                { "limit", trend.Limit.ToString() }
            };
            if (trend.Rating != GiphyRatings.Empty)
            {
                collection.Add("rating", trend.Rating.ToRatingString());
            }
            if (!string.IsNullOrEmpty(trend.Format))
            {
                collection.Add("fmt", trend.Format);
            }

            var url = await _site.GetUri(new Uri($"{BaseUri}/trending{ConvertUri.ToUrlString(collection)}"));
            if (!url.IsSuccess)
            {
                throw new WebException("The GIFs you called for failed to work.");
            }
            return JsonConvert.DeserializeObject<GiphySearchResults>(url.JsonData);
        }
    }
}