using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orikivo.Systems.Wrappers.Core.Converters;
using Orikivo.Systems.Wrappers.Core.Handlers;
using Orikivo.Systems.Wrappers.Overwatch.Objects.ProfileObject;
using Orikivo.Systems.Wrappers.Overwatch.Parameters;
using Orikivo.Systems.Wrappers.Overwatch.Parameters.Enumerators;
using Orikivo.Systems.Wrappers.Core.Interfaces;

namespace Orikivo.Systems.Wrappers.Overwatch
{
    public class OverwatchWrapper
    {
        private readonly IWebsiteHandler _site = new WebsiteHandler();
        private const string BaseUri = "https://ow-api.com/v1/stats";

        public async Task<OverwatchProfileData> GetProfile(OverwatchProfile profile)
        {
            if (string.IsNullOrEmpty(profile.BattleTag))
            {
                throw new FormatException("You must type a battletag to get the information.");
            }
            var collection = new NameValueCollection
            {
                { "platform", profile.Platform.ToPlatformString() },
                { "region", profile.Region.ToRegionString() },
                { "battletag", profile.BattleTag }
            };
            var url = await _site.GetUri(new Uri($"{BaseUri}{ConvertOwUri.ToUrlString(collection)}/profile"));
            if (!url.IsSuccess)
            {
                throw new WebException("The profile you called for failed to function.");
            }

            return JsonConvert.DeserializeObject<OverwatchProfileData>(url.JsonData);

        }
    }
}